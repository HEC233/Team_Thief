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

// 그럼 Unit의 Rigidbody를 가져와서 여기서 컨트롤해도 괜찮을까?
// -> 이건 커플링인가?