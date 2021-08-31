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
    public static GameManager instance;

    private GameStateEnum _gameState = GameStateEnum.InGame;
    private GameStateEnum _prevState;
    public GameStateEnum GameState
    {
        get { return _gameState; }
        set { _prevState = _gameState; _gameState = value; _uiManager.ToggleUI(value); Debug.Log("GameState Changed : " + _gameState); }
    }

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
    }

    // 게임의 포커스가 나갔을 경우.
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus == false)
        {
            if (GameState == GameStateEnum.InGame)
            {
                PauseGame();
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

    public void StartGame()
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

    public void ReloadScene()
    {
        LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator StartGameCoroutine(string SceneName)
    {
        yield return GameLoader.instance.SceneLoad(SceneName);
        ShadowParticle.UnRegistAllCollider();
        if (DialogueSystem.CheckRunning()) DialogueSystem.EndDialogue();
        GameState = GameStateEnum.InGame;
        WwiseSoundManager.instance.StopAllBGM();
        WwiseSoundManager.instance.PlayInGameBgm(SceneName);
        WwiseSoundManager.instance.PlayAMBSound(SceneName);
        ChangeActorToPlayer();
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
    }

    public void PauseGame()
    {
        switch (GameState) 
        {
            // 인게임에서 정지로
            case GameStateEnum.InGame:
                GameState = GameStateEnum.Pause;
                SetControlActor(_uiManager.UiActor);
                DialogueSystem.PauseDialogue();
                WwiseSoundManager.instance.PlayEventSound("Click_Exit");
                WwiseSoundManager.instance.PauseAllSound();
                break;
            // 정지에서 인게임으로
            case GameStateEnum.Pause:
                GameState = GameStateEnum.InGame;
                ChangeActorToPlayer();
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
                SetControlActor(_uiManager.UiActor);
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
        SetControlActor(_uiManager.UiActor);
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
}
