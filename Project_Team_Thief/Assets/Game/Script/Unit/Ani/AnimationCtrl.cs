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
    Hit,
    Contact,
    DoubleJump,
    Attack2,
    Attack3,
    JumpAttack,
    Skll1,
    Backstep,
}

public class AnimationCtrl : MonoBehaviour
{
    [SerializeField]
    private Animator _animator = null;

    public Animator Animator => _animator;

    public void PlayAni(AniState aniState)
    {
        Animator.SetInteger("State", (int) aniState);
    }

    public float GetCurAniTime()
    {
        return Animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
    }
}
