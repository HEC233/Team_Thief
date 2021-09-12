using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillCooltimeBox : MonoBehaviour
{
    [SerializeField]
    private Image coolTimeImage;
    [SerializeField]
    private Image skillIcon;
    [SerializeField]
    private RectTransform skillIconRect;

    private bool bCoolTimeFlag;

    public void SetSkillIcon(Sprite sprite)
    {
        skillIcon.sprite = sprite;
        Debug.Log(sprite.name);
        bCoolTimeFlag = true;
    }

    float t = 0.5f;
    const float length = 0.5f;

    public void CustomUpdate(float cooltimeRatio)
    {
        coolTimeImage.fillAmount = 1 - cooltimeRatio;

        if (coolTimeImage.fillAmount != 0)
        {
            bCoolTimeFlag = false;
        }
        else
        {
            if (!bCoolTimeFlag)
            {
                bCoolTimeFlag = true;
                t = 0;
            }
            else
            {
                if (t <= length)
                {
                    skillIconRect.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one, t / length);

                    t += Time.deltaTime;
                }
                else
                {
                    skillIconRect.localScale = Vector3.one;
                }
            }
        }
    }
}
