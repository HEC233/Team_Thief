using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using PS.Util.Tile;
using PS.FX;
using PS.Shadow;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameStateEnum
    {
        MainMenu,
        InGame,
        Pause,
        Setting,
        None,
    }


    public static GameManager instance;

    private GameStateEnum _gameState = GameStateEnum.InGame;
    private GameStateEnum _prevState;
    public GameStateEnum GameState
    {
        get { return _gameState; }
        set { _prevState = _gameState; _gameState = value; _uiMng.ToggleUI(value); Debug.Log("GameState Changed : " + _gameState); }
    }

    [SerializeField]
    private FreameChecker _frameChecker;
    public FreameChecker FrameChecker => _frameChecker;
    [SerializeField]
    private CameraManager _cameraMng;
    public CameraManager CameraMng => _cameraMng;
    [SerializeField]
    private TimeManager _timeMng;
    public TimeManager TimeMng => _timeMng;
    [SerializeField]
    private SoundManager _soundMng;
    public SoundManager SoundMng => _soundMng;
    [SerializeField]
    private UIManager _uiMng;
    public UIManager UIMng => _uiMng;
    [SerializeField]
    private EffectSystem _fx;
    public EffectSystem FX => _fx;
    [SerializeField]
    private GameSkillMgr _gameSkillMgr;
    public GameSkillMgr GameSkillMgr => _gameSkillMgr;
    [SerializeField]
    private EncroachmentManager _encroachmentManager;
    public EncroachmentManager EncroachmentManager => _encroachmentManager;

    //public CommandManager commandManager;
    [SerializeField]
    private SkillSlotManager skillSlotManager;
    public SkillSlotManager SkillSlotManager => skillSlotManager;
    [SerializeField]
    private Spawner spawner;
    public Spawner Spawner => spawner;
    [SerializeField]
    private DialogueSystem dialogueSystem;
    public DialogueSystem DialogueSystem => dialogueSystem;
    [SerializeField]
    private ShadowParticleSystem shadow;
    public ShadowParticleSystem Shadow => shadow;

    [SerializeField]
    private KeyManager _keyManager;
    public KeyManager KeyManager => _keyManager;
    [SerializeField]
    private Grid grid;
    public Grid Grid => grid;


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

    // 게임의 포커스가 나갔을 경우.
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus == false)
        {
            if (GameState == GameStateEnum.InGame)
            {
                EscapeButton();
            }
        }
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
            _uiMng.ShowLoading();
        else
            _uiMng.StopLoading();
    }

    public void StartGame()
    {
        StartCoroutine(StartGameCoroutine("Tutorial"));
    }

    public void LoadGame(string SceneName)
    {
        if (SceneName == "MainScene")
        {
            ExitToMainMenu();
        }
        else
        {
            StartCoroutine(StartGameCoroutine(SceneName));
        }
    }

    public void ReloadScene()
    {
        LoadGame(SceneManager.GetActiveScene().name);
    }

    IEnumerator StartGameCoroutine(string SceneName)
    {
        yield return GameLoader.instance.SceneLoad(SceneName);
        shadow.UnRegistAllCollider();
        if (dialogueSystem.CheckRunning()) dialogueSystem.EndDialogue();
        GameState = GameStateEnum.InGame;
        WwiseSoundManager.instance.StopAllBGM();
        WwiseSoundManager.instance.PlayInGameBgm(SceneName);
        WwiseSoundManager.instance.PlayAMBSound(SceneName);
        ChangeActorToPlayer();
        isPlayerDead = false;

        _timeMng.UnbindAll();
        _timeMng.Reset();
        FX.Bind();

        _cameraMng._cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        _cameraMng.FindCameras();
        _cameraMng.Bind();
        var grid = GameObject.Find("Grid")?.GetComponent<Grid>();
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
            WwiseSoundManager.instance.PlayEventSound("Click_Exit");
            WwiseSoundManager.instance.ResumeAllSound();
        }
        // inGame -> pause
        else if (GameState == GameStateEnum.InGame)
        {
            GameState = GameStateEnum.Pause;
            SetControlActor(_uiMng.UiActor);
            dialogueSystem.PauseDialogue();
            WwiseSoundManager.instance.PlayEventSound("Click_Exit");
            WwiseSoundManager.instance.PauseAllSound();
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
            SetControlActor(_uiMng.UiActor);
            WwiseSoundManager.instance.ResumeAllSound();
        }
    }

    public void ExitToMainMenu()
    {
        isPlayerDead = false;
        _uiMng.TurnOffGameOverScreen();
        dialogueSystem.EndDialogue();
        StartCoroutine(ExitGameCoroutine());
    }

    IEnumerator ExitGameCoroutine()
    {
        shadow.UnRegistAllCollider();
        yield return GameLoader.instance.SceneLoad("MainScene");
        WwiseSoundManager.instance.ResumeAllSound();
        Debug.Log("Sound Main BGM");
        WwiseSoundManager.instance.StopInGameBgm();
        WwiseSoundManager.instance.PlayMainBgm();
        GameState = GameStateEnum.MainMenu;
        SetControlActor(_uiMng.UiActor);
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
        _uiMng.developerConsole?.AddLine(text);
    }

    public void ApplySetting(GameSettingData newData)
    {
        _settingData = newData;

        _frameChecker.enabled = _settingData.bShowFPS;
        _uiMng.developerConsole?.SetConsoleUsage(_settingData.bUseDeveloperConsole);
        _uiMng.SetShowCommandInfo(!_settingData.bDontUseCommandAssist);
    }

    //====================== 빠른 구현을 위해 임의로 여기에 넣어놨음
    public void PushEventQueue()
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
            es.AddQueue(nearest);
    }

    //======================
}
