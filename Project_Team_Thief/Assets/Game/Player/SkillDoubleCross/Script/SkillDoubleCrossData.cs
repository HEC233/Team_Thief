using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillDoubleCross", menuName = "ScriptableObject/SkillDoubleCrossData")]
public class SkillDoubleCrossData : SkillDataBase
{
    public override SkillControllerBase GetSkillController(GameSkillObject skillObject, Unit unit)
    {
        return new SkillDoubleCrossController(skillObject, this, unit);
    }
}
