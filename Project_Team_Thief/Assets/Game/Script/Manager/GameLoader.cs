using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    public static GameLoader instance;

    private bool gameDataLoaded = false;

    public delegate bool SceneLoadCallback(out string Error);
    private List<SceneLoadCallback> sceneLoadCallbacks = new List<SceneLoadCallback>();

    // 랜덤 씬 로드를 위한 자료구조
    [System.Serializable]
    public class SceneGroup
    {
        public string theme;
        public string[] sceneNames;
        public int loadCount;
        public string endSceneName;
    }
    [SerializeField]
    private SceneGroup[] _sceneData;
    private Dictionary<string, bool[]> _sceneLoadCheck;
    private int[] _sceneLoadCount;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            DestroyImmediate(this.gameObject);
        }
        gameDataLoaded = false;
    }

    private void Start()
    {
        GameManager.instance.GameState = GameManager.GameStateEnum.MainMenu;
        GameManager.instance.SetControlActor(GameManager.instance.UIMng.UiActor);
        InitializeInternalSceneData();
    }

    private void InitializeInternalSceneData()
    {
        // 인스펙터 창에서 넣은 데이터를 힙공간에 구축
        _sceneLoadCheck = new Dictionary<string, bool[]>();
        foreach (var data in _sceneData)
        {
            var sceneLoadCheckBoolean = new bool[data.sceneNames.Length];
            for (int i = 0; i < sceneLoadCheckBoolean.Length; i++)
            {
                sceneLoadCheckBoolean[i] = false;
            }
            _sceneLoadCheck.Add(data.theme, sceneLoadCheckBoolean);
        }
        _sceneLoadCount = new int[_sceneData.Length];
    }

    // 씬 로드시 필요한 초기화함수들을 델리게이트로 추가해주면 죠스입니다.
    public void AddSceneLoadCallback(SceneLoadCallback callback)
    {
        if (!sceneLoadCallbacks.Contains(callback))
        {
            sceneLoadCallbacks.Add(callback);
        }
    }

    IEnumerator LoadGameData()
    {
        gameDataLoaded = false;
        yield return null;
        yield return StartCoroutine(Addressable.instance.LoadAll());
        gameDataLoaded = true;
    }

    public IEnumerator SceneLoad(string SceneName)
    {
        GameManager.instance?.AddTextToDeveloperConsole(SceneName + " Scene Load Start");
        GameManager.instance?.SetLoadingScreenShow(true);
        if (!gameDataLoaded)
            yield return StartCoroutine(LoadGameData());

        if (_sceneLoadCheck.ContainsKey(SceneName))
        {
            int index = 0;
            while(_sceneData[index].theme != SceneName)
            {
                index++;
            }
            if (_sceneLoadCount[index] == _sceneData[index].loadCount)
            {
                yield return SceneManager.LoadSceneAsync(_sceneData[index].endSceneName, LoadSceneMode.Single);
            }
            else
            {
                var sceneCheck = _sceneLoadCheck[SceneName];
                int randomIndex = 0;
                for (int i = 0; i < 10; i++)
                {
                    randomIndex = Random.Range(0, sceneCheck.Length);
                    if (!sceneCheck[randomIndex])
                    {
                        sceneCheck[randomIndex] = true;
                        break;
                    }
                }
                _sceneLoadCount[index]++;
                yield return SceneManager.LoadSceneAsync(_sceneData[index].sceneNames[randomIndex], LoadSceneMode.Single);
            }
        }
        else
        {
            yield return SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);
        }
        
        GameManager.instance.TimeMng.UnbindAll();
        
        foreach (var callback in sceneLoadCallbacks)
        {
            string error = string.Empty;
            if(!callback(out error))
            {
                Debug.LogError(error);
                GameManager.instance?.AddTextToDeveloperConsole(error);
            }
        }

        GameManager.instance?.SetLoadingScreenShow(false);
        GameManager.instance?.AddTextToDeveloperConsole(SceneName + " Scene Load Finished");
    }
}
