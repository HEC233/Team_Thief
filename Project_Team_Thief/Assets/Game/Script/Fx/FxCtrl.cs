using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PS.FX;

public enum FxAniEnum
{
    SlidingFx,
    JumpFx,
}

public class FxCtrl : MonoBehaviour
{
    [SerializeField]
    private Animator _fxAnimator;

    [SerializeField] 
    private EffectSystem _effectSystem;
    
    public void PlayAni(FxAniEnum fxAniEnum)
    {
        _fxAnimator.SetInteger("State", (int) fxAniEnum);
    }

    public void PlayParticle(FxAniEnum fxAniEnum)
    {
        _effectSystem.Play(fxAniEnum.ToString(), transform.position);
    }
}
