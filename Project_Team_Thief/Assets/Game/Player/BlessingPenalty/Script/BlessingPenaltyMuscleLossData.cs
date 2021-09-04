using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlessingPenaltyMuscleLossData", menuName = "ScriptableObject/BlessingPenalty/BlessingPenaltyMuscleLossData")]
public class BlessingPenaltyMuscleLossData : BlessingPenaltyDataBase
{
    private PlayerUnit _playerUnit;

    [SerializeField] 
    private float _damageDecreasedAmount;
    public float DamageDecreasedAmount => _damageDecreasedAmount;

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
        
        _playerUnit.ChangeDamage(_damageDecreasedAmount);
        _playerUnit.StartCoroutine(PenaltyCoroutine());
    }
    
    private IEnumerator PenaltyCoroutine()
    {
        int penaltyDurationMapCount = _playerUnit.MapCount + _duration;
        while (penaltyDurationMapCount > _playerUnit.MapCount)
        {
            yield return new WaitForFixedUpdate();
        }
        
        _playerUnit.ChangeDamage(1 / _damageDecreasedAmount);
    }

    public override void SetContentString()
    {
        contentString = originalContentString;
        contentString = contentString.Insert(9, _damageDecreasedAmount.ToString());
        
        durationString = originalDurationString;
        durationString = durationString.Insert(4, _duration.ToString());
    }
}
