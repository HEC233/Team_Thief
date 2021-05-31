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
    public Image currentHPImage;
    public Image currentMPImage;

    public TextMeshProUGUI curHPText;
    public TextMeshProUGUI maxHPText;

    private float _maxHP;
    private float _curHP;
    private float _displayHP;

    private float _maxMP;
    private float _curMP;

    public SOPlayer playerInfo;
    public UICommandInfo commandInfo;

    private bool _bUseCommandInfo;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        InitInfo();
        UpdateUIHP();
        SetMP();
        playerInfo.hpChangeEvent += SetHP;
        playerInfo.encroachmentChangeEvent += SetMP;
    }

    private void OnDisable()
    {
        playerInfo.hpChangeEvent -= SetHP;
        playerInfo.encroachmentChangeEvent -= SetMP;
    }

    public void Toggle(bool value)
    {
        this.gameObject.SetActive(value);
        commandInfo.gameObject.SetActive(_bUseCommandInfo && value);
    }

    public void SetShowCommandInfo(bool value)
    {
        _bUseCommandInfo = value;
        commandInfo.gameObject.SetActive(value);
    }

    public void InitInfo()
    {
        _maxHP = playerInfo.MaxHP;
        _curHP = playerInfo.CurHP;
        _displayHP = _curHP;

        // Init MP here
        _maxMP = playerInfo.MaxEncroachment;
        _curMP = playerInfo.CurEncroachment;
    }

    public void CommandUpdate()
    {
        commandInfo.gameObject.SetActive(true);
        commandInfo.Init();
        commandInfo.gameObject.SetActive(_bUseCommandInfo);
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

    public void SetMP()
    {
        _maxMP = playerInfo.MaxEncroachment;
        _curMP = playerInfo.CurEncroachment;

        float ratio = Mathf.Clamp01(_curMP / _maxMP);
        float width = 0;
        if ((int)(ratio * 73.0f) > 2)
            width = Mathf.Clamp((int)(ratio * 73.0f), 2, 73);

        currentMP.sizeDelta = new Vector2(width, currentMP.sizeDelta.y);
        //currentMP.fillAmount = ratio;
    }

    private void UpdateUIHP()
    {
        float ratio = _displayHP / _maxHP;
        float width = Mathf.Clamp((int)(ratio * 108.0f), 3, 108);

        var size = currentHP.sizeDelta;
        currentHP.sizeDelta = new Vector2(width, size.y);
        //currentHP.fillAmount = ratio;

        maxHPText.text = ((int)_maxHP).ToString();
        curHPText.text = ((int)_curHP).ToString();
    }
}
