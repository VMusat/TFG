using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.Barracuda;

public class PlayerAgent : Agent
{
    public bool Discrete;
    public bool ReadFromTraining;
    public Player EnemyPlayer;

    private Player ThisPlayer;
    private TrainingAgent TrainingAgent;
    private float RewardEpisode;
    private float TimeBegin;
    private int NumberUnitsSpawned;
    private int NumberEnemyUnitsKilled;
    private int AllyTowerDamage;
    private int EnemyTowerDamage;
    private int NumberSoldiers;
    private int NumberCatapults;
    private int NumberBuildings;

    private bool EnemyTowerDestroyed;
    private bool AllyTowerDestroyed;
    private bool TieMatch;

    //Soldado, Caballero, Catapulta, Barbaro
    private int[] UnitCount;
    //Casa, Granja, Aserradero
    private int[] BuilCount;


    private void Awake()
    {
        ThisPlayer = GetComponent<Player>();
        TrainingAgent = GetComponent<TrainingAgent>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //EnemyPlayer.OnUnitDead += OnEnemyUnitKilled;
        //ThisPlayer.OnUnitDead += OnAllyUnitKilled;
        //EnemyPlayer.tower.OnDamaged += OnEnemyTowerDamaged;
        //ThisPlayer.tower.OnDamaged += OnAllyTowerDamaged;
    }
    
    protected override void OnDisable() {
        base.OnDisable();
        //EnemyPlayer.OnUnitDead -= OnEnemyUnitKilled;
        //ThisPlayer.OnUnitDead -= OnAllyUnitKilled;
        //EnemyPlayer.tower.OnDamaged -= OnEnemyTowerDamaged;
        //ThisPlayer.tower.OnDamaged -= OnAllyTowerDamaged;
    }

     public override void OnEpisodeBegin()
    {
        Debug.Log(name + " - RewardEpisode: " + RewardEpisode);
        RewardEpisode = 0.0f;
        NumberEnemyUnitsKilled = 0;
        NumberUnitsSpawned = 0;
        NumberBuildings = 0;
        AllyTowerDamage = 0;
        EnemyTowerDamage = 0;
        NumberSoldiers = 0;
        NumberCatapults = 0;
        UnitCount = new int[4];
        BuilCount = new int[3];
        EnemyTowerDestroyed = false;
        AllyTowerDestroyed = false;
        TieMatch = false;
        TimeBegin = Time.time;
        if (ThisPlayer.Units != null && EnemyPlayer.Units != null)
        {
            ThisPlayer.Units[Unit.UnitType.Soldier].Clear();
            ThisPlayer.Units[Unit.UnitType.Knight].Clear();
            ThisPlayer.Units[Unit.UnitType.Catapult].Clear();
            ThisPlayer.Units[Unit.UnitType.Barbarian].Clear();

            EnemyPlayer.Units[Unit.UnitType.Soldier].Clear();
            EnemyPlayer.Units[Unit.UnitType.Knight].Clear();
            EnemyPlayer.Units[Unit.UnitType.Catapult].Clear();
            EnemyPlayer.Units[Unit.UnitType.Barbarian].Clear();
        }
        if (ReadFromTraining)
        {
            TrainingAgent.Reset();
        }
    }

