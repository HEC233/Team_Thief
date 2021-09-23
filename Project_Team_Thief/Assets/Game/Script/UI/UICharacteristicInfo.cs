using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICharacteristicInfo : MonoBehaviour
{
    [SerializeField]
    private Image _image;

    public void SetInfo(Sprite image)
    {
        _image.sprite = image;
    }
    
    
}
