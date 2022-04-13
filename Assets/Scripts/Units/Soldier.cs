using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Unit
{
 public int DamageUnits = 20;
 public int DamageTower = 5;

 public Soldier(){
     popCost = 5;
     foodCost = 10;
     this.data = "Soldier";
     this.Type = UnitType.Soldier;
 }

    protected override void Attack(GameObject enemy)
    {
        Unit unit = enemy.GetComponentInParent<Unit>();
        if (unit != null)
        {
            unit.AddHealth(-DamageUnits);
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
