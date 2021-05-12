using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SeraphimUnit : MonsterUnit
{
    public void BackStep(Vector2 ImpulseForce)
    {
        _rigid.AddForce(ImpulseForce, ForceMode2D.Impulse);
        skipGroundCheckTime = 0.1f;
        isOnGround = false;
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
            if (GameManager.instance.shadow)
            {
                GameManager.instance.shadow.Burst(inputDamage.hitPosition, 10, 10, 5, true);
            }
            hitEvent.Invoke();
        }
    }
}
