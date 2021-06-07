using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoadingAnimation : MonoBehaviour
{
    public Image blackScreen;

    public float fadeoutTime;
    public float fadeinTime;

    private bool bRunning = false;
    private float timeCheck = 0;
    private bool bValue;

    public void Toggle(bool value)
    {
        this.gameObject.SetActive(true);
        //StopAllCoroutines();
        //StartCoroutine(Animation(value));
        timeCheck = 0;
        bRunning = true;
        bValue = value;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        if (bRunning)
        {
            if (bValue)
            {
                if (timeCheck <= fadeoutTime)
                {
                    blackScreen.color = new Color(0, 0, 0, timeCheck / fadeoutTime);
                }
                else
                {
                    blackScreen.color = new Color(0, 0, 0, 1);
                    bRunning = false;
                }
            }
            else
            {
                if (timeCheck <= fadeinTime)
                {
                    blackScreen.color = new Color(0, 0, 0, 1 - timeCheck / fadeinTime);
                }
                else
                {
                    bRunning = false;
                    this.gameObject.SetActive(false);
                }
            }
            timeCheck += Time.deltaTime;
        }
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
