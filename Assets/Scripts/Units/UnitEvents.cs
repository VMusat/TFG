using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEvents : MonoBehaviour
{
    private Unit unit;

    private void Awake()
    {
        unit = GetComponentInParent<Unit>();
    }

    public void OnAnimationAttackEnded()
    {
        if (!unit.Dead)
            unit.OnAnimationAttackEnded();
    }
}
