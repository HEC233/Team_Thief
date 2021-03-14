using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AniState
{
    Idle = 0,
    Move,
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
}
