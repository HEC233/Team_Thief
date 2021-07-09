using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShadowUnit : Unit, IShadowBase
{
    public event UnityAction OnChangeControlState;
    public event UnityAction OnChangeIdleState;
    public void ChangeControlState(float controlTime)
    {
        throw new System.NotImplementedException();
    }

    public void ChagneIdleState()
    {
        throw new System.NotImplementedException();
    }

    public void OnControlActiveEventOn(string skillname)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerator ControlTimeCoroutine(float controlTime)
    {
        throw new System.NotImplementedException();
    }

    public void ChangeControlState()
    {
        OnChangeControlState?.Invoke();
    }
}
