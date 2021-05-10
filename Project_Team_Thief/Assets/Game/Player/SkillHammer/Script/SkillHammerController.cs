using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillHammerController : SkillControllerBase
{
    public SkillHammerController(GameSkillObject skillObject, SkillDataBase data, Unit unit) : base(skillObject, data, unit) { }

    private SkillHammerData _skillHammerData;

    private PlayerUnit _unit;

    private Damage _damage;
    
    public override void Invoke()
    {
        Init();
    }

    private void Init()
    {
        
    }
}
