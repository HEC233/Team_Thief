using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayerInfo : MonoBehaviour
{
    RectTransform _rect;

    public RectTransform currentHP;
    public RectTransform currentMP;

    public TextMeshProUGUI curHPText;
    public TextMeshProUGUI maxHPText;

    private float _maxHP;
    private float _curHP;
    private float _displayHP;

    public SOPlayer playerInfo;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        InitInfo();
        UpdateUIHP();
        playerInfo.hpChangeEvent += SetHP;
    }

    private void OnDisable()
    {
        playerInfo.hpChangeEvent -= SetHP;
    }

    public void Toggle(bool value)
    {
        this.gameObject.SetActive(value);
    }

    public void InitInfo()
    {
        _maxHP = playerInfo.MaxHP;
        _curHP = playerInfo.CurHP;
        _displayHP = _curHP;
    }

    public void SetHP()
    {
        var targetHP = playerInfo.CurHP;
        StopAllCoroutines();
        StartCoroutine(HPChangeCoroutine(targetHP));
    }

    IEnumerator HPChangeCoroutine(float targetHP)
    {
        var wait = new WaitForSeconds(0.1f);
        var diff = _curHP - targetHP;

        if(diff < 0)
        {
            _displayHP = targetHP;
            _curHP = targetHP;
            UpdateUIHP();
            yield break;
        }

        _displayHP = _curHP;
        _curHP = targetHP;
        diff /= 10;
        for (int i = 0; i < 10; i++)
        {
            _displayHP -= diff;

            UpdateUIHP();
            yield return wait;
        }

        yield break;
    }

    private void UpdateUIHP()
    {
        float ratio = _displayHP / _maxHP;
        float width = Mathf.Clamp((int)(ratio * 108.0f), 3, 108);

        var size = currentHP.sizeDelta;
        currentHP.sizeDelta = new Vector2(width, size.y);

        maxHPText.text = ((int)_maxHP).ToString();
        curHPText.text = ((int)_curHP).ToString();
    }

    private void UpdateUIMP()
    {

    }
}
