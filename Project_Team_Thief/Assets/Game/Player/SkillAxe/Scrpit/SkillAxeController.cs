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
        _damage.power = _skillAxeData.AxeAttackDamage;
        _damage.knockBack = _skillAxeData.KnockBackPower;
        _damage.additionalInfo = 3;
    }

    private void Progress()
    {
       
        _skillAxeAttackCtrl =
            GameObject.Instantiate(_skillAxeData.AxeGameObject, Unit.transform.position, quaternion.identity)
                .GetComponent<SkillAxeAttackCtrl>();

        _skillAxeAttackCtrl.OnEndSkillEvent += EndSkill;
        _skillAxeAttackCtrl.OnEnemyHitEvent += _unit.OnAddComboEventCall;
        _skillAxeAttackCtrl.SetDamage(_damage);
        _skillAxeAttackCtrl.Init(_skillAxeData.AxeMovePostionX, _skillAxeData.AxeMoveTime, _skillAxeData.CinemachineSignalSource, _unit.FacingDir, _skillAxeData.AxeMultiStageHit, _skillAxeData.AxeMultiStageHitInterval);
    }

    private void EndSkill()
    {
        _skillAxeAttackCtrl.OnEndSkillEvent -= EndSkill;
        _skillAxeAttackCtrl.OnEnemyHitEvent -= _unit.OnAddComboEventCall;
        _skillAxeAttackCtrl = null;
        OnEndSkillAction?.Invoke();
    }
}
