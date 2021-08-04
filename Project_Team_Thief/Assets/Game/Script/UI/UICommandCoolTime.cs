using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICommandCoolTime : MonoBehaviour
{
    [SerializeField]
    private Image coolTimeImage;
    [SerializeField]
    private Image skillIcon;
    [SerializeField]
    private RectTransform skillIconRect;
    
    // 김태성 수정 CommandCtrl -> SkillSlot
    private SkillSlotManager.SkillSlot _data;

    private bool bCoolTimeReady;

    public void SetSkillIcon(Sprite sprite)
    {
        skillIcon.sprite = sprite;
    }

    // 김태성 수정 CommandCtrl -> SkillSlot
    public void SetData(SkillSlotManager.SkillSlot data)
    {
        _data = data;
        bCoolTimeReady = false;
    }

    float t = 0;
    const float length = 0.5f;

    private void Update()
    {
        coolTimeImage.fillAmount = 1 - Mathf.Clamp01(_data.SkillSlotCurCoolTime / _data.SkillDataBase.CoolTime);

        if (coolTimeImage.fillAmount != 0)
        {
            bCoolTimeReady = false;
        }
        else
        {
            if (!bCoolTimeReady)
            {
                bCoolTimeReady = true;
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

    //private IEnumerator CoolTimeReadyAnimation()
    //{
    //    float t = 0;
    //    const float length = 0.5f;
    //    while (t <= length)
    //    {
    //        skillIconRect.localScale = Vector3.Lerp(Vector3.one * 1.5f, Vector3.one, t / length);

    //        t += Time.deltaTime;
    //        yield return null;
    //    }
    //    skillIconRect.localScale = Vector3.one;
    //    yield break;
    //}
}
