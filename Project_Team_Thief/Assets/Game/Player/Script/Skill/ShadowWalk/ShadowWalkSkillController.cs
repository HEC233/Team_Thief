using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowWalkSkillController : SkillControllerBase
{
    public ShadowWalkSkillController(GameSkillObject skillObject, SkillDataBase data, Unit unit) : base(skillObject, data, unit) { }

    public override void Invoke()
    {
        PlayerUnit unit = Unit as PlayerUnit;
        var transform = unit._dumyShadow.transform;
        unit.Rigidbody2D.MovePosition(transform.position);
        
        unit._dumyShadow.ChangeControlState();
        //OnEndSkill.
    }
}
