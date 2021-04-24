using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowWalkSkillController : SkillControllerBase
{
    public ShadowWalkSkillController(GameSkillObject skillObject, SkillDataBase data, Unit unit) : base(skillObject, data, unit) { }

    private ShadowWalkSkillData _shadowWalkSkillData;

    private PlayerUnit _unit;

    public override void Invoke()
    {
        _shadowWalkSkillData = SkillData as ShadowWalkSkillData;
        _unit = Unit as PlayerUnit;
        var transform = _unit.shadowWalkShadow.transform;
        _unit.Rigidbody2D.MovePosition(transform.position);
        
        _unit.shadowWalkShadow.ChangeControlState(_shadowWalkSkillData.ControlTime);
        CrateShadowLump();
        OnEndSkill?.Invoke();
    }

    private void CrateShadowLump()
    {
        for (int i = 0; i < _shadowWalkSkillData.ShadowLumpAmount; i++)
        {
            var shadowLump = 
                GameObject.Instantiate(_shadowWalkSkillData.ShadowLumpGameObject);
            
            shadowLump.transform.position = _unit.shadowWalkShadow.transform.position;
            shadowLump.GetComponent<ShadowLumpUnit>().Init();
        }
    }
}
