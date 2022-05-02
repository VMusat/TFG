using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Unit
{
 public int DamageUnits = 40;
 public int DamageTower = 6;

 private int count = 0;

 public static int popCost = 5;
 public static int foodCost = 40;
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
