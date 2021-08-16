using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIHpReduceInfo : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public Animator animator;
    RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();   
    }

    public void Show(Vector2 screenPos, int damageCount, bool isFromRight, bool critical)
    {
        this.gameObject.SetActive(true);
        _rect.localScale = Vector3.one;
        _rect.rotation = Quaternion.identity;

        _rect.anchoredPosition = screenPos;
        damageText.text = damageCount.ToString();

        animator.enabled = true;
        if (critical)
            StartCoroutine(CriticalAnimation());
        else
            StartCoroutine(NormalAnimation(!isFromRight));
    }

    private void End()
    {
        animator.enabled = false;
        this.gameObject.SetActive(false);
    }

    public void SetVisible(bool value)
    {
        if (value)
        {
            damageText.alpha = 1;
            animator.speed = 1;
        }
        else
        {
            damageText.alpha = 0;
            animator.speed = 0;
        }
    }

    IEnumerator NormalAnimation(bool isGoingRight)
    {
        damageText.color = Color.white;
        damageText.fontSize = 20;
        float timeCheck = 0.5f;
        var speed = new Vector2(isGoingRight ? 100f : -100f, 100f);

        animator.ResetTrigger("Critical");
        while (timeCheck > 0)
        {
            _rect.anchoredPosition += speed * GameManager.instance.TimeMng.DeltaTime;
            
            speed = new Vector2(speed.x, speed.y - 400.0f * GameManager.instance.TimeMng.DeltaTime);

            //damageText.color = new Color(damageText.color.r, damageText.color.g, damageText.color.b, timeCheck * 2f);
            //_rect.localScale = Vector3.one * (timeCheck + 0.5f);

            timeCheck -= GameManager.instance.TimeMng.DeltaTime;
            yield return null;
        }
        End();
    }

    IEnumerator CriticalAnimation()
    {
        animator.SetTrigger("Critical");

        damageText.color = Color.yellow;
        damageText.fontSize = 24;

        float timeCheck = 1f;
        var speed = new Vector2(0f, 50f);

        while (timeCheck > 0)
        {
            _rect.anchoredPosition += speed * GameManager.instance.TimeMng.DeltaTime;

            timeCheck -= GameManager.instance.TimeMng.DeltaTime;
            yield return null;
        }
        End();
    }
}
