using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AniState
{
    Idle = 0,
    Move,
    RunningInertia,
    Jump,
    Fall,
    Roll,
    Wallslideing,
    Attack,
    Attack2,
    Attack3,
    JumpAttack,
    Hit,
    Contact,
    DoubleJump,
    Skll1,
    Backstep,
    Dash,
    Die,
    AttackReady,
}

public class AnimationCtrl : MonoBehaviour
{
    [SerializeField]
    private Animator _animator = null;
    [SerializeField] private SpriteRenderer spriteRenderer;

    //public Animator Animator => _animator;

    public void PlayAni(AniState aniState)
    {
        _animator.SetInteger("State", (int) aniState);
    }

    public float GetCurAniTime()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
    }

    public void Flip()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    public void SetFlip(bool value)
    {
        spriteRenderer.flipX = value;
    }

    public void SetSpeed(float speed)
    {
        _animator.speed = speed;
    }
    public void SetAnimationTimeSclae(float timeScale)
    {
        _animator.speed = timeScale;
    }
}
