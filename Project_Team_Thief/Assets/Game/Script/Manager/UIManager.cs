using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PS.Util.DeveloperConsole;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private UIPlayerInfo uiPlayerInfo;
    [SerializeField]
    private UIMainMenu uiMainMenu;
    [SerializeField]
    private UISettingMenu uiSettingMenu;
    [SerializeField]
    private UIPauseMenu uiPauseMenu;
    [SerializeField]
    private UILoadingAnimation uiLoading;
    [SerializeField]
    private UIDynamic uiDynamic;
    [SerializeField]
    private UIComboInfo uiComboInfo;
    [SerializeField]
    private CanvasGroup uiGameOver;
    [SerializeField]
    private GameObject playerDeadResumeButton;
    public ConsoleComponent developerConsole;
    public DialogueUIController uiDialogue;
    public EventSystem eventSystem;
    private IUIFocus m_focusedUI = null;
    private UIActor m_uiActor;
    public IActor UiActor => m_uiActor;

    private static bool exist = false;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        if (exist)
            DestroyImmediate(this.gameObject);
        exist = true;
        m_uiActor = new UIActor(this);
    }

    private void Start()
    {
        GameLoader.instance?.AddSceneLoadCallback(InitUI);
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
                m_focusedUI = uiMainMenu;
                break;
            case GameManager.GameStateEnum.InGame:
                uiPlayerInfo.Toggle(true);
                uiDynamic.Toggle(true);
                uiMainMenu.Toggle(false);
                uiSettingMenu.Toggle(false);
                uiPauseMenu.Toggle(false);
                m_focusedUI = null;
                break;
            case GameManager.GameStateEnum.Pause:
                uiPlayerInfo.Toggle(true);
                uiDynamic.Toggle(false);
                uiMainMenu.Toggle(false);
                uiSettingMenu.Toggle(false);
                uiPauseMenu.Toggle(true);
                m_focusedUI = uiPauseMenu;
                break;
            case GameManager.GameStateEnum.Setting:
                uiPlayerInfo.Toggle(false);
                uiDynamic.Toggle(false);
                uiMainMenu.Toggle(false);
                uiSettingMenu.Toggle(true);
                uiPauseMenu.Toggle(false);
                m_focusedUI = null;
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

    //public void InitUI()
    //{
    //    uiPlayerInfo.CommandUpdate();
    //    uiDynamic.Init(); 
    //}
    public bool InitUI(out string errorMessage)
    {
        errorMessage = string.Empty;
        uiPlayerInfo.CommandUpdate();
        uiDynamic.Init();
        return true;
    }

    public void SetCombo(int comboCount)
    {
        uiComboInfo.SetCombo(comboCount);
    }

    public void PlayerDead()
    {
        uiGameOver.gameObject.SetActive(true);
        StartCoroutine(GameOver());
    }

    public void TurnOffGameOverScreen()
    {
        uiGameOver.gameObject.SetActive(false);
    }

    IEnumerator GameOver()
    {
        float t = 0.0f;
        while(t <= 0.1f)
        {
            uiGameOver.alpha = Mathf.Lerp(0, 1, t / 0.1f);
            yield return null;
            t += Time.deltaTime;
        }
        uiGameOver.alpha = 1;
        eventSystem.SetSelectedGameObject(playerDeadResumeButton);
    }

    public void SetShowCommandInfo(bool value)
    {
        uiPlayerInfo.SetShowCommandInfo(value);
    }

    public class UIActor : IActor
    {
        private UIManager m_uiManager;
        private bool m_bCurrentInputKeyboard = true;
        public UIActor(UIManager manager)
        {
            m_uiManager = manager;
        }

        public Unit GetUnit()
        {
            return null;
        }

        public bool Transition(TransitionCondition condition, object param = null)
        {
            if (condition == TransitionCondition.MouseMove)
            {
                if (m_bCurrentInputKeyboard)
                {
                    m_uiManager.m_focusedUI?.FocusWithMouse();
                    m_bCurrentInputKeyboard = false;
                }
            }
            else if(condition == TransitionCondition.ArrowInput)
            {
                if (!m_bCurrentInputKeyboard)
                {
                    m_uiManager.m_focusedUI?.FocusWithKeyboard();
                    m_bCurrentInputKeyboard = true;
                }
            }

            return true;
        }
    }
}
