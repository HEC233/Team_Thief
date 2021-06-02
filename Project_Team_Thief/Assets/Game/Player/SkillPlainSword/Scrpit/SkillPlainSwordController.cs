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
                _skillPlainSwordData.KnockBackPowerArr[_unit.skillKopshIndex].x * (_unit.FacingDir * -1),
                _skillPlainSwordData.KnockBackPowerArr[_unit.skillPlainSwordIndex].y);
        }
        else
        {
            _damage.knockBack = new Vector2(
                _skillPlainSwordData.KnockBackPowerArr[_unit.skillKopshIndex].x * _unit.FacingDir,
                _skillPlainSwordData.KnockBackPowerArr[_unit.skillPlainSwordIndex].y);
        }

        _damage.additionalInfo = _unit.skillPlainSwordIndex;

        _attackInterval = _skillPlainSwordData.MultiStateHitInterval;
        
        _unit.OnSkillPlainSwordAttackEvent += AttackSkillPlainSword;
        _unit.OnSkillPlainSwordFastEvent += OnSkillPlainSwordFastEventCall;
        _unit.OnSkillPlainSwordEndEvent += OnSkillPlainSwordEndEventCall;
    }
    
    public override void Release()
    {
        base.Release();

        _unit.OnSkillKopshAttackEvent -= AttackSkillPlainSword;
    }

    private void AttackSkillPlainSword()
    {
        if (_unit.skillPlainSwordIndex == _unit._SkillPlainSwordAttackCtrls.Length - 1)
        {
            _multiHitCoroutine = _unit.StartCoroutine(MultiHitAttackCoroutine());
        }
        else
        {
            _unit.SkillPlainSwordAttack(_damage);
        }
    }

    private void OnSkillPlainSwordFastEventCall()
    {
        _attackInterval *= _skillPlainSwordData.MultiStateHitIntervalFastAmount;
    }

    private void OnSkillPlainSwordEndEventCall()
    {
        _isEnd = true;
        _unit.StopCoroutine(MultiHitAttackCoroutine());
    }
    

    IEnumerator MultiHitAttackCoroutine()
    {
        float timer = 0.2f;
        while (!_isEnd)
        {
            timer += GameManager.instance.timeMng.FixedDeltaTime;

            if (timer >= _attackInterval)
            {
                _unit.SkillPlainSwordAttack(_damage);
                timer = 0.0f;
            }
            
            yield return new WaitForFixedUpdate();
        }
        
        OnEndSkillAction?.Invoke();
    }
    
}
