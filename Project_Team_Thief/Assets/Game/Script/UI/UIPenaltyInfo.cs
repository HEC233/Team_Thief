using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPenaltyInfo : MonoBehaviour
{
    [SerializeField]
    private Sprite _image;
    [SerializeField]
    private string _penaltyName;
    [SerializeField]
    private string _penaltyDescription;

    public void SetInfo(Sprite image, string name, string description)
    {
        _image = image;
        _penaltyName = name;
        _penaltyDescription = description;
    }
}
