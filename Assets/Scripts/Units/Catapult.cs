using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : Unit
{
 public int DamageUnits = 20;
 public int DamageTower = 10;

 private int count = 0;

 public static int popCost = 10;
 public static int foodCost = 20;
    protected override void Attack(GameObject enemy)
    {
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