     public override void CollectObservations(VectorSensor sensor){
        sensor.AddObservation((float)ThisPlayer.ThisTower.Health / (float)ThisPlayer.ThisTower.InitialHealth);
        
        float maxPop = ThisPlayer.popMax;
        const float maxMat = 200.0f;
        const float maxMatPerSec = 3.0f;
        const float maxFood = 200.0f;
        const float maxFoodPerSec = 3.0f;

        sensor.AddObservation(Mathf.Clamp01((float)ThisPlayer.population / maxPop));
        sensor.AddObservation(Mathf.Clamp01((float)EnemyPlayer.population / maxPop));

        sensor.AddObservation(Mathf.Clamp01((float)ThisPlayer.materials / maxMat));
        sensor.AddObservation(Mathf.Clamp01((float)EnemyPlayer.materials / maxMat));
        sensor.AddObservation(Mathf.Clamp01((float)ThisPlayer.matPerSec / maxMatPerSec));
        sensor.AddObservation(Mathf.Clamp01((float)EnemyPlayer.matPerSec / maxMatPerSec));

        sensor.AddObservation(Mathf.Clamp01((float)ThisPlayer.food / maxFood));
        sensor.AddObservation(Mathf.Clamp01((float)EnemyPlayer.food / maxFood));
        sensor.AddObservation(Mathf.Clamp01((float)ThisPlayer.foodPerSec / maxFoodPerSec));
        sensor.AddObservation(Mathf.Clamp01((float)EnemyPlayer.foodPerSec / maxFoodPerSec));

        const float maxSoldiers = 5.0f;
        const float maxCatapults = 3.0f;
        
        Mathf.Clamp01((float)ThisPlayer.Units[Unit.UnitType.Soldier].Count / maxSoldiers);
        Mathf.Clamp01((float)EnemyPlayer.Units[Unit.UnitType.Soldier].Count / maxSoldiers);
        Mathf.Clamp01((float)ThisPlayer.Units[Unit.UnitType.Catapult].Count / maxCatapults);
        Mathf.Clamp01((float)EnemyPlayer.Units[Unit.UnitType.Catapult].Count / maxCatapults);
        
        sensor.AddObservation((float)ThisPlayer.Units[Unit.UnitType.Knight].Count > 0 ? 1.0f : 0.0f);
        sensor.AddObservation((float)EnemyPlayer.Units[Unit.UnitType.Knight].Count > 0 ? 1.0f : 0.0f);
        sensor.AddObservation((float)ThisPlayer.Units[Unit.UnitType.Barbarian].Count > 0 ? 1.0f : 0.0f);
        sensor.AddObservation((float)EnemyPlayer.Units[Unit.UnitType.Barbarian].Count > 0 ? 1.0f : 0.0f);

        float closerDistAlly = GetCloserUnit(ThisPlayer, EnemyPlayer); // Closer Ally Unit (0 at our tower, 1 at enemy's tower)
        float closerDistEnemy = GetCloserUnit(EnemyPlayer, ThisPlayer); // Closer Enemy Unit (0 at enemy's tower, 1 at our tower)
        sensor.AddObservation(closerDistAlly);
        sensor.AddObservation(closerDistEnemy);
        
     }

