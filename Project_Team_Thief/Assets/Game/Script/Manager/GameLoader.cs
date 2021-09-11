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
    private string _curTheme = string.Empty;

    private class UsingSceneData
    {
        public List<string> unloadedScenes;
        public int loadCount;
        public string endSceneName;

        public UsingSceneData(SceneGroup data)
        {
            unloadedScenes = new List<string>(data.sceneNames);
            loadCount = data.loadCount;
            endSceneName = data.endSceneName;
        }

        public UsingSceneData(UsingSceneData previousData)
        {
            unloadedScenes = previousData.unloadedScenes.ConvertAll(s => s);
            loadCount = previousData.loadCount;
            endSceneName = previousData.endSceneName;
        }
    }
    private Dictionary<string, UsingSceneData> _unloadedSceneData;
    private UsingSceneData curUsingData;

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
        GameManager.instance.ControlActor = GameManager.instance.UIMng.UiActor;
        InitializeInternalSceneData();
    }

    private void InitializeInternalSceneData()
    {
        // 인스펙터 창에서 넣은 씬에 대한 데이터를 힙공간에 구축
        _unloadedSceneData = new Dictionary<string, UsingSceneData>();
        foreach(var data in _sceneData)
        {
            UsingSceneData unloadSceneData = new UsingSceneData(data);
            _unloadedSceneData.Add(data.theme, unloadSceneData);
        }
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

    private IEnumerator LoadRandomScene(string themeName)
    {
        if(_curTheme != themeName)
        {
            curUsingData = new UsingSceneData(_unloadedSceneData[themeName]);
        }
        if(curUsingData.loadCount <= 0 || curUsingData.unloadedScenes.Count == 0)
        {
            yield return SceneManager.LoadSceneAsync(curUsingData.endSceneName, LoadSceneMode.Single);
        }
        else
        {
            string newSceneName = curUsingData.unloadedScenes[Random.Range(0, curUsingData.unloadedScenes.Count)];
            curUsingData.unloadedScenes.Remove(newSceneName);
            curUsingData.loadCount--;
            yield return SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Single);
        }
    }

    public IEnumerator SceneLoad(string sceneName)
    {
        GameManager.instance.UIMng.AddTextToDeveloperConsole(sceneName + " Scene Load Start");
        GameManager.instance.UIMng.ShowLoading();
        if (!gameDataLoaded)
            yield return StartCoroutine(LoadGameData());

        if (_unloadedSceneData.ContainsKey(sceneName))
        {
            yield return LoadRandomScene(sceneName);
            _curTheme = sceneName;
        }
        else
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            _curTheme = string.Empty;
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
        GameManager.instance.UIMng.AddTextToDeveloperConsole(sceneName + " Scene Load Finished");
    }
}
