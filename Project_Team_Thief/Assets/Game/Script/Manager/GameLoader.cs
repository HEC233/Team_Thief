using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    public static GameLoader instance;

    private bool gameDataLoaded = false;

    public delegate bool SceneLoadCallback(ref string Error);
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
        }
        else
        {
            DestroyImmediate(this.gameObject);
        }
        gameDataLoaded = false;
    }

    private void Start()
    {
        GameManager.instance.GameState = GameStateEnum.MainMenu;
        GameManager.instance.SetControlActor(GameManager.instance.UIMng.UiActor);
        InitializeInternalSceneData();
    }

    private void InitializeInternalSceneData()
    {
        // 인스펙터 창에서 넣은 씬에 대한 데이터를 힙공간에 구축
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

    /// <summary>
    /// 씬이 로드되고 나서 실행되기를 원하는 함수를 등록하기 위한 델리게이트를 할당하는 함수
    /// 함수가 원하는 바를 성공시 true 리턴, 실패시 false 리턴 후 Error인수에 에러내용을 담을것.
    /// </summary>
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
        GameManager.instance.UIMng.AddTextToDeveloperConsole(SceneName + " Scene Load Start");
        GameManager.instance.UIMng.ShowLoading();
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

        string error;
        foreach (var callback in sceneLoadCallbacks)
        {
            error = string.Empty;
            if(!callback(ref error))
            {
                Debug.LogError(error);
                GameManager.instance.UIMng.AddTextToDeveloperConsole(error);
            }
        }

        GameManager.instance.UIMng.StopLoading();
        GameManager.instance.UIMng.AddTextToDeveloperConsole(SceneName + " Scene Load Finished");
    }
}
