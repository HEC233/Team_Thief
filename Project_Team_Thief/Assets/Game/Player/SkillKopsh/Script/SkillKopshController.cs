using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillKopshController : SkillControllerBase
{
    public SkillKopshController(GameSkillObject skillObject, SkillDataBase data, Unit unit) : base(skillObject, data, unit) { }

    private SkillKopshData _skillKopshData;

    private PlayerUnit _unit;

    private Damage _damage;

    private float _rushSpeed = 0;

    private bool _isMove = false;

    public override void Invoke()
    {
        Init();
    }

    private void Init()
    {
        _skillKopshData = SkillData as SkillKopshData;
        _unit = Unit as PlayerUnit;

        _damage = new Damage();
        _damage.power = _skillKopshData.AttackDamageArr[_unit.skillKopshIndex] *
                        _unit.GetDamageWeightFromEencroachment();
        _damage.knockBack = new Vector2(_skillKopshData.KnockBackPowerArr[_unit.skillKopshIndex].x * _unit.FacingDir,
            _skillKopshData.KnockBackPowerArr[_unit.skillKopshIndex].y);
        _damage.additionalInfo = _unit.skillKopshIndex;

        _unit._skillKopshAttackCtrls[_unit.skillKopshIndex].signalSourceAsset =
            _skillKopshData.CinemachineSignalSourceArr[_unit.skillKopshIndex];

        _rushSpeed = (1 / _skillKopshData.PlayerMoveTimeArr[_unit.skillKopshIndex]) *
                     _skillKopshData.PlayerMovePostionXArr[_unit.skillKopshIndex];

        _unit.OnSkillKopshAttackEvent += AttackSkillKopsh;

        _unit.StartCoroutine(SkillKopshMoveCoroutine());
    }
    
    public override void Release()
    {
        base.Release();

        _unit.OnSkillKopshAttackEvent -= AttackSkillKopsh;
    }

    private void AttackSkillKopsh()
    {
        _unit.SkillKopshAttack(_damage);
        if (_isMove == false)
        {
            OnEndSkillAction?.Invoke();
        }
    }

    IEnumerator SkillKopshMoveCoroutine()
    {
        _isMove = true;
        float timer = 0.02f;

        while (timer <= _skillKopshData.PlayerMoveTimeArr[_unit.skillKopshIndex])
        {
            timer += GameManager.instance.timeMng.FixedDeltaTime;
            _unit.Rigidbody2D.velocity = Vector2.zero;
            var power = new Vector2(_rushSpeed * _unit.FacingDir * GameManager.instance.timeMng.TimeScale, 0);
            _unit.Rigidbody2D.AddForce(power, ForceMode2D.Impulse);
            yield return new WaitForFixedUpdate();
        }
        
        _unit.Rigidbody2D.velocity = Vector2.zero;
        _isMove = false;
    }
}
