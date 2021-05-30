using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComboInfo : MonoBehaviour
{
    public Sprite[] digits = new Sprite[10];

    public RectTransform comboRect;
    public RectTransform digitRect;
    public Image[] comboImages = new Image[3];
    public RectTransform[] comboRects = new RectTransform[2];
    public RectTransform comboBase;

    public float comboTime;

    public void SetCombo(int comboCount)
    {
        comboCount = Mathf.Clamp(comboCount, 0, 999);

        int first = comboCount % 10;
        int second = (comboCount / 10) % 10;
        int third = (comboCount / 100) % 10;

        comboImages[0].sprite = digits[first];
        comboImages[1].sprite = digits[second];
        comboImages[2].sprite = digits[third];
        comboImages[1].gameObject.SetActive(second != 0);
        comboImages[2].gameObject.SetActive(third != 0);

        if (second == 0)
        {
            digitRect.localScale = Vector3.one * 1.5f;
        }
        else if (third != 0)
        {
            digitRect.localScale = Vector3.one * 0.75f;
        }
        else
        {
            digitRect.localScale = Vector3.one;
        }

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
            for (int i = 0; i < 3; i++)
            {
                comboImages[i].color = Color.Lerp(new Color(1, 1, 1, 0), new Color(1, 1, 1, 1), timeCheck / 0.1f);
                comboRects[i].localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one, timeCheck / 0.1f);
            }
            yield return null;
            timeCheck += Time.deltaTime;
        }
        comboBase.anchoredPosition = endPos;
        for (int i = 0; i < 3; i++)
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
}
