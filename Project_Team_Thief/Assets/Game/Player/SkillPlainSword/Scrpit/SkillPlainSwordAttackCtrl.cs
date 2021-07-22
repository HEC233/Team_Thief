using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;
using Cinemachine;

public class SkillPlainSwordAttackCtrl : AttackBase
{
    [SerializeField] 
    private bool _stopSound = false;
    public override void AttackEnd()
    {
        base.AttackEnd();

        if (_stopSound == true)
        {
            WwiseSoundManager.instance.StopEventSoundFromId(_sfxSoundId);
        }
    }
}
