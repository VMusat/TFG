using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Unit
{
 public int DamageUnits = 20;
 public int DamageTower = 5;

 private int count = 0;

 public static int popCost = 5;
 public static int foodCost = 10;

 public Soldier(){
     //popCost = 5;
     //foodCost = 10;
     this.data = "Soldier";
     this.Type = UnitType.Soldier;
 }

    protected override void Attack(GameObject enemy)
    {   
        //Debug.Log("Entre");
        if (enemy != null){
            Unit unit = enemy.GetComponentInParent<Unit>();
            if (unit != null)
            {
                if(count == 4){
                    DamageUnits = Random.Range(19,21);
                    count = -1;
                } 
                unit.AddHealth(-DamageUnits);
                count += 1;
                return;
            }
            Tower tower = enemy.GetComponentInParent<Tower>();
            if (tower != null)
            {
                tower.AddHealth(-DamageTower);
                return;
            }
        }
    }
}
