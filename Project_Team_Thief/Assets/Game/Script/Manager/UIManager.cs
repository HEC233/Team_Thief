using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PS.Util.DeveloperConsole;

public class UIManager : MonoBehaviour
{
    public UIPlayerInfo uiPlayerInfo;
    public UIMainMenu uiMainMenu;
    public UISettingMenu uiSettingMenu;
    public UIPauseMenu uiPauseMenu;
    public UILoadingAnimation uiLoading;
    public UIDynamic uiDynamic;
    public ConsoleComponent developerConsole;

    private static bool exist = false;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (exist)
            DestroyImmediate(this.gameObject);
        exist = true;
    }

    public void ToggleUI(GameManager.GameStateEnum gameState)
    {
        switch (gameState)
        {
            case GameManager.GameStateEnum.MainMenu:
                uiPlayerInfo.Toggle(false);
                uiDynamic.Toggle(false);
                uiMainMenu.Toggle(true);
                uiSettingMenu.Toggle(false);
                uiPauseMenu.Toggle(false);
                break;
            case GameManager.GameStateEnum.InGame:
                uiPlayerInfo.Toggle(true);
                uiDynamic.Toggle(true);
                uiMainMenu.Toggle(false);
                uiSettingMenu.Toggle(false);
                uiPauseMenu.Toggle(false);
                break;
            case GameManager.GameStateEnum.Pause:
                uiPlayerInfo.Toggle(true);
                uiDynamic.Toggle(false);
                uiMainMenu.Toggle(false);
                uiSettingMenu.Toggle(false);
                uiPauseMenu.Toggle(true);
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

    public void StopLoading()
    {
        uiLoading.Toggle(false);
    }

    public void ToggleDeveloperConsole()
    {
        developerConsole.ToggleUi();
    }

    public void ShowDamageText(Vector3 position, int damageCount,bool isFromRight, bool critical)
    {
        uiDynamic.ShowDamageText(position, damageCount, isFromRight, critical);
    }

    public UIHpMonster GetMonsterHP()
    {
        return uiDynamic.GetMonsterHP();
    }
}
