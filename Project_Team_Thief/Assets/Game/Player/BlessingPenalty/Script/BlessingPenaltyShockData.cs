using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlessingPenaltyShockData", menuName = "ScriptableObject/BlessingPenalty/BlessingPenaltyShockData")]
public class BlessingPenaltyShockData : BlessingPenaltyDataBase
{
    private PlayerUnit _playerUnit;

    [SerializeField] 
    private float _shockDamage;
    public float ShockDamage => _shockDamage;

    public override void ActivePenalty(Unit unit)
    {
        _playerUnit = unit as PlayerUnit;

        if (_playerUnit == null)
        {
            return;
        }

        Damage damage = new Damage();
        damage.power = _shockDamage;
        damage.knockBack = Vector2.zero;
        
        _playerUnit.HandleHit(damage);
    }

    public override void SetContentString()
    {
        contentString = contentString.Insert(0, _shockDamage.ToString());

    }
}
