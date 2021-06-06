using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDynamic : MonoBehaviour
{
    public GameObject monsterHP;
    public GameObject damageText;
    RectTransform _rect;
    [SerializeField]
    RectTransform ChainButton;

    private List<UIHpReduceInfo> damageTexts = new List<UIHpReduceInfo>();
    private List<UIHpMonster> monsterHps = new List<UIHpMonster>();

    public void Init()
    {
        foreach (var d in damageTexts)
        {
            d.gameObject.SetActive(false);
        }
        foreach(var m in monsterHps)
        {
            m.gameObject.SetActive(false);
            m.SetUninit();
        }
        this.gameObject.SetActive(true);
    }

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void Update()
    {
        if(ChainButton.gameObject.activeSelf)
        {
            ChainButton.anchoredPosition = GameManager.instance.uiMng.GetScreenPos(GameManager.instance.GetControlActor().GetUnit().transform.position + Vector3.up * 1.5f);
        }
    }

    public void Toggle(bool value)
    {
        foreach (var d in damageTexts)
        {
            d.SetVisible(value);
        }
    }

    public void TurnChangButton(bool value)
    {
        ChainButton.gameObject.SetActive(value);
    }

    private UIHpReduceInfo GetDamageText()
    {
        UIHpReduceInfo returnValue = null;
        foreach(var d in damageTexts)
        {
            if (!d.gameObject.activeSelf)
            {
                returnValue = d;
                break;
            }
        }
        if(returnValue == null)
        {
            var go = GameObject.Instantiate(damageText, this.gameObject.transform);
            returnValue = go.GetComponent<UIHpReduceInfo>();
            damageTexts.Add(returnValue);
        }
        return returnValue;
    }

    public UIHpMonster GetMonsterHP()
    {
        UIHpMonster returnValue = null;
        foreach (var m in monsterHps)
        {
            if (!m.gameObject.activeSelf)
            {
                returnValue = m;
                break;
            }
        }
        if (returnValue == null)
        {
            var go = GameObject.Instantiate(monsterHP, this.gameObject.transform);
            returnValue = go.GetComponent<UIHpMonster>();
            monsterHps.Add(returnValue);
        }
        returnValue.gameObject.SetActive(true);

        return returnValue;
    }

    public void ShowDamageText(Vector3 position, int damageCount, bool isFromRight, bool critical)
    {
        var screenPos = GameManager.instance.uiMng.GetScreenPos(position);

        GetDamageText().Show(screenPos, damageCount, isFromRight, critical);
    }
}
