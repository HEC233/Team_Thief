using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillSnakeSwordFlurryData", menuName = "ScriptableObject/SkillSnakeSwordFlurryData")]
public class SkillSnakeSwordFlurryData : SkillDataBase
{
    private float animationTime = 0.0f;

    public float AnimationTime
    {
        get => animationTime;
        set => animationTime = value;
    }

    public override SkillControllerBase GetSkillController(GameSkillObject skillObject, Unit unit)
    {
        return new SkillSnakeSwordFlurryController(skillObject, this, unit);
    }
}
