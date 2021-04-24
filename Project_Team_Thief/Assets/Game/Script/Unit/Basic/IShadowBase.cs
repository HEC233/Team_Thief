using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IShadowBase
{
    event UnityAction OnChangeControlState;
    event UnityAction OnChangeIdleState;
    
    void ChangeControlState(float controlTime);

    void ChagneIdleState();

    void OnControlActiveEventOn(string skillname);

    IEnumerator ControlTimeCoroutine(float controlTime);
}