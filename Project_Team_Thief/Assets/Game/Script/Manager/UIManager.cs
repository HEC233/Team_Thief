using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using PS.Util.DeveloperConsole;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private UIPlayerInfo uiPlayerInfo;
    public UIPlayerInfo UIPlayerInfo => uiPlayerInfo;
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

    // for boss hp
    [SerializeField]
    private GameObject bossHp;
    [SerializeField]
    private Image delayHp;
    [SerializeField]
    private Image curHp;
    [SerializeField]
    private Image maxHp;
    private MonsterUnit _bossUnit;
    private float _lastDelayFill = 1;
    private float t = 0;
    //

    private void Awake()
    {
        if (exist)
            DestroyImmediate(this.gameObject);
        exist = true;
        m_uiActor = new UIActor(this);
    }

    private void Start()
    {
        GameLoader.instance?.AddSceneLoadCallback(InitUI);

    }

    private void Update()
    {
        delayHp.fillAmount = Mathf.Lerp(_lastDelayFill, curHp.fillAmount, t);
        t += GameManager.instance.TimeMng.DeltaTime;
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
            case GameManager.GameStateEnum.None:
                uiPlayerInfo.Toggle(false);
                uiDynamic.Toggle(false);
                uiMainMenu.Toggle(false);
                uiSettingMenu.Toggle(false);
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

    /// <summary>
    /// 보스 HP바는 하나만 존재하므로 ui매니저에서 관리하도록 하였음
    /// </summary>
    /// <param name="unit"></param>
    public void InitBossHP(MonsterUnit unit)
    {
        _bossUnit = unit;

        bossHp.SetActive(true);
    }
    public void SetBossHPColor(Color hpColor, Color backgroundColor)
    {
        curHp.color = hpColor;
        maxHp.color = backgroundColor;
    }
    public void BossHPUpdate()
    {
        curHp.fillAmount = _bossUnit.GetCurHp() / _bossUnit.GetMaxHp();
        _lastDelayFill = delayHp.fillAmount;
        t = 0;
    }
    public void BossDie()
    {
        bossHp.SetActive(false);
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
        bossHp.SetActive(false);
        return true;
    }

    public void SetCombo(int comboCount)
    {
        uiComboInfo.SetCombo(comboCount);
    }

    public void PlayerDead()
    {
        //uiGameOver.gameObject.SetActive(true);
        //StartCoroutine(GameOver());
        GameManager.instance.ReloadScene();
    }

    public void TurnOffGameOverScreen()
    {
        //uiGameOver.gameObject.SetActive(false);
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

    public Vector3 GetScreenPos(Vector3 worldPos)
    {
        float _screenRatio = Screen.currentResolution.width / Screen.currentResolution.height;
        float final = 9 / 16 * _screenRatio;
        float rcp = 1 / final;

        var screenPos = GameManager.instance.CameraMng.mainCam.WorldToScreenPoint(worldPos);
        screenPos = new Vector2(screenPos.x / Screen.width * 480 , screenPos.y / Screen.height * 270);


        return screenPos;
    }

    public void TurnXButtonUI(bool value)
    {
        uiDynamic.TurnChangButton(value);
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
