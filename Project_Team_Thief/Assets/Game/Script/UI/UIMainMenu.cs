using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIMainMenu : MonoBehaviour, IUIFocus
{
    private RectTransform _rect;

    [SerializeField]
    private GameObject m_firstSelectButton;
    private GameObject m_lastSelectButton;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void Toggle(bool value)
    {
        this.gameObject.SetActive(value);
        if (value)
        {
            m_lastSelectButton = null;
            GameManager.instance.UIMng.eventSystem.SetSelectedGameObject(m_firstSelectButton);
        }
    }

    public void GameStart()
    {
        GameManager.instance.StartGame();
        WwiseSoundManager.instance.PlayEventSound("Click");
    }

    public void GameSetting()
    {
        GameManager.instance.GameState = GameStateEnum.Setting;
        WwiseSoundManager.instance.PlayEventSound("Click");
    }

    public void GameEnd()
    {
        GameManager.instance.ExitGame();
        WwiseSoundManager.instance.PlayEventSound("Click");
    }

    public void FocusWithMouse()
    {
        m_lastSelectButton = GameManager.instance.UIMng.eventSystem.currentSelectedGameObject;
        GameManager.instance.UIMng.eventSystem.SetSelectedGameObject(null);
    }

    public void FocusWithKeyboard()
    {
        GameManager.instance.UIMng.eventSystem.SetSelectedGameObject(m_lastSelectButton == null ? m_firstSelectButton : m_lastSelectButton);
    }
}
