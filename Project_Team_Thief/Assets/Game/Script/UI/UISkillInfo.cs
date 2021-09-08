using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UISkillInfo : MonoBehaviour
{
    private const int _skillSlotSize = 4;
    [SerializeField]
    private UISkillSlot[] _skillSlotUI = new UISkillSlot[_skillSlotSize];
    private int _curSelectIndex = -1;

    public void RegistSkillData(int index, SkillSlotManager.SkillSlot skillSlot)
    {
        if (index >= 0 && index < _skillSlotSize)
        {
            _skillSlotUI[index].Regist(skillSlot);
        }
    }
    public void UpdateSkillData(int index)
    {
        if (index >= 0 && index < _skillSlotSize)
        {
            _skillSlotUI[index].UpdateData();
        }
    }

    public void SkillIconInteractable(bool value)
    {
        _curSelectIndex = -1;
        foreach (var slot in _skillSlotUI)
        {
            slot.SetButtonInteractable(value);
        }
    }

    public void SkillIconSelect(int index)
    {
        Assert.IsTrue(index >= 0 && index < _skillSlotSize);

        _curSelectIndex = index;
    }

    public bool IsSelected
    {
        get { return _curSelectIndex != -1; }
    }
    public int CurID
    {
        get { return _curSelectIndex; }
    }
    public void UnSelect()
    {
        _curSelectIndex = -1;
    }
}
