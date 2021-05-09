using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPauseMenu : MonoBehaviour
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

    public void Resume()
    {
        GameManager.instance.PauseGame();
    }

    public void Setting()
    {

    }

    public void ExitGame()
    {
        GameManager.instance.ExitToMainMenu();
    }
}
