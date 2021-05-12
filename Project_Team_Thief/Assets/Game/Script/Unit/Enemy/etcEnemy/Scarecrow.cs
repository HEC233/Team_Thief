using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scarecrow : MonsterUnit
{
    public override void HandleHit(in Damage inputDamage)
    {
        var finalDamage = inputDamage.power * _unitData.reduceHit;
        _hp -= finalDamage;
        GameManager.instance?.uiMng.ShowDamageText(inputDamage.hitPosition,
            (int)finalDamage, 0 < (inputDamage.hitPosition - transform.position).x, false);

        if (_hp <= 0)
        {
            _hp = _unitData.hp;
            hitEvent.Invoke();
        }
        hitEvent.Invoke();
    }
}
