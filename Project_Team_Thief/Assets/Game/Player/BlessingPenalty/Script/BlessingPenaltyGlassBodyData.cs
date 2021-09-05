using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlessingPenaltyGlassBodyData", menuName = "ScriptableObject/BlessingPenalty/BlessingPenaltyGlassBodyData")]

public class BlessingPenaltyGlassBodyData : BlessingPenaltyDataBase
{
    private PlayerUnit _playerUnit;

    [SerializeField]
    private float _addDamagePerAmount;
    public float AddDamagePerAmount => _useAddDamagePerAmount;
    private float _useAddDamagePerAmount;
    
    [SerializeField]
    private int _duration;
    public int Duration => _duration;

    public override void ActivePenalty(Unit unit)
    {
        _playerUnit = unit as PlayerUnit;

        if (_playerUnit == null)
        {
            return;
        }
        
        _playerUnit.ChangeDecreaseHp(_addDamagePerAmount);
        _playerUnit.StartCoroutine(PenaltyCoroutine());

    }
    
    private IEnumerator PenaltyCoroutine()
    {
        int penaltyDurationMapCount = _playerUnit.MapCount + _duration;
        float originalDecreaseHp = _playerUnit.DecreaseHp;
        while (penaltyDurationMapCount > _playerUnit.MapCount)
        {
            yield return new WaitForFixedUpdate();
        }
        
        _playerUnit.ChangeDecreaseHp(originalDecreaseHp);
    }

    public override void SetContentString()
    {
        durationString = originalDurationString;
        durationString = durationString.Insert(2, _duration.ToString());
        contentString = originalContentString;
        contentString = contentString.Insert(7, AddDamagePerAmount.ToString());
    }

    public override void SetAddPenalty(float zeroTimer)
    {
        _useAddDamagePerAmount = _addDamagePerAmount;
        
        if (zeroTimer >= 10)
        {
            _useAddDamagePerAmount += 0.2f;
        }
        else if (zeroTimer >= 20)
        {
            _useAddDamagePerAmount += 0.4f;
        }
        else if (zeroTimer >= 30)
        {
            _useAddDamagePerAmount += 0.6f;
        }
    }
}
