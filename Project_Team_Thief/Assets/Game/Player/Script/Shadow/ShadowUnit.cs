using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShadowUnit : Unit, IShadowBase
{
    public event UnityAction OnChangeControlState;
    
    public void ChangeControlState()
    {
        OnChangeControlState?.Invoke();
    }
}
