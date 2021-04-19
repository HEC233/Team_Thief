using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowWalkSkillController : SkillControllerBase
{
    public ShadowWalkSkillController(GameSkillObject skillObject, SkillDataBase data) : base(skillObject, data) { }

    public override void Invoke()
    {
        Debug.Log("ShadowWalk");
    }
}