     public override void OnActionReceived(ActionBuffers actions){
        if (EnemyTowerDestroyed)
        {
            SetReward(1.0f);
            EndEpisode();
        }
        else if (TieMatch)
        {
            SetReward(0.0f);
            EndEpisode();
        }
        else if (AllyTowerDestroyed)
        {
            SetReward(-1.0f);
            EndEpisode();
        }

        int action;
        if (Discrete)
        {
            action = actions.DiscreteActions[0];
        }
        else
        {
            action = 0;
            float probability = actions.ContinuousActions[0];
            for (int i = 1; i < 8; ++i)
            {
                if (actions.ContinuousActions[i] > probability)
                {
                    action = i;
                    probability = actions.ContinuousActions[i];
                }
            }
        }

        float maxPop = ThisPlayer.popMax;
        const float maxMat = 200.0f;
        const float maxMatPerSec = 3.0f;
        const float maxFood = 200.0f;
        const float maxFoodPerSec = 3.0f;

        float pop = Mathf.Clamp01((float)ThisPlayer.population / maxPop);
        float enemyPop = Mathf.Clamp01((float)EnemyPlayer.population / maxPop);

        float mat = Mathf.Clamp01((float)ThisPlayer.materials / maxMat);
        float enemyMat = Mathf.Clamp01((float)EnemyPlayer.materials / maxMat);
        float matPerSec = Mathf.Clamp01((float)ThisPlayer.matPerSec / maxMatPerSec);
        float enemyMatPerSec = Mathf.Clamp01((float)EnemyPlayer.matPerSec / maxMatPerSec);

        float food = Mathf.Clamp01((float)ThisPlayer.food / maxFood);
        float enemyFood = Mathf.Clamp01((float)EnemyPlayer.food / maxFood);
        float foodPerSec = Mathf.Clamp01((float)ThisPlayer.foodPerSec / maxFoodPerSec);
        float enemyFoodPerSec = Mathf.Clamp01((float)EnemyPlayer.foodPerSec / maxFoodPerSec);

        const float maxSoldiers = 5.0f;
        const float maxCatapults = 3.0f;
        
        float soldiers = Mathf.Clamp01((float)ThisPlayer.Units[Unit.UnitType.Soldier].Count / maxSoldiers);
        float enemySoldiers = Mathf.Clamp01((float)EnemyPlayer.Units[Unit.UnitType.Soldier].Count / maxSoldiers);
        float catapults = Mathf.Clamp01((float)ThisPlayer.Units[Unit.UnitType.Catapult].Count / maxCatapults);
        float enemyCatapults = Mathf.Clamp01((float)EnemyPlayer.Units[Unit.UnitType.Catapult].Count / maxCatapults);
        
        float knights = (float)ThisPlayer.Units[Unit.UnitType.Knight].Count > 0 ? 1.0f : 0.0f;
        float enemyKnights = (float)EnemyPlayer.Units[Unit.UnitType.Knight].Count > 0 ? 1.0f : 0.0f;
        float barbarians = (float)ThisPlayer.Units[Unit.UnitType.Barbarian].Count > 0 ? 1.0f : 0.0f;
        float enemyBarbarians = (float)EnemyPlayer.Units[Unit.UnitType.Barbarian].Count > 0 ? 1.0f : 0.0f;

        float closerDistAlly = GetCloserUnit(ThisPlayer, EnemyPlayer);
        float closerDistEnemy = GetCloserUnit(EnemyPlayer, ThisPlayer);

        //Recompensas por reclutar
        if(action == 2 && enemySoldiers >= 0.6f) AddReward(0.1f); // Tres o mas soldados -> Caballero
        if(action == 4 && enemyKnights == 1.0f) AddReward(0.1f); // Hay caballero -> Barbaro
        if(action == 1 && enemyBarbarians == 1.0f) AddReward(0.1f); // Hay barbaro -> Soldado
        if(action == 2 && enemyCatapults >= 0.6f) AddReward(0.1f); //Dos o mas catapultas -> Caballero
        if(action == 3 && soldiers >= 0.4f) AddReward(0.1f); // Dos soldados aliados -> Catapulta

        //Recompensas por construir
        if(action == 5 && pop >= 0.90f) AddReward(0.1f);
        if(action == 7 && mat >= 0.25f && closerDistEnemy <= 0.3f) AddReward(0.1f);
        if(action == 6 && mat >= 0.25f && food <= 0.20f) AddReward(0.1f);

        bool done;
        switch (action){
            case 0: //Nada
                break;
            case 1: //Soldado
                done = ThisPlayer.Spawn(Unit.UnitType.Soldier);
                if (done){
                    NumberUnitsSpawned += 1;
                    if (NumberSoldiers >= maxSoldiers){
                        AddReward(-0.1f);
                    }
                    NumberSoldiers += 1;
                    this.UnitCount[0] += 1;
                }
                break;
            case 2: //Caballero
                done = ThisPlayer.Spawn(Unit.UnitType.Knight);
                if (done){
                    NumberUnitsSpawned += 1;
                    this.UnitCount[1] += 1;
                } 
                break;
            case 3: //Catapulta
                done = ThisPlayer.Spawn(Unit.UnitType.Catapult);
                if (done){
                    NumberUnitsSpawned += 1;
                    if (NumberSoldiers >= maxCatapults){
                        AddReward(-0.1f);
                    }
                    NumberCatapults += 1;
                    this.UnitCount[2] += 1;
                }
                break;
            case 4: //Barbaro
                done = ThisPlayer.Spawn(Unit.UnitType.Barbarian);
                if (done){
                    NumberUnitsSpawned += 1;
                    this.UnitCount[3] += 1;
                }
                break;
            case 5: //Hogar
                done = ThisPlayer.build(new House());
                if (done){
                    NumberBuildings += 1;
                    this.BuilCount[0] += 1;
                } 
                break;
            case 6: //Granja
                done = ThisPlayer.build(new Farm());
                if (done){
                    NumberBuildings += 1;
                    this.BuilCount[1] += 1;
                } 
                break;
            case 7: //Aserradero
                done = ThisPlayer.build(new Sawmill());
                if (done){
                    NumberBuildings += 1;
                    this.BuilCount[2] += 1;
                } 
                break;   
        }
     }

