using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shadow : MonoBehaviour, IShadowBase
{
    public event UnityAction OnChangeControlState;

    public void ChangeControlState()
    {
        throw new System.NotImplementedException();
    }
}
