using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Shadow : MonoBehaviour, IShadowBase
{
    public event UnityAction OnChangeControlState;
    public event UnityAction OnChangeIdleState;
    
    [SerializeField]
    private AnimationCtrl _animationCtrl;


    public void Init()
    {
        
    }

    public void ChangeControlState(float controlTime)
    {
        GameManager.instance.ShadowControlManager.OnControlActive += OnControlActiveEventOn;
        OnChangeControlState?.Invoke();
        
        _animationCtrl.PlayAni(AniState.ShadowControl);
        StartCoroutine(ControlTimeCoroutine(controlTime));
    }

    public void ChagneIdleState()
    {
        GameManager.instance.ShadowControlManager.OnControlActive -= OnControlActiveEventOn;
        _animationCtrl.PlayAni(AniState.Idle);
    }

    public void OnControlActiveEventOn(string skillname)
    {
    }

    public IEnumerator ControlTimeCoroutine(float controlTime)
    {
        float _timer = 0.0f;
        
        while (_timer < controlTime)
        {
            _timer += GameManager.instance.timeMng.FixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        ChagneIdleState();
    }
}
