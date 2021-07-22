using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Assert = UnityEngine.Assertions.Assert;
public class SkillSpearAttackCtrl : AttackBase 
{
    public void SetSpearRush()
    {
        AttackAreaDetection();
        
        Debug.Log("Spere Is Enter : " + _isEnter);

    }
}
