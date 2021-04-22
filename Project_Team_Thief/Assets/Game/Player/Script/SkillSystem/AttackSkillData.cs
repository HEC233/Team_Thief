using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSkillData : SkillDataBase
{
    [SerializeField]
    private float _damagePer = 1.0f;
    public float DamagePer => _damagePer;

    public override SkillControllerBase GetSkillController(GameSkillObject skillObject, Unit unit)
    {
        return new AttackSkillController(skillObject, this, unit);
    }
}
