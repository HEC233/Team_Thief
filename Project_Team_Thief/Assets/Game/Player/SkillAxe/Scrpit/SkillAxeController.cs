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
    
    public override void Invoke()
    {
        Init();
        Progress();
    }

    private void Init()
    {
        _skillAxeData = SkillData as SkillAxeData;
        _unit = Unit as PlayerUnit;

        _damage = new Damage();
        _damage.power = _skillAxeData.Damages[0] * _unit.GetDamageWeightFromEencroachment();
        _damage.knockBack = new Vector2(_skillAxeData.KnockBackXs[0], _skillAxeData.KnockBackYs[0]);
        _damage.additionalInfo = 3;
    }

    private void Progress()
    {
        _skillAxeAttackCtrl =
            GameObject.Instantiate(_skillAxeData.AxeGameObject, Unit.transform.position, quaternion.identity)
                .GetComponent<SkillAxeAttackCtrl>();
        Debug.Log(_skillAxeData.HitNumberOfTimes[0]);
        _skillAxeAttackCtrl.OnEndSkillEvent += EndSkill;
        _skillAxeAttackCtrl.OnEnemyHitEvent += _unit.OnAddComboEventCall;
        _skillAxeAttackCtrl.Init(_damage, _skillAxeData.CinemachineSignalSource);
        _skillAxeAttackCtrl.Init(_skillAxeData.ProjectileMoveX, _skillAxeData.ProjectileMoveTime, _skillAxeData.CinemachineSignalSource, _unit.FacingDir, _skillAxeData.HitNumberOfTimes[0], _skillAxeData.HitIntervals[0]);
        
    }

    private void EndSkill()
    {
        _skillAxeAttackCtrl.OnEndSkillEvent -= EndSkill;
        _skillAxeAttackCtrl.OnEnemyHitEvent -= _unit.OnAddComboEventCall;
        _skillAxeAttackCtrl = null;
        OnEndSkillAction?.Invoke();
    }
}
