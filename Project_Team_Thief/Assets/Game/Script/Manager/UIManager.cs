using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public UIPlayerInfo uiPlayerInfo;
    public UIMainMenu uiMainMenu;
    public UISettingMenu uiSettingMenu;
    public UIPauseMenu uiPauseMenu;
    public UILoadingAnimation uiLoading;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void ToggleUI(GameManager.GameStateEnum gameState)
    {
        switch (gameState)
        {
            case GameManager.GameStateEnum.MainMenu:
                uiPlayerInfo.Toggle(false);
                uiMainMenu.Toggle(true);
                uiSettingMenu.Toggle(false);
                uiPauseMenu.Toggle(false);
                uiLoading.Toggle(false);
                break;
            case GameManager.GameStateEnum.InGame:
                uiPlayerInfo.Toggle(true);
                uiMainMenu.Toggle(false);
                uiSettingMenu.Toggle(false);
                uiPauseMenu.Toggle(false);
                uiLoading.Toggle(false);
                break;
            case GameManager.GameStateEnum.Pause:
                uiPlayerInfo.Toggle(true);
                uiMainMenu.Toggle(false);
                uiSettingMenu.Toggle(false);
                uiPauseMenu.Toggle(true);
                uiLoading.Toggle(false);
                break;
        }
    }

    public void ShowLoading()
    {
        uiPlayerInfo.Toggle(false);
        uiMainMenu.Toggle(false);
        uiSettingMenu.Toggle(false);
        uiPauseMenu.Toggle(false);
        uiLoading.Toggle(true);
    }
}
