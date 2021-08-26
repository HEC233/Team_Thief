using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UISkillSlot : MonoBehaviour
{
    private SkillSlotManager.SkillSlot _skillData;
    [SerializeField]
    private UISkillCommandBox _skillCommandBox;
    [SerializeField]
    private UISkillCooltimeBox _skillCooltimeBox;

    public void Regist(SkillSlotManager.SkillSlot skillSlot)
    {
        Assert.IsNotNull(skillSlot);
        _skillData = skillSlot;
    }

    public void UpdateData()
    {
        if (_skillData == null || _skillData.SkillDataBase == null)
        {
            return;
        }
        _skillCommandBox.InitCommandInfo(_skillData.CommandString);
        _skillCooltimeBox.SetSkillIcon(_skillData.SkillDataBase.Icon);
    }

    public void FixedUpdate()
    {
        if (_skillData == null)
        {
            return;
        }
        if (_skillData.SkillDataBase == null)
        {
            _skillCooltimeBox.SetSkillIcon(null);
            return;
        }
        if(_skillData.IsSeal)
        {

        }
        
        int count = 0;
        int length = _skillData.CommandString.Length;
        for (int i = 0; i < length && i < _skillData.CommandList.Count; i++)
        {
            if (_skillData.CommandList[i] == _skillData.CommandString[i])
            {
                count++;
            }
            else
            {
                count = 0;
                break;
            }
        }

        _skillCommandBox.CommandUpdate(count, length);
        float cooltimeRatio = Mathf.Clamp01(_skillData.SkillSlotCurCoolTime / _skillData.SkillSlotCoolTime);
        _skillCooltimeBox.CustomUpdate(cooltimeRatio);
    }
}
