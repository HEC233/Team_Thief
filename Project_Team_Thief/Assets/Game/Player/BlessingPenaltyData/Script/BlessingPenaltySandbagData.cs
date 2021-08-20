using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlessingPenaltySandbagData", menuName = "ScriptableObject/BlessingPenalty/BlessingPenaltySandbagData")]
public class BlessingPenaltySandbagData : BlessingPenaltyDataBase
{
    [SerializeField] 
    private float _moveSpeedDecreasedAmount;

    public float MoveSpeedDecreasedAmount => _moveSpeedDecreasedAmount;

    public override void ActivePenalty(Unit unit)
    {
        SetContentString();

        var playerUnit = unit as PlayerUnit;

        if (playerUnit == null)
        {
            return;
        }
        
        
    }

    protected override void SetContentString()
    {
        contentString = contentString.Insert(12, MoveSpeedDecreasedAmount.ToString());
    }
}
