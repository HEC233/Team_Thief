using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlessingPenaltyLivingHardData", menuName = "ScriptableObject/BlessingPenalty/BlessingPenaltyLivingHardData")]
public class BlessingPenaltyLivingHardData : BlessingPenaltyDataBase
{
    private PlayerUnit _playerUnit;

    [SerializeField] 
    private int _duration;
    public int Duration => _useDuration;
    private int _useDuration;
    
    public override void ActivePenalty(Unit unit)
    {
        _playerUnit = unit as PlayerUnit;

        if (_playerUnit == null)
        {
            return;
        }
        
        _playerUnit.ApplyLivingHard();
        _playerUnit.StartCoroutine(PenaltyCoroutine());
    }
    
    private IEnumerator PenaltyCoroutine()
    {
        int penaltyDurationMapCount = _playerUnit.MapCount + _duration;
        while (penaltyDurationMapCount > _playerUnit.MapCount)
        {
            yield return new WaitForFixedUpdate();
        }
        
        _playerUnit.TurnOffLivingHard();
    }

    public override void SetContentString()
    {
        durationString = originalContentString;
        durationString = durationString.Insert(3, _duration.ToString());
    }

    public override void SetAddPenalty(float zeroTimer)
    {
        _useDuration = _duration;
        
        if (zeroTimer <= 10)
        {
            _useDuration += 1;
        }
        else if (zeroTimer <= 20)
        {
            _useDuration += 2;
        }
        else if (zeroTimer >= 30)
        {
            _useDuration += 3;
        }
    }
}
