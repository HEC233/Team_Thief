using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SkillAxeController : SkillControllerBase
{
    public SkillAxeController(GameSkillObject skillObject, SkillDataBase data, Unit unit) : base(skillObject, data, unit) { }
    private SkillAxeData _skillAxeData;
    private PlayerUnit _unit;
    private SkillAxeAttackCtrl _skillAxeAttackCtrl;
    private Damage _damage;
    private int _index;
    private float _moveSpeed;

    public override void Invoke()
    {
        Init();
        Progress();
    }

    private void Init()
    {
        _skillAxeData = SkillData as SkillAxeData;
        _unit = Unit as PlayerUnit;
        FindIndex();
        SetDamage();
    }

    private void SetDamage()
    {
        _damage = new Damage();
        _damage.power = _skillAxeData.Damages[0];
        _damage.knockBack = new Vector2(_skillAxeData.KnockBackXs[0], _skillAxeData.KnockBackYs[0]);
        _damage.additionalInfo = _index;
        _damage.stiffness = _skillAxeData.Stiffness;

    }
    
    private void FindIndex()
    {
        if (_skillAxeData.ID == 3)
        {
            _index = 0;
        }
        else
        {
            _index = 1;
        }
        
    }

    private void Progress()
    {
        SpawnAxe();
        SetAttackMoveValue();
        _unit.StartCoroutine(AttackMoveCoroutine());
    }

    private void SpawnAxe()
    {
        _skillAxeAttackCtrl =
            GameObject.Instantiate(_skillAxeData.AxeGameObject, Unit.transform.position, quaternion.identity)
                .GetComponent<SkillAxeAttackCtrl>();
        _skillAxeAttackCtrl.OnEndSkillEvent += EndSkill;
        _skillAxeAttackCtrl.OnEnemyHitEvent += _unit.OnAddComboEventCall;
        _skillAxeAttackCtrl.Init(_damage, _skillAxeData);
        _skillAxeAttackCtrl.StartAxeProgress(_unit.FacingDir);
    }

    private void SetAttackMoveValue()
    {
        if(_skillAxeData.MoveTimes[0] == 0)
        {
            _moveSpeed = 0;
            return;
        }
        
        _unit.MoveStop();

        _moveSpeed = (1 / _skillAxeData.MoveTimes[0]) * _skillAxeData.MoveXs[0];
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
        _skillAxeAttackCtrl.OnEndSkillEvent -= EndSkill;
        _skillAxeAttackCtrl.OnEnemyHitEvent -= _unit.OnAddComboEventCall;
        _skillAxeAttackCtrl = null;
        OnEndSkillAction?.Invoke();
    }
    
    IEnumerator AttackMoveCoroutine()
    {
        float timer = 0.0f;
        while (_skillAxeData.MoveTimes[0] > timer)
        {
            timer += GameManager.instance.TimeMng.FixedDeltaTime;
            AttackMove();
            yield return new WaitForFixedUpdate();
        }

        EndAttackMove();
    }
}
