using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSkillController : SkillControllerBase
{
    private AttackSkillData AttackData => SkillData as AttackSkillData;
    public AttackSkillController(GameSkillObject skillObject, SkillDataBase data) : base(skillObject, data) { }

    public override void Invoke()
    {
        SkillObject.StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        yield return null;
    }
}
