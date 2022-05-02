using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingAgent : MonoBehaviour
{
    public Player EnemyPlayer;
    private Player ThisPlayer;
    public bool PlayWithoutAgent;
    public bool StaticSwords;
    public bool StaticSwordsPlus;
    public bool FullTraining;
    public int UpdateSteps = 10;
    private int CurrentSteps;
    public int Action { get; private set; }
    private int TrainingType;

    private void Awake()
    {
        ThisPlayer = GetComponent<Player>();
        Reset();
    }

    public void Reset()
    {
        if (!FullTraining)
        {
            TrainingType = Random.Range(0, 2);
        }
    }

    private void FixedUpdate()
    {
        if (CurrentSteps == 0) UpdateLogic();
        CurrentSteps = (CurrentSteps + 1) % UpdateSteps;
    }

    private void UpdateLogic(){
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

        //Actions: 0 = Nada, 1 = Soldado, 2 = Caballero, 3 = Catapulta, 4 = Barbaro, 5 = Hogar, 6 = Granja, 7 = Aserradero
        Action = 0;
        if (Random.Range(0.0f, 1.0f) < 0.1f)
        {
                Action = 1;
        }

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
