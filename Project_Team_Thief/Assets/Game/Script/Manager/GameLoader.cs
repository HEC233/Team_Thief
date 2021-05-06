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

    public void StartGame()
    {
        StartCoroutine(SceneLoad());
        GameManager.instance.ShowLoadingScreen();
    }

    IEnumerator LoadGameData()
    {
        gameDataLoaded = false;
        yield return null;
        yield return StartCoroutine(Addressable.instance.LoadAll());
        gameDataLoaded = true;
    }

    IEnumerator SceneLoad()
    {
        if (!gameDataLoaded)
            yield return StartCoroutine(LoadGameData());

        yield return new WaitForSeconds(2.0f);

        yield return SceneManager.LoadSceneAsync("HHG");
        var grid = GameObject.Find("Grid").GetComponent<Grid>();
        GameManager.instance.grid = grid;
        //---
        GameManager.instance.uiMng.uiPlayerInfo.InitInfo();
        GameManager.instance.GameState = GameManager.GameStateEnum.InGame;
    }
}
