using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    public static GameLoader instance;

    private bool gameDataLoaded = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
        GameManager.instance.GameState = GameManager.GameStateEnum.MainMenu;
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
        GameManager.instance.LoadingScreen(true);
        if (!gameDataLoaded)
            yield return StartCoroutine(LoadGameData());

        //yield return new WaitForSeconds(2.0f);

        GameManager.instance.timeMng.UnbindAll();

        yield return SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Single);

        GameManager.instance.LoadingScreen(false);
    }
}
