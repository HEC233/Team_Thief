using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISettingMenu : MonoBehaviour, IUIFocus
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

    public void FocusWithMouse()
    {
        throw new System.NotImplementedException();
    }

    public void FocusWithKeyboard()
    {
        throw new System.NotImplementedException();
    }
}
