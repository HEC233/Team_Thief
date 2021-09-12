using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSnakeSwordFlurryController : SkillControllerBase
{
    private SkillSnakeSwordFlurryData _skillSnakeSwordFlurryData;
    private PlayerUnit _unit;
    private Damage _damage;
    private bool _isSkillEnd = false;

    public SkillSnakeSwordFlurryController(GameSkillObject skillObject, SkillDataBase data, Unit unit) : base(skillObject, data, unit) { }

    public override void Invoke()
    {
        base.Invoke();
        Init();
    }
    
    private void Init()
    {
        _skillSnakeSwordFlurryData = SkillData as SkillSnakeSwordFlurryData;
        _unit = Unit as PlayerUnit;
        SetDamage();
        _unit.SkillSnakeSwordFlurryAttackBase.Init(_damage, _skillSnakeSwordFlurryData.CinemachineSignalSource);
        _unit.skillSnakeSwordFlurryEndEvent += EndSkill;
        Progress();
    }
    
    private void SetDamage()
    {
        _damage = new Damage();
        _damage.power =
            _unit.CalcSkillDamage(_skillSnakeSwordFlurryData.Damages[0] /
                                  _skillSnakeSwordFlurryData.SkillNumberOfTimes);
        _damage.knockBack =
            new Vector2(_skillSnakeSwordFlurryData.KnockBackXs[0], _skillSnakeSwordFlurryData.KnockBackYs[0]) *
            _unit.FacingDir;
        _damage.additionalInfo = 0;
        _damage.stiffness = _skillSnakeSwordFlurryData.Stiffness;
    }

    private void Progress()
    {
        SkillObject.StartCoroutine(SnakeSwordFlurryCoroutine());
    }

    private void EndSkill()
    {
        ResetValue();
        OnEndSkillAction?.Invoke();
    }

    private void ResetValue()
    {
        _isSkillEnd = true;
        _unit.skillSnakeSwordFlurryEndEvent -= EndSkill;
    }

    IEnumerator SnakeSwordFlurryCoroutine()
    {
        float timer = 0.0f;
        float snakeSwordFlurryHitInterval =
            _skillSnakeSwordFlurryData.AnimationTime / _skillSnakeSwordFlurryData.SkillNumberOfTimes;
        while (_isSkillEnd == false)
        {
            timer += GameManager.instance.TimeMng.FixedDeltaTime;
            if (timer >= snakeSwordFlurryHitInterval)
            {
                _unit.SkillSnakeSwordFlurryAttackBase.Progress();
                timer = 0.0f;
            }
            yield return new WaitForFixedUpdate();
        }
        
    }
    
    // IEnumerator SnakeSwordMultiStageHitCoroutine(Collider2D target)
    // {
    //     float timer = 0.0f;
    //     int count = 0;
    //     while (_skillSnakeSwordFlurryData.HitNumberOfTimes[0] >= count)
    //     {
    //         timer += GameManager.instance.TimeMng.FixedDeltaTime;
    //
    //         if (timer >= _skillSnakeSwordFlurryData.HitIntervals[0])
    //         {
    //             timer = 0.0f;
    //             count++;
    //             _unit.SkillSnakeSwordFlurryAttackBase.ProgressTargetSelection(target);
    //         }
    //         
    //         yield return new WaitForFixedUpdate();
    //     }
    //
    // }
}
