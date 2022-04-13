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
    public GameObject Base;
    List<Building> buildingList = new List<Building>();
    //Variables Iniciales que cambian con las construcciones
    int popMaxInit = 100;
    float foodPerSecInit = 1;
    float matPerSecInit = 1;
    //
    public Vector3 TowerLocationLocal;
    public bool IsPlayer;
    public Tower tower;
    public Dictionary<Unit.UnitType, List<GameObject>> Units;

    void Start()
    {
        Units = new Dictionary<Unit.UnitType, List<GameObject>>();
        Units.Add(Unit.UnitType.Soldier, new List<GameObject>());
        Units.Add(Unit.UnitType.Barbarian, new List<GameObject>());
        Units.Add(Unit.UnitType.Knight, new List<GameObject>());
        Units.Add(Unit.UnitType.Catapult, new List<GameObject>());
        population = 50;
        popMax = popMaxInit;
        popPerSec = 1;
        food = 50;
        foodPerSec = foodPerSecInit;
        materials = 50;
        matPerSec = matPerSecInit;
        popFoodConst = 150;
        UpdateFoodUI();
        UpdateFoodPerSecondUI();
        UpdateMatUI();
        UpdateMatPerSecondUI();
        UpdatePopUI();
        UpdatePopPerSecondUI();
        UpdateBuildingsUI();
        StopAllCoroutines();
        StartCoroutine(ResourceLoop());
        ResetUnits();
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
            if (population < 0){
                population = 0;
                StopCoroutine(ResourceLoop());
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

    public void build(Building buil){
        if (materials >= buil.getCost()){
            buildingList.Add(buil);
            string data = buil.data;
            GameObject b = Instantiate(Resources.Load($"Prefabs/Buildings/{data}") as GameObject, new Vector3(0,0,0), Quaternion.identity);
            b.transform.SetParent(Base.transform, false);
            materials = materials - buil.getCost();
            UpdateMatUI();
            UpdateBuildingsUI();
        }  
    }

    public bool Spawn(Unit unit){
        int unitFCost = unit.getFoodCost();
        int unitPCost = unit.getPopCost();
        if(unitFCost <= food && unitPCost <= population){
            GameObject prefab = Resources.Load($"Prefabs/Units/{unit.data}") as GameObject;
            if (prefab != null){
                GameObject unitGO = Instantiate(prefab, transform.position + TowerLocationLocal, Quaternion.identity, transform);
                unitGO.GetComponentInChildren<Unit>().IsPlayer = IsPlayer;
                Units[unit.Type].Add(unitGO);
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

    private void ResetUnits()
    {
        foreach (Transform unitTransform in transform)
        {
            Destroy(unitTransform.gameObject);
        }
    }

    private void UpdatePopUI(){
        if (PopText != null)
            PopText.text = ((int)population).ToString();
    }
    private void UpdateFoodUI(){
        if (FoodText != null)
            FoodText.text = ((int)food).ToString();
    }
    private void UpdateMatUI(){
        if (MatText != null)
            MatText.text = ((int)materials).ToString();
    }
    private void UpdatePopPerSecondUI(){
        if (PopPerSecondText != null)
            PopPerSecondText.text = "+" + popPerSec.ToString();
    }
    private void UpdateFoodPerSecondUI(){
        if (FoodPerSecondText != null)
            FoodPerSecondText.text = "+" + foodPerSec.ToString();
    }
    private void UpdateMatPerSecondUI(){
        if (MatPerSecondText != null)
            MatPerSecondText.text = "+" + matPerSec.ToString();
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
