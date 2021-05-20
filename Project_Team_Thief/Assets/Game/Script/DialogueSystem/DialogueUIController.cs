using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueUIController : MonoBehaviour
{
    private bool bAnimationEnd = true;
    private string curText;

    public TextMeshProUGUI textBox;

    public bool CheckAnimationEnd()
    {
        return bAnimationEnd;
    }

    public void FinishAnimation()
    {
        StopAllCoroutines();
        bAnimationEnd = true;
    }

    public void ShowText(in string text)
    {
        curText = text;
        textBox.text = curText;
        bAnimationEnd = false;
        StartCoroutine(TextAnimationCoroutine());
    }

    private IEnumerator TextAnimationCoroutine()
    {

        bAnimationEnd = true;
        yield break;
    }


}
