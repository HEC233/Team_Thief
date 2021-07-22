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
        _skillHammerData = SkillData as SkillHammerData;
        _unit = Unit as PlayerUnit;

        _damage = new Damage();
        _damage.power = _skillHammerData.AttackDamage * _unit.GetDamageWeightFromEencroachment();
        _damage.knockBack = new Vector2(_skillHammerData.KnockBackPower.x * _unit.FacingDir,
            _skillHammerData.KnockBackPower.y);
        _damage.additionalInfo = 4;

        _unit._skillHammerAttackCtrl.Init(_damage, _skillHammerData.CinemachineSignalSource);

        _unit.OnSkillHammerAttackEvent += AttackSkillHammer;
    }

    public override void Release()
    {
        base.Release();
        
        _unit.OnSkillHammerAttackEvent -= AttackSkillHammer;
    }

    public void AttackSkillHammer()
    {
        _unit.SkillHammerAttack(_damage);
        
        OnEndSkillAction?.Invoke();
    }
}
