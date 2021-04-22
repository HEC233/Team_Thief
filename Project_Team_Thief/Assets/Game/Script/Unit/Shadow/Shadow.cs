using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shadow : MonoBehaviour, IShadowBase
{
    public event UnityAction OnChangeControlState;
    
    [SerializeField]
    private AnimationCtrl _animationCtrl;
    
    

    public void ChangeControlState()
    {
        OnChangeControlState?.Invoke();
        
        _animationCtrl.PlayAni(AniState.ShadowControl);
    }
}
