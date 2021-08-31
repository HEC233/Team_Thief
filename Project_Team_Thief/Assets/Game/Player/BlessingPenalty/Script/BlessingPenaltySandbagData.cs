using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlessingPenaltySandbagData", menuName = "ScriptableObject/BlessingPenalty/BlessingPenaltySandbagData")]
public class BlessingPenaltySandbagData : BlessingPenaltyDataBase
{
    private PlayerUnit _playerUnit;
    
    [SerializeField] 
    private float _moveSpeedDecreasedAmount;
    public float MoveSpeedDecreasedAmount => _moveSpeedDecreasedAmount;

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
        
        _playerUnit.ChangeMoveSpeed(_moveSpeedDecreasedAmount);
        _playerUnit.StartCoroutine(PenaltyCoroutine());
    }

    private IEnumerator PenaltyCoroutine()
    {
        int penaltyDurationMapCount = _playerUnit.MapCount + _duration;
        while (penaltyDurationMapCount > _playerUnit.MapCount)
        {
            yield return new WaitForFixedUpdate();
        }
        
        _playerUnit.ChangeMoveSpeed(1 / _moveSpeedDecreasedAmount);
    }

    public override void SetContentString()
    {
        contentString = contentString.Insert(12, MoveSpeedDecreasedAmount.ToString());
        durationString = durationString.Insert(4, _duration.ToString());
    }
}
