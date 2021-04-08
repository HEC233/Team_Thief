using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BattleIdleCtrl : MonoBehaviour
{
    private int _counter = 0;
    public event UnityAction<bool> OnIsBattleIdleEvent;

    private void CheckBattleIdle()
    {
        if (_counter >= 1)
            OnIsBattleIdleEvent?.Invoke(true);
        else
            OnIsBattleIdleEvent?.Invoke(false);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
            _counter++;
        
        CheckBattleIdle();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
            _counter--;

        CheckBattleIdle();
    }
}
