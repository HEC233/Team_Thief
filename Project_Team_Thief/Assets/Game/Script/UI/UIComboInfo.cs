using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComboInfo : MonoBehaviour
{
    public Sprite[] digits = new Sprite[10];

    public RectTransform comboRect;
    public Image[] comboImages = new Image[2];
    public RectTransform[] comboRects = new RectTransform[2];
    public RectTransform comboBase;

    public float comboTime;

    public void SetCombo(int comboCount)
    {
        comboCount = Mathf.Clamp(comboCount, 0, 99);

        int first = comboCount % 10;
        int second = comboCount / 10;

        comboImages[0].sprite = digits[first];
        comboImages[1].sprite = digits[second];
        comboImages[1].gameObject.SetActive(second != 0);

        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(ComboAnimation());
    }

    IEnumerator ComboAnimation()
    {
        comboRect.anchoredPosition = Vector2.zero;
        float timeCheck = 0;
        var startPos = new Vector2(96.0f, 0);
        var endPos = Vector2.zero;
        while(timeCheck <= 0.1f)
        {
            comboBase.anchoredPosition = Vector2.Lerp(startPos, endPos, timeCheck / 0.1f);
            for (int i = 0; i < 2; i++)
            {
                comboImages[i].color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), timeCheck / 0.1f);
                comboRects[i].localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one, timeCheck / 0.1f);
            }
            yield return null;
            timeCheck += Time.deltaTime;
        }
        comboBase.anchoredPosition = endPos;
        for (int i = 0; i < 2; i++)
        {
            comboImages[i].color = new Color(1, 1, 1, 1);
            comboRects[i].localScale = Vector3.one;
        }

        yield return new WaitForSeconds(comboTime - timeCheck);

        timeCheck = 0;
        while (timeCheck <= 0.1f)
        {
            comboRect.anchoredPosition = Vector2.Lerp(endPos, startPos, timeCheck / 0.1f);
            yield return null;
            timeCheck += Time.deltaTime;
        }

        gameObject.SetActive(false);
    }

    private int c = 0;
    public void AddCombo()
    {
        c++;
        SetCombo(c);
    }
}
