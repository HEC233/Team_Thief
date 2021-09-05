using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlessingPenaltyShockData", menuName = "ScriptableObject/BlessingPenalty/BlessingPenaltyShockData")]
public class BlessingPenaltyShockData : BlessingPenaltyDataBase
{
    private PlayerUnit _playerUnit;

    [SerializeField] 
    private float _shockDamage;
    public float ShockDamage => _useShockDamage;
    private float _useShockDamage;

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
        contentString = originalContentString;
        contentString = contentString.Insert(0, ShockDamage.ToString());

    }

    public override void SetAddPenalty(float zeroTimer)
    {
        _useShockDamage = _shockDamage;
        
        if (zeroTimer >= 10)
        {
            _useShockDamage += 5;
        }
        else if (zeroTimer >= 20)
        {
            _useShockDamage += 10;
        }
        else if (zeroTimer >= 30)
        {
            _useShockDamage += 15;
        }
        
        Debug.Log(_useShockDamage);
    }
}
