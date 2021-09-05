using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPenaltyInfo : MonoBehaviour
{
    [SerializeField]
    private Image _image;
    [SerializeField]
    private TextMeshProUGUI _penaltyName;
    [SerializeField]
    private TextMeshProUGUI _penaltyDescription;
    [SerializeField]
    private TextMeshProUGUI _penaltyDuation;

    public void SetInfo(Sprite image, string name, string description, string duration)
    {
        _image.sprite = image;
        _penaltyName.text = name;
        _penaltyDescription.text = description;
        _penaltyDuation.text = duration;
    }
}
