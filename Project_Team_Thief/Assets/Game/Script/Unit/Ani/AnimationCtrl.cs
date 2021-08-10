using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AniState
{
    Idle = 0,
    Move,           // 1
    RunningInertia, // 2
    Jump,           // 3
    Fall,           // 4
    Dash,           // 5
    Wallslideing,   // 6
    Attack,         // 7
    Attack2,        // 8
    Attack3,        // 9
    JumpAttack,     // 10
    Hit,            // 11
    Contact,        // 12
    DoubleJump,     // 13
    Skll1,          // 14
    Backstep,       // 15
    Die,            // 16
    AttackReady,    // 17
    Hit2,           // 18
    BattleIdle,     // 19
    Attack4,        // 20
    SkillAxe,       // 21
    SkillAxe2,      // 22
    SkillSpear,     // 23
    SkillHammer,    // 24
    JumpAttack2,    // 25
    SkillKopsh,     // 26
    SkillKopsh2,    // 27
    SkillKopsh3,    // 28
    SkillPlainSword,    // 29
    SkillPlainSword2,   // 30
    SkillPlainSword3,   // 31
    SkillDoubleCross,   // 32
    SkillDoubleCross2,  // 33

    // SkillShadowWalk,// 20
    // ShadowControl,  // 21
    // ShadowLumpSpawn,// 22
    // ShadowLumpHit,  // 23
    // ShadowLumpDie,  // 24
}

public class AnimationCtrl : MonoBehaviour
{
    [SerializeField]
    private Animator _animator = null;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    //public Animator Animator => _animator;

    public void PlayAni(AniState aniState)
    {
        if (_animator.GetInteger("State") == (int)aniState)
            return;
        
        _animator.Rebind();
        _animator.SetInteger("State", (int) aniState);
    }

    public float GetCurAniTime()
    {
        return _animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;
    }

    public int GetCurrentPlayAni()
    {
        return _animator.GetInteger("State");
    }

    public void Flip()
    {
        _spriteRenderer.flipX = !_spriteRenderer.flipX;
    }

    public void SetFlip(bool value)
    {
        _spriteRenderer.flipX = value;
    }

    public void SetSpeed(float speed)
    {
        _animator.speed = speed;
    }
    public void SetAnimationTimeSclae(float timeScale)
    {
        _animator.speed = timeScale;
    }

    public void SetColor(Color color)
    {
        _spriteRenderer.color = color;
    }

    public void SetOnOffSpriteRenderer(bool active)
    {
        _spriteRenderer.enabled = active;
    }
}
