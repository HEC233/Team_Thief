using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISkillSelectInfo : MonoBehaviour
{
    [SerializeField]
    private Image _background;
    [SerializeField]
    private Image _image;
    [SerializeField]
    private TextMeshProUGUI _skillName;
    [SerializeField]
    private TextMeshProUGUI _skillDescription;

    public void SetInfo(Sprite image, string name, string description)
    {
        _image.sprite = image;
        _skillName.text = name;
        _skillDescription.text = description;
    }

    public void SetHightlight(bool value)
    {
        _background.color = value ? Color.red : Color.white;
    }

    public void Show(bool value)
    {
        gameObject.SetActive(value);
    }
}
