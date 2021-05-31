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
        GameManager.instance.SetControlActor(GameManager.instance.uiMng.UiActor);
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
        GameManager.instance.SetLoadingScreenShow(true);
        if (!gameDataLoaded)
            yield return StartCoroutine(LoadGameData());

        //yield return new WaitForSeconds(2.0f);

        GameManager.instance.timeMng.UnbindAll();

        yield return SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);

        foreach (var callback in sceneLoadCallbacks)
        {
            string error = string.Empty;
            if(!callback(out error))
            {
                Debug.LogError(error);
                GameManager.instance?.AddTextToDeveloperConsole(error);
            }
        }

        GameManager.instance.SetLoadingScreenShow(false);
        GameManager.instance?.AddTextToDeveloperConsole(SceneName + " Scene Load Finished");
    }
}
