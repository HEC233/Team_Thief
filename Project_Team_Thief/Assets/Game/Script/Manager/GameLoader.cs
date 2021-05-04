using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    private bool gameDataLoaded = false;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void StartGame()
    {
        StartCoroutine(SceneLoad());
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

        yield return SceneManager.LoadSceneAsync("HHG");
    }
}
