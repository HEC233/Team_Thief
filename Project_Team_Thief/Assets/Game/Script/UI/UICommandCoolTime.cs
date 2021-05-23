using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICommandCoolTime : MonoBehaviour
{
    public Image coolTimeImage;
    public Image skillIcon;
    private CommandManager.CommandCtrl _data;

    public void SetSkillIcon(Sprite sprite)
    {
        skillIcon.sprite = sprite;
    }

    public void SetData(CommandManager.CommandCtrl data)
    {
        _data = data;
    }

    private void Update()
    {
        coolTimeImage.fillAmount = 1 - Mathf.Clamp01(_data.CommandData.coolTime / _data.CommandData.maxCoolTIme);
    }
}
