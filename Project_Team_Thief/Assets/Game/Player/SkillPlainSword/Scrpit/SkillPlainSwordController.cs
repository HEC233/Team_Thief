using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillPlainSwordController : SkillControllerBase
{
    public SkillPlainSwordController(GameSkillObject skillObject, SkillDataBase data, Unit unit) : base(skillObject, data, unit) { }

    private SkillPlainSwordData _skillPlainSwordData;
    private PlayerUnit _unit;
    private Damage _damage;
    private Coroutine _multiHitCoroutine;

    private float _attackInterval = 0.0f;
    private bool _isEnd = false;

    public override void Invoke()
    {
        Init();
    }

    private void Init()
    {
        _skillPlainSwordData = SkillData as SkillPlainSwordData;
        _unit = Unit as PlayerUnit;

        _damage = new Damage();
        _damage.power = _skillPlainSwordData.AttackDamageArr[_unit.skillPlainSwordIndex] *
                        _unit.EncroachmentPerPlayerAttackDamage;
        
        if (_unit.skillPlainSwordIndex == 1)
        {
            _damage.knockBack = new Vector2(
                _skillPlainSwordData.KnockBackPowerArr[_unit.skillPlainSwordIndex].x * (_unit.FacingDir * -1),
                _skillPlainSwordData.KnockBackPowerArr[_unit.skillPlainSwordIndex].y);
            
        }
        else
        {
            _damage.knockBack = new Vector2(
                _skillPlainSwordData.KnockBackPowerArr[_unit.skillPlainSwordIndex].x * _unit.FacingDir,
                _skillPlainSwordData.KnockBackPowerArr[_unit.skillPlainSwordIndex].y);
        }

        _damage.additionalInfo = _unit.skillPlainSwordIndex;
        _unit._SkillPlainSwordAttackCtrls[_unit.skillPlainSwordIndex]._cinemachineSignalSource =
            _skillPlainSwordData.CinemachineSignalSourceArr[_unit.skillPlainSwordIndex];
        
        _attackInterval = _skillPlainSwordData.MultiStateHitInterval;
        
        _unit.OnSkillPlainSwordAttackEvent += AttackSkillPlainSword;
    }
    
    public override void Release()
    {
        base.Release();

        _unit.OnSkillPlainSwordAttackEvent -= AttackSkillPlainSword;
    }

    private void AttackSkillPlainSword()
    {
        if (_unit.skillPlainSwordIndex == _unit._SkillPlainSwordAttackCtrls.Length - 1)
        {
            _unit.SkillPlainSwordMultiAttack(_damage);
        }
        else
        {
            _unit.SkillPlainSwordAttack(_damage);
        }
        
        OnEndSkillAction?.Invoke();
    }



    
}
