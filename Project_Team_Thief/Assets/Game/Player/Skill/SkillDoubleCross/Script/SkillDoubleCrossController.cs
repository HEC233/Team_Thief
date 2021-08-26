using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDoubleCrossController : SkillControllerBase
{

    private SkillDoubleCrossData _skillDoubleCrossData;
    private PlayerUnit _unit;
    private Damage _damage;
    private int _index;
    private float _moveSpeed;
    
    public SkillDoubleCrossController(GameSkillObject skillObject, SkillDataBase data, Unit unit) : base(skillObject, data, unit) { }
    
    public override void Invoke()
    {
        Init();
    }

    private void Init()
    {
        _skillDoubleCrossData = SkillData as SkillDoubleCrossData;
        _unit = Unit as PlayerUnit;
        FindIndex();
        _unit.OnSkillDoubleCrossAttackEvent += ReceiveAttackEvent;

        _damage = new Damage();
        _damage.power =  _unit.CalcSkillDamage(_skillDoubleCrossData.Damages[0]);
        _damage.knockBack = new Vector2(_skillDoubleCrossData.KnockBackXs[0], _skillDoubleCrossData.KnockBackYs[0]);
        _damage.additionalInfo = _index;
        // 넉백 타임은?
        _damage.stiffness = _skillDoubleCrossData.Stiffness;
        
        _unit.SkillDoubleCrossAttackBase[_index].Init(_damage, _skillDoubleCrossData.CinemachineSignalSource);
        
    }

    private void FindIndex()
    {
        if (_skillDoubleCrossData.ID == 8)
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
        _unit.SkillDoubleCrossAttackBase[_index].Progress();

        if (_index == 0)
        {
            SetAttackMove();
            _unit.StartCoroutine(AttackMoveCoroutine());
        }
        else
        {
            EndSkill();
        }
    }

    private void SetAttackMove()
    {
        _unit.MoveStop();

        _moveSpeed = (1 / _skillDoubleCrossData.MoveTimes[0]) * _skillDoubleCrossData.MoveXs[0];
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
        _moveSpeed = 0;
        _index = 0;
    }
    
    private void EndSkill()
    {
        ResetValue();
        _unit.OnSkillDoubleCrossAttackEvent -= ReceiveAttackEvent;
        OnEndSkillAction?.Invoke();
    }

    IEnumerator AttackMoveCoroutine()
    {
        float timer = 0.0f;
        while (_skillDoubleCrossData.MoveTimes[0] > timer)
        {
            timer += GameManager.instance.TimeMng.FixedDeltaTime;
            AttackMove();
            yield return new WaitForFixedUpdate();
        }

        EndAttackMove();
        EndSkill();
    }
}
