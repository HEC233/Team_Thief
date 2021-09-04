using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlessingPenaltySandbagData", menuName = "ScriptableObject/BlessingPenalty/BlessingPenaltySandbagData")]
public class BlessingPenaltySandbagData : BlessingPenaltyDataBase
{
    private PlayerUnit _playerUnit;
    
    [SerializeField] 
    private float _moveSpeedDecreasedAmount;
    public float MoveSpeedDecreasedAmount => _useMoveSpeedDecreasedAmount;
    private float _useMoveSpeedDecreasedAmount;
    
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
        contentString = originalContentString;
        durationString = originalDurationString;
        contentString = contentString.Insert(11, MoveSpeedDecreasedAmount.ToString());
        durationString = durationString.Insert(3, _duration.ToString());
    }

    public override void SetAddPenalty(float zeroTimer)
    {
        _useMoveSpeedDecreasedAmount = _moveSpeedDecreasedAmount;
        
        if (zeroTimer <= 10)
        {
            _useMoveSpeedDecreasedAmount += 0.1f;
        }
        else if (zeroTimer <= 20)
        {
            _useMoveSpeedDecreasedAmount += 0.2f;
        }
        else if (zeroTimer >= 30)
        {
            _useMoveSpeedDecreasedAmount += 0.3f;
        }
    }
}
