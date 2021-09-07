using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlessingPenaltySealData", menuName = "ScriptableObject/BlessingPenalty/BlessingPenaltySealData")]
public class BlessingPenaltySealData : BlessingPenaltyDataBase
{
    private PlayerUnit _playerUnit;

    [SerializeField] 
    private int _duration;
    public int Duration => _useDuration;
    private int _useDuration;

    private int _randSkillSlotIndex;

    public override void ActivePenalty(Unit unit)
    {
        _randSkillSlotIndex = UnityEngine.Random.Range(0, GameManager.instance.SkillSlotMng.SkillSlots.Count);
        GameManager.instance.SkillSlotMng.SealSkillSlot();

        _playerUnit = unit as PlayerUnit;

        if (_playerUnit == null)
        {
            return;
        }

        _playerUnit.StartCoroutine(PenaltyCoroutine());
    }
    
    private IEnumerator PenaltyCoroutine()
    {
        int penaltyDurationMapCount = _playerUnit.MapCount + _duration;
        while (penaltyDurationMapCount > _playerUnit.MapCount)
        {
            yield return new WaitForFixedUpdate();
        }
        
        GameManager.instance.SkillSlotMng.UnSealingSkillSlot(_randSkillSlotIndex);
    }

    public override void SetContentString()
    {
        durationString = originalDurationString;
        durationString = durationString.Insert(4, Duration.ToString());
    }

    public override void SetAddPenalty(float zeroTimer)
    {
        _useDuration = _duration;
        
        if (zeroTimer >= 10)
        {
            _useDuration += 1;
        }
        else if (zeroTimer >= 20)
        {
            _useDuration += 2;
        }
        else if (zeroTimer >= 30)
        {
            _useDuration += 3;
        }
        
        Debug.Log(_useDuration);
    }
}
