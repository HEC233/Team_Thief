using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillSnakeSwordStingData", menuName = "ScriptableObject/SkillData/SkillSnakeSwordStingData")]
public class SkillSnakeSwordStingData : SkillDataBase
{
    public override SkillControllerBase GetSkillController(GameSkillObject skillObject, Unit unit)
    {
        return new SkillSnakeSwordStingController(skillObject, this, unit);
    }
}
