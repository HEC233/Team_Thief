using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashCtrl : MonoBehaviour
{
    float _flashOffTime = 0.3f;

    public void SetFlash(float flashOffTime)
    {
        _flashOffTime = flashOffTime;
        StartCoroutine(offFlash());
    }

    IEnumerator offFlash()
    {
        yield return new WaitForSeconds(_flashOffTime);

        gameObject.SetActive(false);
    }
}
