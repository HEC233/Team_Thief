using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBaldoController : SkillControllerBase
{
    private SkillBaldoData _skillBaldoData;
    private PlayerUnit _unit;
    private Damage _damage;
    private bool _isHit = false;
    private float _moveSpeed;

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
        _unit.SkillBaldoAttackBase.Init(_damage, _skillBaldoData.CinemachineSignalSource, _skillBaldoData.ZoomInOutData);
        _unit.ZoomInEvent += ZoomInEventCall;
        _unit.ZoomOutEvent += ZoomOutEventCall;
        _unit.OnSkillBaldoAttackEvent += ReceiveAttackEvent;
        ((SkillBaldoAttackCtrl)_unit.SkillBaldoAttackBase).OnEnemyHitEvent += ReceiveHitEvent;
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

        _moveSpeed = (1 / _skillBaldoData.MoveTimes[0]) * _skillBaldoData.MoveXs[0];
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
        _unit.OnSkillBaldoAttackEvent -= ReceiveAttackEvent;
        _unit.ZoomInEvent -= ZoomInEventCall;
        _unit.ZoomOutEvent -= ZoomOutEventCall;
        
        _isHit = false;
        _moveSpeed = 0.0f;
    }
    
    private void EndSkill()
    {
        ResetValue();
        OnEndSkillAction?.Invoke();
    }

    private void ZoomInEventCall()
    {
        _unit.SkillBaldoAttackBase.ZoomIn();        
    }

    private void ZoomOutEventCall()
    {
        _unit.SkillBaldoAttackBase.ZoomOut();                
    }

    private IEnumerator SpecialStateCoroutine()
    {
        float timer = 0.0f;
        bool _isFrist = true;
        
        while (_skillBaldoData.AddCriticalTime >= timer)
        {
            if (_isHit == true && _isFrist == true)
            {
                timer = 0;
                _unit.ChangeCritical(_skillBaldoData.AddCriticalAmount);
                _isFrist = false;
            }
            
            timer += GameManager.instance.TimeMng.FixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        if (_isHit == true)
        {
            _unit.ChangeCritical(-_skillBaldoData.AddCriticalAmount);
        }

        EndSkill();
    }
    
    IEnumerator AttackMoveCoroutine()
    {
        float timer = 0.0f;
        while (_skillBaldoData.MoveTimes[0] > timer)
        {
            timer += GameManager.instance.TimeMng.FixedDeltaTime;
            AttackMove();
            yield return new WaitForFixedUpdate();
        }

        EndAttackMove();
    }
}
