using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBaldoController : SkillControllerBase
{
    private SkillBaldoData _skillBaldoData;
    private PlayerUnit _unit;
    private Damage _damage;

    public SkillBaldoController(GameSkillObject skillObject, SkillDataBase data, Unit unit) : base(skillObject, data, unit) { }

    public override void Invoke()
    {
        Init();
    }

    private void Init()
    {
        _skillBaldoData = SkillData as SkillBaldoData;
        _unit = Unit as PlayerUnit;
        SetDamage();
        _unit.SkillBaldoAttackBase.Init(_damage, _skillBaldoData.CinemachineSignalSource);
        _unit.OnSkillBaldoAttackEvent += ReceiveAttackEvent;
    }
    
    private void SetDamage()
    {
        _damage = new Damage();
        _damage.power =
            _unit.CalcSkillDamage(_skillBaldoData.Damages[0]);
        _damage.knockBack = new Vector2(_skillBaldoData.KnockBackXs[0], _skillBaldoData.KnockBackYs[0]) *
                            _unit.FacingDir;
        _damage.additionalInfo = 0;
        _damage.stiffness = _skillBaldoData.Stiffness;
    }
    
    private void ReceiveAttackEvent()
    {
        Progress();
    }
    
    private void Progress()
    {
        _unit.SkillBaldoAttackBase.Progress();
        _unit.StartCoroutine(SpecialStateCoroutine());
    }
    
    private void ResetValue()
    {
        _unit.OnSkillBaldoAttackEvent -= ReceiveAttackEvent;
    }
    
    private void EndSkill()
    {
        ResetValue();
        OnEndSkillAction?.Invoke();
    }

    private IEnumerator SpecialStateCoroutine()
    {
        float timer = 0.0f;
        
        _unit.ChangeCritical(_skillBaldoData.AddCriticalAmount);
        
        while (_skillBaldoData.AddCriticalTime >= timer)
        {
            timer += GameManager.instance.TimeMng.FixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        _unit.ChangeCritical(-_skillBaldoData.AddCriticalAmount);
        EndSkill();
    }
}
