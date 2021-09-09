using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using PS.Util.Tile;
using PS.FX;
using PS.Shadow;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

/// <summary>
/// GameManager는 게임의 핵심 로직(플로우)만 담당하게
/// 그리고 코드를 최대한 간결하고 읽기 쉽게
/// </summary>

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private GameStateEnum _gameState = GameStateEnum.InGame;
    private GameStateEnum _prevState;
    public GameStateEnum GameState
    {
        get { return _gameState; }
        set { _prevState = _gameState; _gameState = value; _uiManager.ToggleUI(value); Debug.Log("GameState Changed : " + _gameState); }
    }

    [SerializeField] 
    private GameObject _playerPrefab;
    private GameObject _playerGameObject;
    
    [SerializeField]
    private FreameChecker _frameChecker;
    public FreameChecker FrameChecker => _frameChecker;
    [SerializeField]
    private CameraManager _cameraManager;
    public CameraManager CameraMng => _cameraManager;
    [SerializeField]
    private TimeManager _timeManager;
    public TimeManager TimeMng => _timeManager;
    [SerializeField]
    private SoundManager _soundManager;
    public SoundManager SoundMng => _soundManager;
    [SerializeField]
    private UIManager _uiManager;
    public UIManager UIMng => _uiManager;
    [SerializeField]
    private EffectSystem _fx;
    public EffectSystem FX => _fx;
    [SerializeField]
    private GameSkillMgr _gameSkillManagerr;
    public GameSkillMgr GameSkillMng => _gameSkillManagerr;
    [SerializeField]
    private EncroachmentManager _encroachmentManager;
    public EncroachmentManager EncroachmentMng => _encroachmentManager;
    //public CommandManager commandManager;
    [SerializeField]
    private SkillSlotManager _skillSlotManager;
    public SkillSlotManager SkillSlotMng => _skillSlotManager;
    [SerializeField]
    private Spawner _spawner;
    public Spawner Spawner => _spawner;
    [SerializeField]
    private DialogueSystem _dialogueSystem;
    public DialogueSystem DialogueSystem => _dialogueSystem;
    [SerializeField]
    private ShadowParticleSystem _shadowParticleSystem;
    public ShadowParticleSystem ShadowParticle => _shadowParticleSystem;
    [SerializeField]
    private KeyManager _keyManager;
    public KeyManager KeyMng => _keyManager;
    [SerializeField]
    private NPCManager _npcManager;
    public NPCManager NPCMng => _npcManager;
    [SerializeField]
    private GameSettingController _gameSettingCtrl;
    private Grid grid;

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

        _mapEndEvents.AddListener(DebugLogger);

        _mapStartEvents.AddListener(MapStartLogger);
        _mapEndEvents.AddListener(MapEndLogger);
    }

    // 게임의 포커스가 나갔을 경우.
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus == false)
        {
            if (_gameState == GameStateEnum.InGame)
            {
                //PauseGame();
            }
        }
    }

    //--------- 액터(컨트롤 대상)관련 로직 처리 ----------
    private IActor _playerActor;
    public IActor PlayerActor
    {
        get { return _playerActor; }
    }

    private void SpawnPlayer()
    {
        if (isPlayerDead == true)
        {
            Destroy(_playerGameObject);
            _playerGameObject = null;
        }
        
        if (_playerGameObject == null)
        {
            _playerGameObject = GameObject.Instantiate(_playerPrefab);
            _playerActor = _playerGameObject.GetComponent<PlayerFSMSystem>();
            ChangeCurActorToPlayer();
        }
        
        _playerGameObject.transform.position = Vector3.zero;
    }
    
    /// <summary>
    /// Control Actor, 현재 플레이어의 입력을 받아 처리하는 액터
    /// </summary>
    public IActor ControlActor
    {
        get { return _keyManager.GetControlActor(); }
        set { _keyManager.SetControlActor(value); }
    }

    public void ChangeCurActorToPlayer()
    {
        if(_playerActor != null)
        {
            _keyManager.SetControlActor(_playerActor);
        }
    }
    //---------------------------------------------------

    //---------- 씬(맵)과 관련된 로직 --------------
    public void StartNewGame()
    {
        StartCoroutine(StartGameCoroutine("Tutorial"));
    }

    public void LoadScene(string SceneName)
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

    // 플레이어가 죽으면 메인씬, (현재 튜토리얼이 그 역활 중)으로 이동
    public void ReloadScene()
    {
        LoadScene("Tutorial");
    }

    IEnumerator StartGameCoroutine(string SceneName)
    {
        yield return GameLoader.instance.SceneLoad(SceneName);
        //
        SpawnPlayer();
        //
        
        ShadowParticle.UnRegistAllCollider();
        if (DialogueSystem.CheckRunning()) DialogueSystem.EndDialogue();
        GameState = GameStateEnum.InGame;
        WwiseSoundManager.instance.StopAllBGM();
        WwiseSoundManager.instance.PlayInGameBgm(SceneName);
        WwiseSoundManager.instance.PlayAMBSound(SceneName);
        ChangeCurActorToPlayer();
        isPlayerDead = false;

        _timeManager.UnbindAll();
        _timeManager.Reset();
        FX.Bind();

        _cameraManager._cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
        _cameraManager.FindCameras();
        _cameraManager.Bind();
        // 맵 그리드 할당
        var grid = GameObject.Find("Grid")?.GetComponent<Grid>();
        this.grid = grid;
        TileCoordClass.SetGrid(grid);
        
        
        // 맵 클리어 이벤트 인보커 할당
        var invoker = GameObject.FindObjectOfType<MapEndTrigger>();
        if (invoker != null)
        {
            invoker.GetComponent<MapEndTrigger>().RegistUnityEvent(_mapEndEvents);
        }
        else
        {
            Debug.LogWarning("There is no Map End Trigger!!!");
        }
        
        _mapStartEvents.Invoke();
    }

    public void PauseGame()
    {
        switch (_gameState) 
        {
            // 인게임에서 정지로
            case GameStateEnum.InGame:
                GameState = GameStateEnum.Pause;
                ControlActor = _uiManager.UiActor;
                DialogueSystem.PauseDialogue();
                WwiseSoundManager.instance.PlayEventSound("Click_Exit");
                WwiseSoundManager.instance.PauseAllSound();
                break;
            // 정지에서 인게임으로
            case GameStateEnum.Pause:
                GameState = GameStateEnum.InGame;
                ChangeCurActorToPlayer();
                DialogueSystem.ResumeDialogue();
                WwiseSoundManager.instance.PlayEventSound("Click_Exit");
                WwiseSoundManager.instance.ResumeAllSound();
                break;
            // 메인메뉴에서 게임 종료
            case GameStateEnum.MainMenu:
                ExitGame();
                break;
            // 세팅에서 세팅끄기
            case GameStateEnum.Setting:
                GameState = _prevState;
                ControlActor = _uiManager.UiActor;
                WwiseSoundManager.instance.ResumeAllSound();
                break;
        }
    }

    public void ExitToMainMenu()
    {
        isPlayerDead = false;
        _uiManager.TurnOffGameOverScreen();
        DialogueSystem.EndDialogue();
        StartCoroutine(LoadMainMenuCoroutine());
    }

    IEnumerator LoadMainMenuCoroutine()
    {
        ShadowParticle.UnRegistAllCollider();
        yield return GameLoader.instance.SceneLoad("MainScene");
        WwiseSoundManager.instance.ResumeAllSound();
        Debug.Log("Sound Main BGM");
        WwiseSoundManager.instance.StopInGameBgm();
        WwiseSoundManager.instance.PlayMainBgm();
        GameState = GameStateEnum.MainMenu;
        ControlActor = _uiManager.UiActor;
    }

    public void ExitGame()
    {
        // 여기서 파일 저장

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ApplySetting(GameSettingData newData)
    {
        _settingData = newData;

        FrameChecker.enabled = _settingData.bShowFPS;
        _uiManager.developerConsole?.SetConsoleUsage(_settingData.bUseDeveloperConsole);
        _uiManager.SetShowCommandInfo(!_settingData.bDontUseCommandAssist);
    }

    //--------------------------------------------------

    //---------- 맵의 진행 로직과 관련된 부분 ----------

    // public으로 노출시키지 않은 것은 밖에서 Invoke 하는 것을 막기 위함
    private UnityEvent _mapStartEvents = new UnityEvent();
    private UnityEvent _mapEndEvents = new UnityEvent();

    private void MapStartLogger()
    {
        Debug.Log("Map Start Event Invoked");
    }
    private void MapEndLogger()
    {
        Debug.Log("Map End Event Invoked");
    }

    public void AddMapStartEventListener(UnityAction action)
    {
        _mapStartEvents.AddListener(action);
    }
    public void AddMapEndEventListener(UnityAction action)
    {
        _mapEndEvents.AddListener(action);
    }

    public void DebugLogger()
    {
        Debug.Log("MapEndEventInvoked!");
    }

    //--------------------------------------------------
}
