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
    private GameSettingData data;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void Toggle(bool value)
    {
        this.gameObject.SetActive(value);
        if(value)
        {
            data = GameManager.instance.SettingData;
            FPS.isOn = data.bShowFPS;
            DeveloperConsole.isOn = data.bUseDeveloperConsole;
        }
    }
    
    public void CloseSettingMenu()
    {
        GameManager.instance.ApplySetting(data);
        GameManager.instance.EscapeButton();
    }

    public void ToggleButtonFPS(bool value)
    {
        data.bShowFPS = value;
    }

    public void ToggleButtonDeveloperConsole(bool value)
    {
        data.bUseDeveloperConsole = value;
    }
}
