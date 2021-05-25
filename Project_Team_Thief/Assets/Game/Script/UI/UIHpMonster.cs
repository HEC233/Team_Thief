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
    public Camera camera;

    private bool show = false;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
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

        show = false;
        maxHp.gameObject.SetActive(show);
        delayHp.gameObject.SetActive(show);
        curHp.gameObject.SetActive(show);
    }

    private void Update()
    {
        var screenPos = camera.WorldToScreenPoint(attachedUnit.GetHpPos());
        screenPos = new Vector2(screenPos.x / Screen.width * 480, screenPos.y / Screen.height * 270);

        _rect.anchoredPosition = screenPos;
    }

    public void HpUpdate()
    {
        show = true;
        StopAllCoroutines();
        StartCoroutine(HpAnimation());
    }

    IEnumerator HpAnimation()
    {
        maxHp.gameObject.SetActive(show);
        delayHp.gameObject.SetActive(show);
        curHp.gameObject.SetActive(show);

        curHp.sizeDelta = new Vector2(attachedUnit.GetCurHp() / 200, curHp.sizeDelta.y);

        float diff = delayHp.sizeDelta.x - curHp.sizeDelta.x;

        yield return new WaitForSeconds(0.2f);

        const float reduceTime = 0.5f;
        var timeCheck = 0.0f;
        var originalSize = delayHp.sizeDelta.x;
        while (timeCheck <= reduceTime)
        {
            var deltaTime = GameManager.instance.timeMng.DeltaTime;
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
