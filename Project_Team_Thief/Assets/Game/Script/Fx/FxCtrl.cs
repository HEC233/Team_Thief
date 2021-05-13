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
    SkillHammer,    
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

}
