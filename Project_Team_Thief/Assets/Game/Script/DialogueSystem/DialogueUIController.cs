using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUIController : MonoBehaviour
{
    [SerializeField]
    private RectTransform DialogueCanvas;
    [SerializeField]
    private TextMeshProUGUI textBox;
    [SerializeField]
    private RectTransform DialogueBox;
    [SerializeField]
    private GameObject PortraitObject;
    [SerializeField]
    private RectTransform LeftPortraitRect;
    [SerializeField]
    private Image LeftPortraitImage;
    [SerializeField]
    private RectTransform RightPortraitRect;
    [SerializeField]
    private Image RightPortraitImage;
    [SerializeField]
    private GameObject NameBoxObject;
    [SerializeField]
    private TextMeshProUGUI NameBoxText;

    private bool bAnimationEnd = true;
    private string curText;

    private bool bPortraitEnable = false;
    private bool bLeftHighlighted = true;

    private float _waitTime;

    private void Awake()
    {
        //SetShowDialogue(false);
    }
    public void ShowDialoge()
    {
        DialogueCanvas.gameObject.SetActive(true);
    }


    public void SetShowDialogue(bool value)
    {
        DialogueCanvas.gameObject.SetActive(false);
        SetShowName(false);
        EnablePortrait(false);
        SetTextPosition(false);
        bLeftHighlighted = true;
        Highlight();
        bAnimationEnd = true;
    }

    public void Puase()
    {
        StopAllCoroutines();

        Highlight();
    }

    public void Resume()
    {
        if (!bAnimationEnd)
        {
            StartCoroutine(TextAnimationCoroutine(_waitTime, textBox.text.Length));
        }
    }

    public bool CheckAnimationEnd()
    {
        return bAnimationEnd;
    }

    public void FinishAnimation()
    {
        StopAllCoroutines();
        textBox.text = curText;
        Highlight();

        bAnimationEnd = true;
    }

    public void ShowText(in string text, float waitTime)
    {
        _waitTime = waitTime;
        curText = text;
        textBox.text = string.Empty;
        bAnimationEnd = false;
        StartCoroutine(TextAnimationCoroutine(waitTime, 0));
    }

    public void SetBold()
    {

    }

    public void SetTextPosition(bool bUpper)
    {
        if (bUpper)
        {
            bPortraitEnable = PortraitObject.activeSelf;
            EnablePortrait(false);
            SetShowName(false);
            DialogueBox.anchoredPosition = new Vector2(0, 90);
        }
        else
        {
            EnablePortrait(bPortraitEnable);
            DialogueBox.anchoredPosition = new Vector2(0, -90);
        }
    }

    private IEnumerator TextAnimationCoroutine(float waitTime, int startIndex)
    {
        var internalTime = new WaitForSeconds(0.1f);
        for (int i = startIndex; i < curText.Length; i++)
        {
            textBox.text += curText.Substring(i, 1);
            yield return internalTime;
        }
        yield return new WaitForSeconds(waitTime);
        bAnimationEnd = true;
        yield break;
    }

    public void EnablePortrait(bool value)
    {
        PortraitObject.SetActive(value);
    }

    public void SetLeftPortrait(string spriteName)
    {
        LeftPortraitRect.gameObject.SetActive(string.Compare(spriteName, "none", true) != 0);
        LeftPortraitImage.sprite = Addressable.instance.GetSprite(spriteName);
    }
    public void SetRightPortrait(string spriteName)
    {
        RightPortraitRect.gameObject.SetActive(string.Compare(spriteName, "none", true) != 0);
        RightPortraitImage.sprite = Addressable.instance.GetSprite(spriteName);
    }
    public void SetPortraitHighlight(bool bLeft)
    {
        bLeftHighlighted = bLeft;
        Highlight();
        //StartCoroutine(HighlightAnimation());
    }

    // rect를 이용한 애니메이션도 추가?
    private IEnumerator HighlightAnimation()
    {
        float t = 0f;
        const float time = 0.5f;

        Color startLeftColor = LeftPortraitImage.color;
        Color startRightColor = RightPortraitImage.color;
        Color endLeftColor;
        Color endRightColor;

        if (bLeftHighlighted)
        {
            endLeftColor = Color.white;
            endRightColor = Color.gray;
        }
        else
        {
            endLeftColor = Color.gray;
            endRightColor = Color.white;
        }
        while (t <= time)
        {
            LeftPortraitImage.color = Color.Lerp(startLeftColor, endLeftColor, t / time);
            RightPortraitImage.color = Color.Lerp(startRightColor, endRightColor, t / time);

            t += Time.deltaTime;
            yield return null;
        }
        Highlight();

        yield break;
    }
    private void Highlight()
    {
        LeftPortraitImage.color = bLeftHighlighted ? Color.white : Color.gray;
        RightPortraitImage.color = bLeftHighlighted ? Color.gray : Color.white;
    }

    public void SetShowName(bool bValue)
    {
        NameBoxObject.SetActive(bValue);
    }

    public void ChangeName(string value)
    {
        NameBoxText.text = value;
    }
}
