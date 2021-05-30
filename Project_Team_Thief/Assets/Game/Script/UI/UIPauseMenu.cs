using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPauseMenu : MonoBehaviour, IUIFocus
{
    private RectTransform _rect;

    public RectTransform buttons;
    public CanvasGroup canvasGroup;
    public Image background;

    [SerializeField]
    private GameObject m_firstSelectButton;
    private GameObject m_lastSelectButton;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void Toggle(bool value)
    {
        if (!this.gameObject.activeSelf && !value)
            return;
        this.gameObject.SetActive(true);
        m_lastSelectButton = null;
        GameManager.instance.uiMng.eventSystem.SetSelectedGameObject(m_firstSelectButton);
        StopAllCoroutines();
        StartCoroutine(PauseAnimation(value));
    }

    private IEnumerator PauseAnimation(bool value)
    {
        canvasGroup.interactable = false;
        float timeCheck = 0;
        var transparent = new Color(0, 0, 0, 0);
        var opaque = new Color(0, 0, 0, 0.9f);
        float t;

        if (value)
        {
            var initPos = new Vector2(0, -50);
            buttons.anchoredPosition = initPos;
            canvasGroup.alpha = 0f;
            background.color = new Color(0, 0, 0, 0);

            GameManager.instance.timeMng.StopTime();
            yield return new WaitForSeconds(0.1f);
            this.gameObject.SetActive(value);

            while(timeCheck <= 0.3f)
            {
                t = timeCheck / 0.3f;
                buttons.anchoredPosition = Vector2.Lerp(initPos, Vector2.zero, t);
                canvasGroup.alpha = Mathf.Lerp(0.0f, 1.0f, t);
                background.color = Color.Lerp(transparent, opaque, t);

                timeCheck += Time.deltaTime;
                yield return null;
            }

            buttons.anchoredPosition = Vector2.zero;
            canvasGroup.alpha = 1.0f;
            background.color = opaque;
            canvasGroup.interactable = true;
        }
        else
        {
            buttons.anchoredPosition = Vector2.zero;
            canvasGroup.alpha = 1f;
            background.color = new Color(0, 0, 0, 0.9f);

            while (timeCheck <= 0.3f)
            {
                t = timeCheck / 0.3f;
                canvasGroup.alpha = Mathf.Lerp(1.0f, 0.0f, t);
                background.color = Color.Lerp(opaque, transparent, t);

                timeCheck += Time.deltaTime;
                yield return null;
            }

            GameManager.instance.timeMng.ResumeTime();
            this.gameObject.SetActive(value);
        }
    }

    public void Resume()
    {
        GameManager.instance.EscapeButton();
    }

    public void Restart()
    {

    }

    public void Setting()
    {
        GameManager.instance.GameState = GameManager.GameStateEnum.Setting;
    }

    public void ExitToMainMenu()
    {
        GameManager.instance.ExitToMainMenu();
    }

    public void ExitGame()
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
