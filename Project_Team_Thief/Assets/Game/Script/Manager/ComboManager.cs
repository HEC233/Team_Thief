using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    [SerializeField]
    private float _comboTime;

    private float _comboTimer;
    private int _curCombo = 0;
    private bool _isContinuingCombo = false;
    private Coroutine _comboCoroutine;

    private void Start()
    {
        GameManager.instance.AddMapEndEventListener(OnEndMapEventCall);
    }

    public void AddCombo()
    {
        _curCombo++;

        if (_isContinuingCombo == true)
        {
            _comboTimer = _comboTime;
        }
        else
        {
            _comboCoroutine = StartCoroutine(ComboCoroutine());    
        }
    }

    private void EndComboSet()
    {
        _isContinuingCombo = false;
        GameManager.instance.EncroachmentMng.EndComboSet(_curCombo);
        _curCombo = 0;
    }

    private void OnEndMapEventCall()
    {
        if (_isContinuingCombo == true)
        {
            StopCoroutine(_comboCoroutine);
            EndComboSet();
        }
    }
    
    IEnumerator ComboCoroutine()
    {
        _isContinuingCombo = true;
        _comboTimer = _comboTime;
        GameManager.instance.EncroachmentMng.StartComboSet();

        while (_comboTimer >= 0)
        {
            _comboTimer -= GameManager.instance.TimeMng.FixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        EndComboSet();
    }
}
