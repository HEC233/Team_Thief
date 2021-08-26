using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkillBaldoAttackCtrl : AttackBase
{
    public event UnityAction OnHitEvent = null;
    
    public override void Progress()
    {
        base.Progress();

        if (_isEnter)
        {
            OnHitEvent?.Invoke();   
            Debug.Log("hit");
        }
    }

    public override void AttackEnd()
    {
        base.AttackEnd();

        OnEnemyHitEvent = null;
    }
}
