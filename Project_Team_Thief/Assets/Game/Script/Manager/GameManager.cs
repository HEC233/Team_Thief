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
    }


    public static GameManager instance;

    private GameStateEnum _gameState = GameStateEnum.InGame;
    public GameStateEnum GameState
    {
        get { return _gameState; }
        set { _gameState = value; uiMng.ToggleUI(value); }
    }

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
    private KeyManager _keyManger;
    public Grid grid;

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
        TileCoordClass.SetGrid(grid);
    }

    public void SetControlActor(IActor actor)
    {
        _keyManger.SetControlActor(actor);
    }

    public IActor GetControlActor()
    {
        return _keyManger.GetControlActor();
    }

    public void LoadingScreen(bool isStart)
    {
        if (isStart)
            uiMng.ShowLoading();
        else
            uiMng.StopLoading();
    }

    public void StartGame()
    {
        StartCoroutine(StartGameCoroutine());
    }

    IEnumerator StartGameCoroutine()
    {
        yield return GameLoader.instance.SceneLoad("HHG");
        GameState = GameStateEnum.InGame;
        uiMng.InitUI(); // SceneLoadCallback���� �Űܾ� �� �ʿ伺�� ������ ����

        var grid = GameObject.Find("Grid").GetComponent<Grid>();
        this.grid = grid;
    }

    private IActor nullActor = new NullActor();
    private IActor prevUnit = null;
    public void EscapeButton()
    {
        if (GameState == GameStateEnum.Pause)
        {
            GameState = GameStateEnum.InGame;
            _keyManger.SetControlActor(prevUnit);
            dialogueSystem.ResumeDialogue();
        }
        else if (GameState == GameStateEnum.InGame)
        {
            GameState = GameStateEnum.Pause;
            prevUnit = _keyManger.GetControlActor();
            _keyManger.SetControlActor(nullActor);
            dialogueSystem.PauseDialogue();

        }
        else if (GameState == GameStateEnum.MainMenu)
        {
            ExitGame();
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
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
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
        var es = go.GetComponent<EventSystem>();
        if (es == null) return;

        string nearest = nm.GetNearestNPC();
        if (nearest != string.Empty)
            es.AddTalkQueue(nearest);
    }

    //======================
}
