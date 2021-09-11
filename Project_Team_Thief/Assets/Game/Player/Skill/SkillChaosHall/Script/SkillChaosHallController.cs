using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillChaosHallController : SkillControllerBase
{
    public SkillChaosHallController(GameSkillObject skillObject, SkillDataBase data, Unit unit) : base(skillObject, data, unit)
    {
    }

    public override void Invoke()
    {
        base.Invoke();
        
        throw new System.NotImplementedException();
    }
}
