using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillSpearController : SkillControllerBase
{
    public SkillSpearController(GameSkillObject skillObject, SkillDataBase data, Unit unit) : base(skillObject, data, unit) { }

    private SkillSpearData _skillSpearData;

    private PlayerUnit _unit;

    private Damage _damage;

    private float _rushSpped = 0;

    public override void Invoke()
    {
        Init();

        GameManager.instance.FX.Play("SpearRushFx", _unit.transform.position, _unit.transform.rotation);
    }

    private void Init()
    {
        _skillSpearData = SkillData as SkillSpearData;
        _unit = Unit as PlayerUnit;

        _damage = new Damage();
        _damage.power = _skillSpearData.AttackDamage;
        _damage.knockBack = new Vector2(_skillSpearData.KnockBackPower.x * _unit.FacingDir,
            _skillSpearData.KnockBackPower.y);
        _damage.additionalInfo = 5;

        _rushSpped = (1 / _skillSpearData.PlayerMoveTime) * _skillSpearData.PlayerMovePostionX;

        _unit._skillSpearAttackCtrl.signalSourceAsset = _skillSpearData.CinemachineSignalSource;
        _unit.OnSkillSpearRushEvent += StartSpearRush;
        _unit.OnSkillSpearAttackEvent += AttackSkillSpear;
        

    }

    public override void Release()
    {
        base.Release();
        
        _unit.OnSkillSpearRushEvent -= StartSpearRush;
        _unit.OnSkillSpearAttackEvent -= AttackSkillSpear;
    }

    private void StartSpearRush()
    {
        SkillObject.StartCoroutine(SkillSpearRushCoroutine());
        WwiseSoundManager.instance.PlayEventSound("PC_spear_Rush");

    }

    private void AttackSkillSpear()
    {
        _unit.SKillSpearAttack(_damage);
    }

    IEnumerator SkillSpearRushCoroutine()
    {
        float timer = 0.02f;
        
        while (timer <= _skillSpearData.PlayerMoveTime)
        {
            timer += GameManager.instance.timeMng.FixedDeltaTime;
            _unit.Rigidbody2D.velocity = Vector2.zero;
            var power = new Vector2(_rushSpped * _unit.FacingDir * GameManager.instance.timeMng.TimeScale, 0);
            _unit.Rigidbody2D.AddForce(power, ForceMode2D.Impulse);
            yield return new WaitForFixedUpdate();
        }
        
        _unit.Rigidbody2D.velocity = Vector2.zero;
        OnEndSkillAction?.Invoke();
    }
}
