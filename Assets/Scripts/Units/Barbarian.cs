using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barbarian : Unit
{
 public int DamageUnits = 50;
 public int DamageTower = 5;

 private int count = 0;

 public static int popCost = 5;
 public static int foodCost = 20;
    protected override void Attack(GameObject enemy)
    {
        this.DamageUnits = 10+(int)this.Health/3;
        //Debug.Log("Entre");
        if (enemy != null){
            Unit unit = enemy.GetComponentInParent<Unit>();
            if (unit != null)
            {
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
