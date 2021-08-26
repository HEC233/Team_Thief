using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSnakeSwordStingController : SkillControllerBase
{
    private SkillSnakeSwordStingData _skillSnakeSwordStingData;
    private PlayerUnit _unit;
    private Damage _damage;
    private int _index;

    public SkillSnakeSwordStingController(GameSkillObject skillObject, SkillDataBase data, Unit unit) : base(skillObject, data, unit) { }

    public override void Invoke()
    {
        Init();
    }

    private void Init()
    {
        _skillSnakeSwordStingData = SkillData as SkillSnakeSwordStingData;
        _unit = Unit as PlayerUnit;
        FindIndex();
        _unit.OnSkillSnakeSwordStingAttackEvent += ReceiveAttackEvent;
        SetDamage();
        
        _unit.SkillSnakeSwordStingAttackBase[_index].Init(_damage, _skillSnakeSwordStingData.CinemachineSignalSource);
    }
    
    private void SetDamage()
    {
        _damage = new Damage();
        _damage.power = _unit.CalcSkillDamage(_skillSnakeSwordStingData.Damages[0]);
        _damage.knockBack = new Vector2(_skillSnakeSwordStingData.KnockBackXs[0], _skillSnakeSwordStingData.KnockBackYs[0]);
        _damage.additionalInfo = _index;
        // 넉백 타임은?
        _damage.stiffness = _skillSnakeSwordStingData.Stiffness;
    }
    
    private void FindIndex()
    {
        if (_skillSnakeSwordStingData.ID == 5)
        {
            _index = 0;
        }
        else
        {
            _index = 1;
        }
        
    }
    
    private void ReceiveAttackEvent()
    {
        Progress();
    }

    private void Progress()
    {
        _unit.SkillSnakeSwordStingAttackBase[_index].Progress();
        EndSkill();
    }

    private void ResetValue()
    {
        _index = 0;
    }
    
    private void EndSkill()
    {
        ResetValue();
        _unit.OnSkillSnakeSwordStingAttackEvent -= ReceiveAttackEvent;
        OnEndSkillAction?.Invoke();
    }
}