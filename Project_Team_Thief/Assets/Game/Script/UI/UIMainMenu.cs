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
        m_lastSelectButton = null;
        GameManager.instance.uiMng.eventSystem.SetSelectedGameObject(m_firstSelectButton);
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

    public void FocusWithMouse()
    {
        m_lastSelectButton = GameManager.instance.uiMng.eventSystem.currentSelectedGameObject;
        GameManager.instance.uiMng.eventSystem.SetSelectedGameObject(null);
    }

    public void FocusWithKeyboard()
    {
        GameManager.instance.uiMng.eventSystem.SetSelectedGameObject(m_lastSelectButton == null ? m_firstSelectButton : m_lastSelectButton);
    }
}
