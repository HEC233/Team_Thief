using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlessingPenaltyHpDegradationData", menuName = "ScriptableObject/BlessingPenalty/BlessingPenaltyHpDegradationData")]
public class BlessingPenaltyHpDegradationData : BlessingPenaltyDataBase
{
    [SerializeField]
    private int _maxHpDecreasedAmount;
    public int MAXHpDecreasedAmount => _maxHpDecreasedAmount;

    
    public override void ActivePenalty(Unit unit)
    {
        SetContentString();
        var playerUnit = unit as PlayerUnit;
        
        if(playerUnit == null)
            return;

        playerUnit.MaxHpDegradation(MAXHpDecreasedAmount);
    }

    protected override void SetContentString()
    {
        contentString = contentString.Insert(7, _maxHpDecreasedAmount.ToString());
    }
}
