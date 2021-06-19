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

    private Coroutine pauseAnimation;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void Toggle(bool value)
    {
        if (pauseAnimation == null && this.gameObject.activeSelf == value)
        {
            return;
        }
        this.gameObject.SetActive(true);
        if (value)
        {
            m_lastSelectButton = null;
            GameManager.instance.uiMng.eventSystem.SetSelectedGameObject(m_firstSelectButton);
            GameManager.instance.timeMng.StopTime();
        }
        else
        {
            GameManager.instance.timeMng.ResumeTime();
        }
        if (pauseAnimation != null)
        {
            EndAnimation(value);
        }
        else
        {
            pauseAnimation = StartCoroutine(PauseAnimation(value));
        }
    }

    private void EndAnimation(bool value)
    {
        StopCoroutine(pauseAnimation);
        pauseAnimation = null;
        if (value)
        {
            buttons.anchoredPosition = Vector2.zero;
            canvasGroup.alpha = 1.0f;
            background.color = new Color(0, 0, 0, 0.9f);
            canvasGroup.interactable = true;
        }
        else
        {
            canvasGroup.interactable = false;
        }
        this.gameObject.SetActive(value);
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

            this.gameObject.SetActive(value);
        }

        pauseAnimation = null;
    }

    public void Resume()
    {
        GameManager.instance.EscapeButton();
        WwiseSoundManager.instance.PlayEventSound("Click");
    }

    public void Restart()
    {
        GameManager.instance.ReloadScene();
        WwiseSoundManager.instance.PlayEventSound("Click");
    }

    public void Setting()
    {
        GameManager.instance.GameState = GameManager.GameStateEnum.Setting;
        WwiseSoundManager.instance.PlayEventSound("Click");
    }

    public void ExitToMainMenu()
    {
        GameManager.instance.ExitToMainMenu();
        WwiseSoundManager.instance.PlayEventSound("Click");
    }

    public void ExitGame()
    {
        GameManager.instance.ExitGame();
        WwiseSoundManager.instance.PlayEventSound("Click");
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
