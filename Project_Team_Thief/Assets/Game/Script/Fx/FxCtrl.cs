using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FxAniEnum
{
    JumpFx,
}

public class FxCtrl : MonoBehaviour
{
    [SerializeField]
    private Animator _fxAnimator;
    
    public void PlayAni(FxAniEnum fxAniEnum)
    {
        _fxAnimator.SetInteger("State", (int) fxAniEnum);
    }

    public void PlayParticle(FxAniEnum fxAniEnum)
    {
        
    }
}
