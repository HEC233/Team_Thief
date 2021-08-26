using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UISkillInfo : MonoBehaviour
{
    private const int _skillSlotSize = 4;
    [SerializeField]
    private UISkillSlot[] _SkillSlotUI = new UISkillSlot[_skillSlotSize];

    public void RegistSkillData(int index, SkillSlotManager.SkillSlot skillSlot)
    {
        if(index >= 0 && index < _skillSlotSize)
        {
            _SkillSlotUI[index].Regist(skillSlot);
        }
    }
    public void UpdateSkillData(int index)
    {
        if (index >= 0 && index < _skillSlotSize)
        {
            _SkillSlotUI[index].UpdateData();
        }
    }
}
