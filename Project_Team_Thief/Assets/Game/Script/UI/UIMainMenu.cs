using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour
{
    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void Toggle(bool value)
    {
        this.gameObject.SetActive(value);
    }

    public void GameStart()
    {
        GameManager.instance.StartGame();
    }

    public void GameSetting()
    {

    }

    public void GameEnd()
    {
        GameManager.instance.ExitGame();
    }
}
