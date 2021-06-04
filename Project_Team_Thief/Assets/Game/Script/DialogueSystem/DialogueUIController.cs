using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueUIController : MonoBehaviour
{
    [SerializeField]
    private RectTransform dialogueCanvas;
    [SerializeField]
    private TextMeshProUGUI textBox;
    [SerializeField]
    private RectTransform dialogueTextBox;
    [SerializeField]
    private RectTransform dialogueBox;
    [SerializeField]
    private GameObject portraitObject;
    [SerializeField]
    private RectTransform leftPortraitRect;
    [SerializeField]
    private Image leftPortraitImage;
    [SerializeField]
    private RectTransform rightPortraitRect;
    [SerializeField]
    private Image rightPortraitImage;
    [SerializeField]
    private RectTransform nameBoxRect;
    [SerializeField]
    private TextMeshProUGUI nameBoxText;
    [SerializeField]
    private GameObject nextButton;

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
        dialogueCanvas.gameObject.SetActive(true);
    }


    public void SetShowDialogue(bool value)
    {
        dialogueCanvas.gameObject.SetActive(false);
        SetShowName(false);
        EnablePortrait(false);
        SetTextPosition(false);
        ShowInteractiveButton(false);
        bLeftHighlighted = true;
        Highlight();
        bAnimationEnd = true;
    }

    public void ShowInteractiveButton(bool value)
    {
        nextButton.SetActive(value);
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
            bPortraitEnable = portraitObject.activeSelf;
            EnablePortrait(false);
            SetShowName(false);
            dialogueTextBox.sizeDelta = new Vector2(330, 50);
            dialogueBox.anchoredPosition = new Vector2(0, 88);
        }
        else
        {
            EnablePortrait(bPortraitEnable);
            dialogueTextBox.sizeDelta = new Vector2(200, 50);
            dialogueBox.anchoredPosition = new Vector2(0, -85);
        }
    }

    private IEnumerator TextAnimationCoroutine(float waitTime, int startIndex)
    {
        var internalTime = new WaitForSeconds(0.1f);
        for (int i = startIndex; i < curText.Length; i++)
        {
            var subStr = curText.Substring(i, 1);
            textBox.text += subStr;
            yield return internalTime;
        }
        yield return new WaitForSeconds(waitTime);
        bAnimationEnd = true;
        yield break;
    }

    public void EnablePortrait(bool value)
    {
        portraitObject.SetActive(value);
        bPortraitEnable = value;
    }

    public void SetLeftPortrait(string spriteName)
    {
        leftPortraitRect.gameObject.SetActive(string.Compare(spriteName, "none", true) != 0);
        leftPortraitImage.sprite = Addressable.instance.GetSprite(spriteName);
    }
    public void SetRightPortrait(string spriteName)
    {
        rightPortraitRect.gameObject.SetActive(string.Compare(spriteName, "none", true) != 0);
        rightPortraitImage.sprite = Addressable.instance.GetSprite(spriteName);
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

        Color startLeftColor = leftPortraitImage.color;
        Color startRightColor = rightPortraitImage.color;
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
            leftPortraitImage.color = Color.Lerp(startLeftColor, endLeftColor, t / time);
            rightPortraitImage.color = Color.Lerp(startRightColor, endRightColor, t / time);

            t += Time.deltaTime;
            yield return null;
        }
        Highlight();

        yield break;
    }
    private void Highlight()
    {
        //LeftPortraitImage.color = bLeftHighlighted ? Color.white : Color.gray;
        //RightPortraitImage.color = bLeftHighlighted ? Color.gray : Color.white;
        leftPortraitImage.gameObject.SetActive(bLeftHighlighted);
        rightPortraitRect.gameObject.SetActive(!bLeftHighlighted);
        nameBoxRect.anchoredPosition = bLeftHighlighted ? new Vector3(-160f, 1.3f, 0f) : new Vector3(160f, 1.3f, 0f);
    }

    public void SetShowName(bool bValue)
    {
        nameBoxRect.gameObject.SetActive(bValue);
    }

    public void ChangeName(string value)
    {
        nameBoxText.text = value;
    }
}
