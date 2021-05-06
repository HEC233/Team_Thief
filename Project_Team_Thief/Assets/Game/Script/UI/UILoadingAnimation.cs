using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoadingAnimation : MonoBehaviour
{
    public Image blackScreen;

    public float fadeoutTime;
    public float fadeinTime;

    public void Toggle(bool value)
    {
        this.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(Animation(value));
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator Animation(bool isStart)
    {
        float timeCheck = 0;
        if (isStart)
        {
            while (timeCheck <= fadeoutTime)
            {
                blackScreen.color = new Color(0, 0, 0, timeCheck / fadeoutTime);

                timeCheck += Time.deltaTime;

                yield return null;
            }
            blackScreen.color = new Color(0, 0, 0, 1);
        }
        else
        {
            while (timeCheck <= fadeinTime)
            {
                blackScreen.color = new Color(0, 0, 0, 1 - timeCheck / fadeinTime);

                timeCheck += Time.deltaTime;

                yield return null;
            }

            this.gameObject.SetActive(false);
        }
    }
}
