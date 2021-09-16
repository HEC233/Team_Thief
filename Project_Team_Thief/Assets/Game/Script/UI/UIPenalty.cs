using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPenalty : MonoBehaviour
{
    [SerializeField]
    private UIPenaltyInfo[] _uiPenalties = new UIPenaltyInfo[3];

    private BlessingPenaltyDataBase[] _datas;

    private void Start()
    {
        ShowPenalties(false);
    }

    public void SetPenalties(BlessingPenaltyDataBase[] penalties)
    {
        if(penalties.Length < 3)
        {
            return;
        }

        _datas = penalties;

        _uiPenalties[0].SetInfo(Addressable.instance.GetSprite(
            penalties[0].SpriteImageName), penalties[0].Name, penalties[0].ContentString, penalties[0].DurationString);
        _uiPenalties[1].SetInfo(Addressable.instance.GetSprite(
            penalties[1].SpriteImageName), penalties[1].Name, penalties[1].ContentString, penalties[1].DurationString);
        _uiPenalties[2].SetInfo(Addressable.instance.GetSprite(
            penalties[2].SpriteImageName), penalties[2].Name, penalties[2].ContentString, penalties[2].DurationString);
    }

    public void ShowPenalties(bool value)
    {
        this.gameObject.SetActive(value);
    }

    public void ButtonDown(int index)
    {
        ShowPenalties(false);
        GameManager.instance.UIMng.ActivePenalty(_datas[index].ID);
    }
}
