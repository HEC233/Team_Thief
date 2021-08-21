using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlessingPenaltyLivingHardData", menuName = "ScriptableObject/BlessingPenalty/BlessingPenaltyLivingHardData")]
public class BlessingPenaltyLivingHardData : BlessingPenaltyDataBase
{
    private PlayerUnit _playerUnit;

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
        durationString = durationString.Insert(4, _duration.ToString());
    }
}
