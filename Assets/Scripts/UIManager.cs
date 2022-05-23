using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public Button HouseButton, FarmButton, SawmillButton;
    public Button SoldierButton, KnightButton, CatapultButton, BarbarianButton;
    public Player player;
    private EventTrigger HouseET, FarmET, SawmillET;
    private EventTrigger SoldierET, KnightET, CatapultET, BarbarianET;
    private bool onCooldown = false;

    private void Awake() {
        player = GetComponent<Player>();
        HouseET = HouseButton.GetComponent<EventTrigger>();
        FarmET = FarmButton.GetComponent<EventTrigger>();
        SawmillET = SawmillButton.GetComponent<EventTrigger>();
        SoldierET = SoldierButton.GetComponent<EventTrigger>();
        KnightET = KnightButton.GetComponent<EventTrigger>();
        CatapultET = CatapultButton.GetComponent<EventTrigger>();
        BarbarianET = BarbarianButton.GetComponent<EventTrigger>();
    }
    private void OnEnable() {
        HouseButton.onClick.AddListener(() => player.build(new House()));
        FarmButton.onClick.AddListener(() => player.build(new Farm()));
        SawmillButton.onClick.AddListener(() => player.build(new Sawmill()));

        SoldierButton.onClick.AddListener(() => player.Spawn(Unit.UnitType.Soldier));
        SoldierButton.onClick.AddListener(() => StartCoroutine(Cooldown()));

        KnightButton.onClick.AddListener(() => player.Spawn(Unit.UnitType.Knight));
        KnightButton.onClick.AddListener(() => StartCoroutine(Cooldown()));

        CatapultButton.onClick.AddListener(() => player.Spawn(Unit.UnitType.Catapult));
        CatapultButton.onClick.AddListener(() => StartCoroutine(Cooldown()));
        
        BarbarianButton.onClick.AddListener(() => player.Spawn(Unit.UnitType.Barbarian));
        BarbarianButton.onClick.AddListener(() => StartCoroutine(Cooldown()));
    }
    private void OnDisable() {
        HouseButton.onClick.RemoveAllListeners();
        FarmButton.onClick.RemoveAllListeners();
        SawmillButton.onClick.RemoveAllListeners();
        SoldierButton.onClick.RemoveAllListeners();
        KnightButton.onClick.RemoveAllListeners();
        CatapultButton.onClick.RemoveAllListeners();
        BarbarianButton.onClick.RemoveAllListeners();
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Alpha5)){
            player.build(new House());
        }
        if(Input.GetKeyDown(KeyCode.Alpha6)){
            player.build(new Farm());
        }
        if(Input.GetKeyDown(KeyCode.Alpha7)){
            player.build(new Sawmill());
        }
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            player.Spawn(Unit.UnitType.Soldier);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            player.Spawn(Unit.UnitType.Knight);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)){
            player.Spawn(Unit.UnitType.Catapult);
        }
        if(Input.GetKeyDown(KeyCode.Alpha4)){
            player.Spawn(Unit.UnitType.Barbarian);
        }

        HouseButton.interactable = player.materials >= new House().cost; //Coste de las construcciones. A mas del mismo tipo, mayor coste? --> Hacer objeto
        HouseET.enabled = HouseButton.interactable;
        FarmButton.interactable = player.materials >= new Farm().cost;
        FarmET.enabled = FarmButton.interactable;
        SawmillButton.interactable = player.materials >= new Sawmill().cost;
        SawmillET.enabled = SawmillButton.interactable;

        SoldierButton.interactable = player.food >= Soldier.foodCost && player.population >= Soldier.popCost && !onCooldown && player.IsCountUnits() && player.IsUnitsInBase();
        SoldierET.enabled = SoldierButton.interactable;

        KnightButton.interactable = player.food >= Knight.foodCost && player.population >= Knight.popCost && !onCooldown && player.IsCountUnits() && player.IsUnitsInBase();
        KnightET.enabled = KnightButton.interactable;

        CatapultButton.interactable = player.food >= Catapult.foodCost && player.population >= Catapult.popCost && !onCooldown && player.IsCountUnits() && player.IsUnitsInBase();
        CatapultET.enabled = CatapultButton.interactable;

        BarbarianButton.interactable = player.food >= Barbarian.foodCost && player.population >= Barbarian.popCost && !onCooldown && player.IsCountUnits() && player.IsUnitsInBase();
        BarbarianET.enabled = BarbarianButton.interactable;
    }
    private WaitForSeconds WaitCooldown = new WaitForSeconds(1.5f);

    private IEnumerator Cooldown(){
        onCooldown = true;
        SoldierButton.interactable = false;
        KnightButton.interactable = false;
        CatapultButton.interactable = false;
        BarbarianButton.interactable = false;
        yield return WaitCooldown;
        onCooldown = false;
        StopCoroutine(Cooldown());
    }
}
