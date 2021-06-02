using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAlterUnit : MonsterUnit
{
    private bool bInvincibility = false;

    public void SetInvincibility(bool value)
    {
        bInvincibility = value;
    }

    private void Start()
    {
        _hp = _unitData.hp;

        contactFilter.useTriggers = true;
        contactFilter.useLayerMask = true;
        contactFilter.SetLayerMask(hitBoxLayer);

        _damage.knockBack = _unitData.knockback;
    }

    public void ResetHP()
    {
        _hp = _unitData.hp;
    }

    public override void HandleHit(in Damage inputDamage)
    {
        if (bInvincibility)
            return;

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

    public override void HandleDeath()
    {

    }
}
