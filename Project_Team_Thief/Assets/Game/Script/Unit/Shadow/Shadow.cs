using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ShadowBase : MonoBehaviour, IShadowBase
{
    protected UnityAction OnChangeControlAction = null;
    public event UnityAction OnChangeControlEvent
    {
        add => OnChangeControlAction += value;
        remove => OnChangeControlAction -= value;
    }

    protected UnityAction OnChangeIdleAction = null;
    public event UnityAction OnChangeIdleEvent
    {
        add => OnChangeIdleAction += value;
        remove => OnChangeIdleAction -= value;
    }
    
    public bool isControlState = false;

    public abstract void ChangeControlState(float controlTime);

    public abstract void ChagneIdleState();

    public abstract void OnControlActiveEventOn(string skillname);

    public abstract IEnumerator ControlTimeCoroutine(float controlTime);
}

public class Shadow : ShadowBase
{
    [SerializeField]
    private AnimationCtrl _animationCtrl;


    public void Init()
    {
        
    }

    public override void ChangeControlState(float controlTime)
    {
        GameManager.instance.ShadowControlManager.OnControlActive += OnControlActiveEventOn;
        isControlState = true;
        OnChangeControlAction?.Invoke();
        
        _animationCtrl.PlayAni(AniState.ShadowControl);
        StartCoroutine(ControlTimeCoroutine(controlTime));
    }

    public override void ChagneIdleState()
    {
        GameManager.instance.ShadowControlManager.OnControlActive -= OnControlActiveEventOn;
        isControlState = false;
        OnChangeIdleAction?.Invoke();
        
        _animationCtrl.PlayAni(AniState.Idle);
    }

    public override void OnControlActiveEventOn(string skillname)
    {
        
    }

    public override IEnumerator ControlTimeCoroutine(float controlTime)
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
