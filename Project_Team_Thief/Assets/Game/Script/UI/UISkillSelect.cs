using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class UISkillSelect : MonoBehaviour
{
    [SerializeField]
    private UISkillSelectInfo[] _skillInfos = new UISkillSelectInfo[3];

    private SkillDataBase[] _skilldatas;
    private int _curIndex = -1;

    public bool IsSelected
    {
        get { return _curIndex != -1; }
    }

    public int CurID
    {
        get { return IsSelected ? _skilldatas[_curIndex].ID : -1; }
    }

    private void Start()
    {
        ShowSkillSelect(false);
    }

    public void SetSkillData(SkillDataBase[] data)
    {
        _skilldatas = data;
        _skillInfos[0].SetInfo(data[0].Icon, data[0].Name, data[0].Description);
        _skillInfos[1].SetInfo(data[1].Icon, data[1].Name, data[1].Description);
        _skillInfos[2].SetInfo(data[2].Icon, data[2].Name, data[2].Description);
    }

    public void ShowSkillSelect(bool value)
    {
        _skillInfos[0].Show(value);
        _skillInfos[1].Show(value);
        _skillInfos[2].Show(value);

        _skillInfos[0].SetHightlight(false);
        _skillInfos[1].SetHightlight(false);
        _skillInfos[2].SetHightlight(false);
        _curIndex = -1;
    }

    public void ButtonSelect(int index)
    {
        Assert.IsTrue(index >= 0 && index < 3);

        ShowSkillSelect(true);

        _skillInfos[index].SetHightlight(true);
        _curIndex = index;
    }

    public void UnSelect()
    {
        ShowSkillSelect(true);
    }
}
