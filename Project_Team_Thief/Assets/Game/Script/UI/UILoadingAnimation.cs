using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UILoadingAnimation : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void Toggle(bool value)
    {
        this.gameObject.SetActive(value);
    }

    private void OnEnable()
    {
        StartCoroutine(Animation());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator Animation()
    {
        var wait = new WaitForSeconds(0.3f);
        while(true)
        {
            text.text = "Loading.";
            yield return wait;
            text.text = "Loading..";
            yield return wait;
            text.text = "Loading...";
            yield return wait;

        }
    }
}
