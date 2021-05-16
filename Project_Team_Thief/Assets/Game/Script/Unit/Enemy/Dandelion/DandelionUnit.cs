using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DandelionUnit : MonsterUnit
{
    public Transform attackPosition;

    public override void Attack()
    {
        GameManager.instance?.FX.Play("DandelionAttack", attackPosition.position);
    }

    public override void HandleHit(in Damage inputDamage)
    {
        base.HandleHit(inputDamage);

        if (_hp <= 0)
        {
            dieEvent.Invoke();
        }
        else
        {
            hitEvent.Invoke();
        }
    }
}
