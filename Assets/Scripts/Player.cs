using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Player : MonoBehaviour
{
    public event Action<Unit.UnitType> OnUnitDead;
    public float population {get; private set; }
    public int popMax {get; private set; }
    public float popPerSec {get; private set; }
    public float food {get; private set; }
    public float foodPerSec {get; private set; }
    public float materials {get; private set; }
    public float matPerSec {get; private set; }
    public float popFoodConst {get; private set; }
    public TextMeshProUGUI PopText;
    public TextMeshProUGUI PopPerSecondText;
    public TextMeshProUGUI FoodText;
    public TextMeshProUGUI FoodPerSecondText;
    public TextMeshProUGUI MatText;
    public TextMeshProUGUI MatPerSecondText;
    public TextMeshProUGUI HousesText;
    public TextMeshProUGUI FarmsText;
    public TextMeshProUGUI SawmillsText;
    public BaseGrid Base;
    List<Building> buildingList = new List<Building>();
    //Variables Iniciales que cambian con las construcciones
    const int popMaxInit = 100;
    const float foodPerSecInit = 1;
    const float matPerSecInit = 1;
    //
    public Vector3 TowerLocationLocal;
    public bool IsPlayer;
    public Tower tower;
    public Dictionary<Unit.UnitType, List<GameObject>> Units;

    public Dictionary<Unit.UnitType, Dictionary<string, int>> UnitValues;

    void Start()
    {
        Units = new Dictionary<Unit.UnitType, List<GameObject>>();
        Units.Add(Unit.UnitType.Soldier, new List<GameObject>());
        Units.Add(Unit.UnitType.Barbarian, new List<GameObject>());
        Units.Add(Unit.UnitType.Knight, new List<GameObject>());
        Units.Add(Unit.UnitType.Catapult, new List<GameObject>());
        UnitValues = new Dictionary<Unit.UnitType, Dictionary<string, int>>(){
            {Unit.UnitType.Soldier, new Dictionary<string, int>(){
                {"Fcost",10}, {"Pcost",5}
            }},
            {Unit.UnitType.Knight, new Dictionary<string, int>(){
                {"Fcost",40}, {"Pcost",5}
            }},
            {Unit.UnitType.Catapult, new Dictionary<string, int>(){
                {"Fcost",20}, {"Pcost",10}
            }},
            {Unit.UnitType.Barbarian, new Dictionary<string, int>(){
                {"Fcost",20}, {"Pcost",5}
            }}
        };
    }

    public void Reset() {
        population = 50;
        popMax = popMaxInit;
        popPerSec = 1;
        food = 50;
        foodPerSec = foodPerSecInit;
        materials = 50;
        matPerSec = matPerSecInit;
        popFoodConst = 100;
        ResetUnits();
        ResetBuildings();
        UpdateFoodUI();
        UpdateFoodPerSecondUI();
        UpdateMatUI();
        UpdateMatPerSecondUI();
        UpdatePopUI();
        UpdatePopPerSecondUI();
        UpdateBuildingsUI();
        StopAllCoroutines();
        StartCoroutine(ResourceLoop());
        tower.Reset();
    }

    private WaitForSeconds WaitFor1Second = new WaitForSeconds(1.0f);
    public IEnumerator ResourceLoop(){
        while(true){
            yield return WaitFor1Second;
            calcPop();
            addPop(popPerSec);
            addFood(foodPerSec);
            addMat(matPerSec);
            addResourcesPS();
            if(tower.dead){
                StopAllCoroutines();
            }
        }
    }

    public void calcPop(){
        float popAux = popPerSec;
        popAux = ((food-population)/popFoodConst);
        popAux = Mathf.Pow(popAux, 3)+1;
        if (popAux>10) popAux = 10;
        if (popAux<-7) popAux = -7;
        popPerSec = (popAux);
        UpdatePopPerSecondUI();
    }

    public void addPop(float pop){
        if(population+pop < popMax){
            population += pop;
            if (population <= 0){
                population = 0;
                StopAllCoroutines();
            } 
        }else{
            population = popMax;
        }
        UpdatePopUI();
    }
    public void addFood(float fo){
        food += fo;
        if (food < 0) food = 0;
        UpdateFoodUI();
    }
    public void addMat(float mat){
        materials += mat;
        if (materials < 0) materials = 0;
        UpdateMatUI();
    }

    public bool build(Building buil){
        if (materials >= buil.getCost()){
            //string data = buil.data;
            //GameObject b = Instantiate(Resources.Load($"Prefabs/Buildings/{data}") as GameObject, new Vector3(0,0,0), Quaternion.identity);
            //b.transform.SetParent(Base.transform, false);

            if(Base.Ocupar(buil)){
                buildingList.Add(buil);
                materials = materials - buil.getCost();
                UpdateMatUI();
                UpdateBuildingsUI();
                return true;
            }else{ //else No queda espacio en la base
                return false;
            }
        }else{
            return false;
        }  
    }

    public bool Spawn(Unit.UnitType unit){
        int unitFCost = UnitValues[unit]["Fcost"];
        int unitPCost = UnitValues[unit]["Pcost"];
        if(unitFCost <= food && unitPCost <= population && IsCountUnits() && IsUnitsInBase()){
            GameObject prefab = Resources.Load($"Prefabs/Units/{unit.ToString()}") as GameObject;
            if (prefab != null){
                GameObject unitGO = Instantiate(prefab, transform.position + TowerLocationLocal, Quaternion.identity, transform);
                unitGO.GetComponentInChildren<Unit>().IsPlayer = IsPlayer;
                Units[unit].Add(unitGO);
            }
            addFood(-unitFCost);
            addPop(-unitPCost);
            return true;
        }else{
            return false;
        }
    }

    public void UnitKilled(GameObject gO, Unit.UnitType unitType)
    {
        bool removed = Units[unitType].Remove(gO);
            for (int j = 0; j < Units[unitType].Count; ++j)
                if (Units[unitType][j] == null) Units[unitType].RemoveAt(j--);
        OnUnitDead?.Invoke(unitType);
        Debug.Assert(removed, "This should not happen");
    }

    public bool IsCountUnits(){
        int cont = 0;
        foreach (Unit.UnitType uType in uni){
             cont = cont + Units[uType].Count;
        }
        return (cont <= 7);
    }

    public bool IsUnitsInBase(){
        int cont = 0;
        cont = Base.unitsOnBase;
        return (cont <= 2);
    }

    private static Unit.UnitType[] uni = {Unit.UnitType.Barbarian, Unit.UnitType.Catapult, Unit.UnitType.Knight, Unit.UnitType.Soldier};

    private void ResetUnits()
    {
        foreach (Transform unitTransform in transform)
        {
            Destroy(unitTransform.gameObject);
        }
    }

    private void ResetBuildings()
    {
        foreach (Transform buildTransform in Base.transform)
        {
            Destroy(buildTransform.gameObject);
        }
        buildingList.Clear();
        Base.Liberar();
    }

    private void UpdatePopUI(){
        if (PopText != null)
            PopText.text = "Pob: "+((int)population).ToString();
    }
    private void UpdateFoodUI(){
        if (FoodText != null)
            FoodText.text = "Alim: "+((int)food).ToString();
    }
    private void UpdateMatUI(){
        if (MatText != null)
            MatText.text = "Mat: "+((int)materials).ToString();
    }
    private void UpdatePopPerSecondUI(){
        if (PopPerSecondText != null)
            PopPerSecondText.text = "P/s: +" + popPerSec.ToString();
    }
    private void UpdateFoodPerSecondUI(){
        if (FoodPerSecondText != null)
            FoodPerSecondText.text = "A/s: +" + foodPerSec.ToString();
    }
    private void UpdateMatPerSecondUI(){
        if (MatPerSecondText != null)
            MatPerSecondText.text = "M/s: +" + matPerSec.ToString();
    }
    public void UpdateBuildingsUI(){
        int hou = 0, far = 0, saw = 0;
        foreach (Building buil in buildingList){
            if (buil.data.Equals("House")) hou++;
            if (buil.data.Equals("Farm")) far++;
            if (buil.data.Equals("Sawmill")) saw++;
        }
        if (HousesText != null)
            HousesText.text = "Hogares: " + hou;
        if (FarmsText != null)
            FarmsText.text = "Granjas: " + far;
        if (SawmillsText != null)
            SawmillsText.text = "Aserraderos: " + saw;
    }

    public void addResourcesPS(){
        int popMaxAux = popMaxInit;
        float foodAux = foodPerSecInit;
        float matAux = matPerSecInit;
        foreach (Building buil in buildingList){
            switch(buil.resourceType){
                case Building.Type.population:
                    popMaxAux += (int)buil.resourceAmount;
                    break;
                case Building.Type.food:
                    foodAux += buil.resourceAmount;
                    break;
                case Building.Type.materials:
                    matAux += buil.resourceAmount;
                    break;
            }
        }
        if (popMaxAux > popMax) popMax=popMaxAux;
        if (foodAux > foodPerSec) foodPerSec = foodAux;
        if (matAux > matPerSec) matPerSec = matAux;
        UpdateMatPerSecondUI();
        UpdateFoodPerSecondUI();
    }
    

}
