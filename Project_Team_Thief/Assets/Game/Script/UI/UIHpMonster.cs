using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHpMonster : MonoBehaviour
{
    private RectTransform _rect;
    public RectTransform maxHp;
    public RectTransform delayHp;
    public RectTransform curHp;

    private MonsterUnit attachedUnit;

    private bool bShow = false;
    private bool bInited = false;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void SetUninit()
    {
        bInited = false;
    }

    public void Init(MonsterUnit unit)
    {
        attachedUnit = unit;
        attachedUnit.hitEvent.AddListener(HpUpdate);
        attachedUnit.dieEvent.AddListener(Detach);

        _rect.sizeDelta = new Vector2(attachedUnit.GetMaxHp() / 200, 6f);
        maxHp.sizeDelta = _rect.sizeDelta;
        delayHp.sizeDelta = _rect.sizeDelta;
        curHp.sizeDelta = _rect.sizeDelta;

        bShow = false;
        maxHp.gameObject.SetActive(bShow);
        delayHp.gameObject.SetActive(bShow);
        curHp.gameObject.SetActive(bShow);

        bInited = true;
    }

    private void Update()
    {
        if(!bInited)
        {
            return;
        }

        var screenPos = GameManager.instance.UIMng.GetScreenPos(attachedUnit.GetHpPos());

        _rect.anchoredPosition = screenPos;
    }

    public void HpUpdate()
    {
        bShow = true;
        StopAllCoroutines();
        StartCoroutine(HpAnimation());
    }

    IEnumerator HpAnimation()
    {
        maxHp.gameObject.SetActive(bShow);
        delayHp.gameObject.SetActive(bShow);
        curHp.gameObject.SetActive(bShow);

        curHp.sizeDelta = new Vector2(attachedUnit.GetCurHp() / 200, curHp.sizeDelta.y);

        float diff = delayHp.sizeDelta.x - curHp.sizeDelta.x;

        yield return new WaitForSeconds(0.2f);

        const float reduceTime = 0.5f;
        var timeCheck = 0.0f;
        var originalSize = delayHp.sizeDelta.x;
        while (timeCheck <= reduceTime)
        {
            var deltaTime = GameManager.instance.TimeMng.DeltaTime;
            delayHp.sizeDelta = new Vector2(originalSize - diff * timeCheck / reduceTime, delayHp.sizeDelta.y);

            timeCheck += deltaTime;

            yield return null;
        }
        delayHp.sizeDelta = curHp.sizeDelta;
    }

    public void Detach()
    {
        attachedUnit.hitEvent.RemoveListener(HpUpdate);
        attachedUnit.dieEvent.RemoveListener(Detach);

        this.gameObject.SetActive(false);
    }
}
