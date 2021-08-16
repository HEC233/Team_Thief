using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightWarriorUnit : MonsterUnit
{
    public override void HandleHit(in Damage inputDamage)
    {
        base.HandleHit(inputDamage);

        if (_hp <= 0)
        {
            dieEvent.Invoke();
        }
        else
        {
            if (GameManager.instance.ShadowParticle)
            {
                GameManager.instance.ShadowParticle.Burst(inputDamage.hitPosition, 10, 10, 5, true);
            }
            hitEvent.Invoke();
        }
    }
}
