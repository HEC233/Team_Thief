using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlessingPenaltyHpDegradationData", menuName = "ScriptableObject/BlessingPenalty/BlessingPenaltyHpDegradationData")]
public class BlessingPenaltyHpDegradationData : BlessingPenaltyDataBase
{
    [SerializeField]
    private int _maxHpDecreasedAmount;
    public int MAXHpDecreasedAmount => _useHpDecreaseAmount;
    private int _useHpDecreaseAmount;

    
    public override void ActivePenalty(Unit unit)
    {
        var playerUnit = unit as PlayerUnit;
        
        if(playerUnit == null)
            return;

        playerUnit.MaxHpDegradation(_useHpDecreaseAmount);
    }

    public override void SetContentString()
    {
        contentString = originalContentString;
        contentString = contentString.Insert(7, MAXHpDecreasedAmount.ToString());
    }

    public override void SetAddPenalty(float zeroTimer)
    {
        _useHpDecreaseAmount = _maxHpDecreasedAmount;
        
        if (zeroTimer >= 10)
        {
            _useHpDecreaseAmount += 10;
        }
        else if (zeroTimer >= 20)
        {
            _useHpDecreaseAmount += 20;
        }
        else if (zeroTimer >= 30)
        {
            _useHpDecreaseAmount += 30;
        }
    }
}