    bool alpha1Down, alpha2Down, alpha3Down, alpha4Down, alpha5Down, alpha6Down;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) alpha1Down = true;
        if (Input.GetKeyDown(KeyCode.Alpha2)) alpha2Down = true;
        if (Input.GetKeyDown(KeyCode.Alpha3)) alpha3Down = true;
        if (Input.GetKeyDown(KeyCode.Alpha4)) alpha4Down = true;
        if (Input.GetKeyDown(KeyCode.Alpha5)) alpha5Down = true;
        if (Input.GetKeyDown(KeyCode.Alpha6)) alpha6Down = true;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        if (Discrete)
        {
            ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
            if (ReadFromTraining)
            {
                discreteActions[0] = TrainingAgent.Action;
            }else
            {
                discreteActions[0] = 0;
                if (alpha1Down)
                {
                    discreteActions[0] = 1;
                }
                if (alpha2Down)
                {
                    discreteActions[0] = 2;
                }
                if (alpha3Down)
                {
                    discreteActions[0] = 3;
                }
                if (alpha4Down)
                {
                    discreteActions[0] = 4;
                }
                if (alpha5Down)
                {
                    discreteActions[0] = 5;
                }
                if (alpha6Down)
                {
                    discreteActions[0] = 6;
                }
            }

        }else
        {
            ActionSegment<float> continousActions = actionsOut.ContinuousActions;

            if (ReadFromTraining)
            {
                for (int i = 0; i < 7; ++i)
                {
                    if (i == TrainingAgent.Action) continousActions[i] = 1.0f;
                    else continousActions[i] = 0.0f;
                }
            }else
            {
                for (int i = 0; i < 7; ++i)
                {
                    continousActions[i] = 0.0f;
                }
                if (alpha1Down)
                {
                    continousActions[1] = 1.0f;
                }
                if (alpha2Down)
                {
                    continousActions[2] = 1.0f;
                }
                if (alpha3Down)
                {
                    continousActions[3] = 1.0f;
                }
                if (alpha4Down)
                {
                    continousActions[4] = 1.0f;
                }
                if (alpha5Down)
                {
                    continousActions[5] = 1.0f;
                }
                if (alpha6Down)
                {
                    continousActions[6] = 1.0f;
                }
            }
        }
        alpha1Down = alpha2Down = alpha3Down = alpha4Down = alpha5Down = alpha6Down = false;   
    }

    public void OnAllyTowerDamaged(int amount)
    {
        AllyTowerDamage += amount;
    }

    public void OnEnemyTowerDamaged(int amount)
    {
        EnemyTowerDamage += amount;
    }

    public void OnAllyUnitKilled(Unit.UnitType unit)
    {
        if (unit == Unit.UnitType.Soldier) NumberSoldiers -= 1;
        if (unit == Unit.UnitType.Catapult) NumberCatapults -= 1;
    }

    public void OnEnemyUnitKilled(Unit.UnitType unit)
    {
        NumberEnemyUnitsKilled += 1;
    }

    public void RewardTowerDestroyed()
    {
        EnemyTowerDestroyed = true;
    }

    public void PenalizeTowerDestroyed()
    {
        AllyTowerDestroyed = true;
    }

    public void Tie()
    {
        TieMatch = true;
    }

    public void PenalizeTime()
    {
        const float reward = 4.0f / 480.0f;
        AddReward(-reward);
        RewardEpisode -= reward;
    }

    public List<int[]> returnStats(){
        List<int[]> stats = new List<int[]>();
        int[] numUnits = new int[1];
        int[] numBuilds = new int[1];
        numUnits[0] = this.NumberUnitsSpawned;
        numBuilds[0] = this.NumberBuildings;
        stats.Add(numUnits);
        stats.Add(UnitCount);
        stats.Add(numBuilds);
        stats.Add(BuilCount);
        return stats;
    }


    private float GetCloserUnit(Player source, Player dest){
        float closerDist = 0.0f;
        if (source != null && dest != null){
            Vector3 init = source.transform.position;
            Vector3 end = dest.transform.position;
            float maxDist = Vector3.Distance(init, end);
            foreach (Unit.UnitType unitT in uni){
                foreach(GameObject unit in source.Units[unitT]){
                    if (unit != null){
                        float dist = Vector3.Distance(unit.transform.position, init) / maxDist;
                        if (dist > closerDist){
                            closerDist = dist;
                        }
                    }
                }
            }
        }
        return closerDist;
    }
    private static Unit.UnitType[] uni = {Unit.UnitType.Barbarian, Unit.UnitType.Catapult, Unit.UnitType.Knight, Unit.UnitType.Soldier};


    



}
