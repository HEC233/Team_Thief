using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSheatingController : SkillControllerBase
{
    private SkillSheatingData _skillSheatingData;
    private PlayerUnit _unit;
    private Damage _damage;
    private bool _isHit = false;
    
    public SkillSheatingController(GameSkillObject skillObject, SkillDataBase data, Unit unit) : base(skillObject, data, unit) { }

 public override void Invoke()
    {
        Init();
    }

    private void Init()
    {
        _skillSheatingData = SkillData as SkillSheatingData;
        _unit = Unit as PlayerUnit;
        SetDamage();
        _unit.SkillSheatingAttackBase.Init(_damage, _skillSheatingData.CinemachineSignalSource);
        _unit.OnSkillSheatingAttackEvent += ReceiveAttackEvent;
        ((SkillBaldoAttackCtrl)_unit.SkillSheatingAttackBase).OnEnemyHitEvent += ReceiveHitEvent;
    }
    
    private void SetDamage()
    {
        _damage = new Damage();
        _damage.power =
            _unit.CalcSkillDamage(_skillSheatingData.Damages[0]);
        _damage.knockBack = new Vector2(_skillSheatingData.KnockBackXs[0], _skillSheatingData.KnockBackYs[0]) *
                            _unit.FacingDir;
        _damage.additionalInfo = 0;
        _damage.stiffness = _skillSheatingData.Stiffness;
    }
    
    private void ReceiveAttackEvent()
    {
        Progress();
    }
    
    private void Progress()
    {
        _unit.SkillSheatingAttackBase.Progress();
        _unit.StartCoroutine(SpecialStateCoroutine());

    }

    private void ReceiveHitEvent()
    {
        _isHit = true;
    }
    
    
    private void ResetValue()
    {
        _unit.OnSkillSheatingAttackEvent -= ReceiveAttackEvent;
        _isHit = false;
    }
    
    private void EndSkill()
    {
        ResetValue();
        OnEndSkillAction?.Invoke();
    }

    private IEnumerator SpecialStateCoroutine()
    {
        float timer = 0.0f;
        bool _isFrist = true;
        
        while (_skillSheatingData.AddCriticalTime >= timer)
        {
            if (_isHit == true && _isFrist == true)
            {
                timer = 0;
                _unit.ChangeCritical(_skillSheatingData.AddCriticalAmount);
                _isFrist = false;
            }
            
            timer += GameManager.instance.TimeMng.FixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (_isHit == true)
        {
            _unit.ChangeCritical(-_skillSheatingData.AddCriticalAmount);
        }

        EndSkill();
    }
}
