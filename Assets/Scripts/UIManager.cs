using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public Button HouseButton, FarmButton, SawmillButton;
    public Button SoldierButton;
    public Player player;
    private EventTrigger HouseET, FarmET, SawmillET;
    private EventTrigger SoldierET;

    private void Awake() {
        player = GetComponent<Player>();
        HouseET = HouseButton.GetComponent<EventTrigger>();
        FarmET = FarmButton.GetComponent<EventTrigger>();
        SawmillET = SawmillButton.GetComponent<EventTrigger>();
        SoldierET = SoldierButton.GetComponent<EventTrigger>();
    }
    private void OnEnable() {
        HouseButton.onClick.AddListener(() => player.build(new House()));
        FarmButton.onClick.AddListener(() => player.build(new Farm()));
        SawmillButton.onClick.AddListener(() => player.build(new Sawmill()));
        SoldierButton.onClick.AddListener(() => player.Spawn(Unit.UnitType.Soldier));
    }
    private void OnDisable() {
        HouseButton.onClick.RemoveAllListeners();
        FarmButton.onClick.RemoveAllListeners();
        SawmillButton.onClick.RemoveAllListeners();
        SoldierButton.onClick.RemoveAllListeners();
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            player.build(new House());
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            player.build(new Farm());
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)){
            player.build(new Sawmill());
        }
        if(Input.GetKeyDown(KeyCode.Alpha4)){
            player.Spawn(Unit.UnitType.Soldier);
        }

        HouseButton.interactable = player.materials >= House.cost; //Coste de las construcciones. A mas del mismo tipo, mayor coste? --> Hacer objeto
        HouseET.enabled = HouseButton.interactable;
        FarmButton.interactable = player.materials >= Farm.cost;
        FarmET.enabled = FarmButton.interactable;
        SawmillButton.interactable = player.materials >= Sawmill.cost;
        SawmillET.enabled = SawmillButton.interactable;
        SoldierButton.interactable = player.food >= Soldier.foodCost && player.population >= Soldier.popCost;
        SoldierET.enabled = SoldierButton.interactable;
    }
}
