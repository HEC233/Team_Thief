using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PS.FX;

public enum FxAniEnum
{
    Idle = 0,
    JumpFx,
    DashFx,
    BasicAttack,
    BasicAttack2,
    BasicAttack3,
    BasicJumpAttack,
    BasicAttack4,
    SkillAxe,
    SkillAxe2,
    SkillSpear,     // 10
    SkillHammer,    // 11
    JumpAttackFx2,  // 12
    SkillKopsh,     // 13
    SkillKopsh2,    // 14
    SkillKopsh3,    // 15
    SkillPlainSword,    // 16
    SkillPlainSword2,   // 17
    SkillSnakeSwordFulrry,   // 18
    SkillBaldoCast,     // 19
    SkillBaldoDelay,    // 20
    SkillBaldo,         // 21
    SkillSheatingCast,  // 22
    SkillSheatingDelay, // 23
    SkillSheating,      // 24
    SkillMagicMissile,  // 25
    SkillChaosHallCast, // 26
    SkillChaosHallDelay,// 27
    SkillChaosHall,     // 28
    SkillDoubleCross,   // 29
    SkillDoubleCross2,  // 30
    SkillSnakeSwordSting,   // 31
    SkillSnakeSwordSting2,  // 32

}

public class FxCtrl : MonoBehaviour
{
    [SerializeField]
    private Animator _fxAnimator;

    [SerializeField] 
    private EffectSystem _effectSystem;
    
    public void PlayAni(FxAniEnum fxAniEnum)
    {
        _fxAnimator.Rebind();
        _fxAnimator.SetInteger("State", (int) fxAniEnum);
    }

    public void PlayParticle(FxAniEnum fxAniEnum, float dir = 1)
    {
        Quaternion _quaternion = Quaternion.identity;
        if (dir == -1)
            _quaternion = Quaternion.Euler(0, -180, 0);
        
        GameManager.instance.FX.Play(fxAniEnum.ToString(), transform.position, _quaternion);
    }

    public void SetAnimationTimeSclae(float timeScale)
    {
        _fxAnimator.speed = timeScale;
    }

    public void OnAnimationEndEvent()
    {
        PlayAni(FxAniEnum.Idle);
    }
    
    public void SetSpeed(float speed)
    {
        _fxAnimator.speed = speed;
    }

}
