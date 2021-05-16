using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scarecrow : MonsterUnit
{
    public override void HandleHit(in Damage inputDamage)
    {
        base.HandleHit(inputDamage);

        if (_hp <= 0)
        {
            _hp = _unitData.hp;
        }
        hitEvent.Invoke();
    }
}
