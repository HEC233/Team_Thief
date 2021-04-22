using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSkillController : SkillControllerBase
{
    private AttackSkillData AttackData => SkillData as AttackSkillData;
    
    // 스킬의 시전자는 생성자에서 가져온다.
    // 요컨데 할당, 해제 때 잘 해라. 
    public AttackSkillController(GameSkillObject skillObject, SkillDataBase data, Unit unit) : base(skillObject, data, unit) { }

    public override void Invoke()
    {
        SkillObject.StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        //GameManager.instance. >> player
        yield return null;
    }

    public void SetUnit(Unit unit)
    {
        
    }
}

// 그럼 Unit의 Rigidbody를 가져와서 여기서 컨트롤해도 괜찮을까?
// -> 이건 커플링인가?