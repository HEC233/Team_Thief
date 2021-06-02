using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PS.Util.Tile;
using PS.FX;
using PS.Shadow;

public class GameManager : MonoBehaviour
{
    public enum GameStateEnum
    {
        MainMenu,
        InGame,
        Pause,
        Setting,
    }


    public static GameManager instance;

    private GameStateEnum _gameState = GameStateEnum.InGame;
    private GameStateEnum _prevState;
    public GameStateEnum GameState
    {
        get { return _gameState; }
        set { _prevState = _gameState; _gameState = value; uiMng.ToggleUI(value); }
    }

    public FreameChecker frameChecker;

    public CameraManager cameraMng;
    public TimeManager timeMng;
    public SoundManager soundMng;
    public UIManager uiMng;
    public EffectSystem FX;
    public GameSkillMgr GameSkillMgr;
    public ShadowControlManager ShadowControlManager;

    public CommandManager commandManager;
    public Spawner spawner;
    public DialogueSystem dialogueSystem;

    public ShadowParticleSystem shadow;

    [SerializeField]
    private KeyManager _keyManager;
    public Grid grid;

    private GameSettingData _settingData;
    public GameSettingData SettingData
    {
        get { return _settingData; }
    }

    public bool isPlayerDead = false;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        ApplySetting(_settingData);
    }

    private IActor m_playerActor;
    public void SetPlayerActor(IActor actor)
    {
        m_playerActor = actor;
    }

    public IActor GetPlayerActor()
    {
        return m_playerActor;
    }

    public void ChangeActorToPlayer()
    {
        if(m_playerActor != null)
        {
            _keyManager.SetControlActor(m_playerActor);
        }
    }

    public void SetControlActor(IActor actor)
    {
        _keyManager.SetControlActor(actor);
    }

    public IActor GetControlActor()
    {
        return _keyManager.GetControlActor();
    }

    public void SetLoadingScreenShow(bool isStart)
    {
        if (isStart)
            uiMng.ShowLoading();
        else
            uiMng.StopLoading();
    }

    public void StartGame()
    {
        StartCoroutine(StartGameCoroutine("HHG"));
    }

    public void LoadGame(string SceneName)
    {
        StartCoroutine(StartGameCoroutine(SceneName));
    }

    IEnumerator StartGameCoroutine(string SceneName)
    {
        yield return GameLoader.instance.SceneLoad(SceneName);
        GameState = GameStateEnum.InGame;
        ChangeActorToPlayer();
        //uiMng.InitUI(); // SceneLoadCallback���� �Űܾ� �� �ʿ伺�� ������ ����
        timeMng.ResetTime();

        var grid = GameObject.Find("Grid").GetComponent<Grid>();
        this.grid = grid;
        TileCoordClass.SetGrid(grid);
    }

    public void EscapeButton()
    {
        // pause -> inGame
        if (GameState == GameStateEnum.Pause)
        {
            GameState = GameStateEnum.InGame;
            ChangeActorToPlayer();
            dialogueSystem.ResumeDialogue();
        }
        // inGame -> pause
        else if (GameState == GameStateEnum.InGame)
        {
            GameState = GameStateEnum.Pause;
            SetControlActor(uiMng.UiActor);
            dialogueSystem.PauseDialogue();

        }
        // mainMenu -> ExitGame
        else if (GameState == GameStateEnum.MainMenu)
        {
            ExitGame();
        }
        // setting -> prevMenu(pause or mainMenu)
        else if (GameState == GameStateEnum.Setting)
        {
            GameState = _prevState;
            SetControlActor(uiMng.UiActor);
        }
    }

    public void ExitToMainMenu()
    {
        isPlayerDead = false;
        uiMng.TurnOffGameOverScreen();
        dialogueSystem.EndDialogue();
        StartCoroutine(ExitGameCoroutine());
    }

    IEnumerator ExitGameCoroutine()
    {
        shadow.UnRegistAllCollider();

        yield return GameLoader.instance.SceneLoad("MainScene");
        GameState = GameStateEnum.MainMenu;
        SetControlActor(uiMng.UiActor);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void AddTextToDeveloperConsole(string text)
    {
        uiMng.developerConsole?.AddLine(text);
    }

    public void ApplySetting(GameSettingData newData)
    {
        _settingData = newData;

        frameChecker.enabled = _settingData.bShowFPS;
        uiMng.developerConsole?.SetConsoleUsage(_settingData.bUseDeveloperConsole);
        uiMng.SetShowCommandInfo(!_settingData.bDontUseCommandAssist);
    }

    //====================== 빠른 구현을 위해 임의로 여기에 넣어놨음
    public void PushTalkCondition()
    {
        GameObject go = GameObject.Find("NPCManager");
        if (go == null) return;
        var nm = go.GetComponent<NPCManager>();
        if (nm == null) return;

        go = GameObject.Find("GameEventSystem");
        if (go == null) return;
        var es = go.GetComponent<GameEventSystem>();
        if (es == null) return;

        string nearest = nm.GetNearestNPC();
        if (nearest != string.Empty)
            es.AddTalkQueue(nearest);
    }

    //======================
}
