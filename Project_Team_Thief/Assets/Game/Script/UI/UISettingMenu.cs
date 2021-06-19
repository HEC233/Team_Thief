using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettingMenu : MonoBehaviour
{
    private RectTransform _rect;
    [SerializeField]
    private Toggle FPS;
    [SerializeField]
    private Toggle DeveloperConsole;
    [SerializeField]
    private Toggle CommandAssist;
    private GameSettingData data;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void Toggle(bool value)
    {
        if (!this.gameObject.activeSelf && !value)
            return;
        this.gameObject.SetActive(value);
        if(value)
        {
            data = GameManager.instance.SettingData;
            FPS.isOn = data.bShowFPS;
            DeveloperConsole.isOn = data.bUseDeveloperConsole;
            CommandAssist.isOn = data.bDontUseCommandAssist;
            GameManager.instance?.timeMng.StopTime();
        }
        else
        {
            GameManager.instance?.timeMng.ResumeTime();
        }
    }
    
    public void CloseSettingMenu()
    {
        GameManager.instance.ApplySetting(data);
        GameManager.instance.EscapeButton();
        WwiseSoundManager.instance.PlayEventSound("Click");
    }

    public void ToggleButtonFPS(bool value)
    {
        data.bShowFPS = value;
        WwiseSoundManager.instance.PlayEventSound("Click");
    }

    public void ToggleButtonDeveloperConsole(bool value)
    {
        data.bUseDeveloperConsole = value;
        WwiseSoundManager.instance.PlayEventSound("Click");
    }

    public void ToggleButtonCommandAssist(bool value)
    {
        data.bDontUseCommandAssist = value;
        WwiseSoundManager.instance.PlayEventSound("Click");
    }
}
