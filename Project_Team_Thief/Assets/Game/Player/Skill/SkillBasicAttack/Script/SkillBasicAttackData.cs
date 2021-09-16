using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillBasicAttackData", menuName = "ScriptableObject/SkillData/SkillBasicAttackData")]

public class SkillBasicAttackData : SkillDataBase
{
    public override SkillControllerBase GetSkillController(GameSkillObject skillObject, Unit unit)
    {
        throw new System.NotImplementedException();
    }
}
