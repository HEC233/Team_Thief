using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISettingMenu : MonoBehaviour
{
    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void Toggle(bool value)
    {
        this.gameObject.SetActive(value);
    }
}
