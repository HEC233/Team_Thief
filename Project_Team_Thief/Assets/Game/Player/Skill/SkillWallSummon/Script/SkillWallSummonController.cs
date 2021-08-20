using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillWallSummonController : SkillControllerBase
{
    private SkillWallSummonData _skillWallSummonData;
    private PlayerUnit _unit;
    private Damage _damage;

    public SkillWallSummonController(GameSkillObject skillObject, SkillDataBase data, Unit unit) : base(skillObject, data, unit) { }

    public override void Invoke()
    {
        
    }
}
