using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSheatingController : SkillControllerBase
{
    private SkillSheatingData _skillSheatingData;
    private PlayerUnit _unit;
    private Damage _damage;
    private bool _isHit = false;
    private float _moveSpeed;
    
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
        _unit.ZoomInEvent += ZoomInEventCall;
        _unit.ZoomOutEvent += ZoomOutEventCall;
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
        SetAttackMoveValue();
        _unit.StartCoroutine(AttackMoveCoroutine());
        _unit.StartCoroutine(SpecialStateCoroutine());

    }

    private void ReceiveHitEvent()
    {
        _isHit = true;
    }
    
    private void SetAttackMoveValue()
    {
        _unit.MoveStop();

        _moveSpeed = (1 / _skillSheatingData.MoveTimes[0]) * _skillSheatingData.MoveXs[0];
        _unit.Rigidbody2D.gravityScale = 0;
    }
    
    private void AttackMove()
    {
        _unit.Rigidbody2D.velocity = Vector2.zero;

        var power = new Vector2(_moveSpeed * _unit.FacingDir * GameManager.instance.TimeMng.TimeScale, 0);
        _unit.Rigidbody2D.AddForce(power, ForceMode2D.Impulse);
    }
    
    private void EndAttackMove()
    {
        _unit.MoveStop();
        _unit.Rigidbody2D.gravityScale = _unit.OriginalGravityScale;
    }
    
    
    private void ResetValue()
    {
        _unit.OnSkillSheatingAttackEvent -= ReceiveAttackEvent;
        _isHit = false;
        _moveSpeed = 0;
    }
    
    private void EndSkill()
    {
        _unit.ZoomInEvent -= ZoomInEventCall;
        _unit.ZoomOutEvent -= ZoomOutEventCall;
        ResetValue();
        OnEndSkillAction?.Invoke();
    }
    
    private void ZoomInEventCall()
    {
        _unit.SkillSheatingAttackBase.ZoomIn();        
    }

    private void ZoomOutEventCall()
    {
        _unit.SkillSheatingAttackBase.ZoomOut();                
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
    
    IEnumerator AttackMoveCoroutine()
    {
        float timer = 0.0f;
        while (_skillSheatingData.MoveTimes[0] > timer)
        {
            timer += GameManager.instance.TimeMng.FixedDeltaTime;
            AttackMove();
            yield return new WaitForFixedUpdate();
        }

        EndAttackMove();
    }
}
