using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDynamic : MonoBehaviour
{
    public GameObject monsterHP;
    public GameObject damageText;
    RectTransform _rect;
    Camera _camera;

    private List<UIHpReduceInfo> damageTexts = new List<UIHpReduceInfo>();
    private List<UIHpMonster> monsterHps = new List<UIHpMonster>();

    public void Init()
    {
        _camera = Camera.main;
    }

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        Init();
    }

    public void Toggle(bool value)
    {
        this.gameObject.SetActive(value);
        if (value) Init();
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
        returnValue.camera = Camera.main;

        return returnValue;
    }

    public void ShowDamageText(Vector3 position, int damageCount, bool isFromRight, bool critical)
    {
        var screenPos = _camera.WorldToScreenPoint(position);
        screenPos = new Vector2(screenPos.x / Screen.width * 480, screenPos.y / Screen.height * 270);

        GetDamageText().Show(screenPos, damageCount, isFromRight, critical);
    }
}
