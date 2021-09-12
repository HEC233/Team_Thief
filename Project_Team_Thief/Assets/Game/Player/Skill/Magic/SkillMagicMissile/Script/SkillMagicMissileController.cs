using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class SkillMagicMissileController : SkillControllerBase
{
    private SkillMagicMissileData _skillMagicMissileData;
    private PlayerUnit _unit;
    private Damage _damage;
    private SkillMagicMissileAttackCtrl[] _skillMagicMissileAttackCtrls = new SkillMagicMissileAttackCtrl[2];
    private int _magicMissileIndex = 0;
    
    public SkillMagicMissileController(GameSkillObject skillObject, SkillDataBase data, Unit unit) : base(skillObject, data, unit) { }

    public override void Invoke()
    {
        base.Invoke();
        Init();
    }

    private void Init()
    {
        _skillMagicMissileData = SkillData as SkillMagicMissileData;
        _unit = Unit as PlayerUnit;
        Progress();
    }
    
    private void SetDamage(int index)
    {
        _damage = new Damage();
        _damage.power =
            _unit.CalcSkillDamage(_skillMagicMissileData.Damages[index]);
        _damage.knockBack = new Vector2(_skillMagicMissileData.KnockBackXs[0], _skillMagicMissileData.KnockBackYs[0]) *
                            _unit.FacingDir;
        _damage.additionalInfo = 0;
        _damage.stiffness = _skillMagicMissileData.Stiffness;
    }

    private void Progress()
    {
        _unit.StartCoroutine(SkillMagicMissileCoroutine());
    }

    private void SpawnMagicMissile()
    {
        _skillMagicMissileAttackCtrls[_magicMissileIndex] =
            GameObject.Instantiate(_skillMagicMissileData.MagicMissileGO, Unit.transform.position, quaternion.identity)
                .GetComponent<SkillMagicMissileAttackCtrl>();
        _skillMagicMissileAttackCtrls[_magicMissileIndex].OnEnemyHitEvent += _unit.OnAddComboEventCall;
        SetDamage(_magicMissileIndex);
        _skillMagicMissileAttackCtrls[_magicMissileIndex].Init(_damage, _skillMagicMissileData);
        
        _skillMagicMissileAttackCtrls[_magicMissileIndex].StartMagicMissileProgress(_unit.FacingDir);
    }

    private void ResetValue()
    {
        _magicMissileIndex = 0;
    }
    
    private void EndSkill()
    {
        ResetValue();
        OnEndSkillAction?.Invoke();
    }

    private IEnumerator SkillMagicMissileCoroutine()
    {
        float timer = 0.0f;

        SpawnMagicMissile();
        _magicMissileIndex++;
        while (_skillMagicMissileData.SpawnInterval >= timer)
        {
            timer += GameManager.instance.TimeMng.FixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }
        
        SpawnMagicMissile();
        EndSkill();
    }
}
