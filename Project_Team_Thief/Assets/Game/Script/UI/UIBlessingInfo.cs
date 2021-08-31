using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIBlessingInfo : MonoBehaviour
{
    private Color[] _color = new Color[2] { new Color(1, 0.2156863f, 0.2745098f), new Color(0.4392157f, 0.1882353f, 0.627451f) };
    private EncroachmentManager _encroMng;

    [SerializeField]
    private Image _gaugebar;
    [SerializeField]
    private Image[] _soulImages = new Image[5];


    private void Start()
    {
        _encroMng = GameManager.instance.EncroachmentMng;
    }

    private void FixedUpdate()
    {
        /*
         * if 문은 cpu클럭에 좋지 않아(분기예측, 분기에 따른 클럭소모등) if문을 제거하기 위해 변경된 코드, 아래와 동일
         * float encroament = 0.0f;
         * if(_encroMng.Encroachment > 0)
         * {
         *      encroament = _encroMng.Encroachment;
         *      _gaugebar.color = _color[1];
         * }
         * else
         * {
         *      encroament = -_encroMng.Encroachment;
         *      _gaugebar.color = _color[1];
         * }
         */
        int result = Convert.ToInt32(_encroMng.Encroachment > 0);
        float encroament = _encroMng.Encroachment + 2 * (result - 1) * _encroMng.Encroachment;
        _gaugebar.color = _color[result];

        _gaugebar.fillAmount = encroament / 100;

        for (int i = 0; i < 5; i++)
        {
            _soulImages[i].gameObject.SetActive((5 - i) > _encroMng.EncroachmentNumber);
        }
    }
}
