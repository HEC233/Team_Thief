using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Unit : MonoBehaviour
{
    protected string _unitName;
    protected Damage _damage;
    [SerializeField]
    protected LayerMask _groundLayer;
    protected Rigidbody2D _rigid;

    private void Awake()
    {
        _damage = new Damage();
        _rigid = GetComponentInParent<Rigidbody2D>();
        Assert.IsNotNull(_rigid);
    }

    // 아무 행동을 하고 있지 않는 기본 상태입니다.
    public virtual void Idle()
    {
        
    }
    
    // 유닛을 자신의 속도에 대해 delta만큼 곱한 값으로 가속합니다.
    public virtual void Move(float delta)
    {

    }

    // 유닛을 position까지 이동시킵니다.
    public virtual void MoveTo(Vector3 position)
    {

    }

    // 유닛을 jumpForce 만큼 점프시킵니다.
    public virtual void Jump(float jumpForce)
    {

    }

    // 유닛의 공격을 처리합니다.
    public virtual void Attack()
    {
    }

    // 유닛의 피격을 처리합니다.
    public virtual void HandleHit(in Damage inputDamage)
    {

    }

    public virtual void HandleDeath()
    {

    }

    public Vector2 GetSpeed()
    {
        return _rigid.velocity;
    }

    // 유닛 공격의 데미지를 조정합니다.
    public Unit SetDamagePower(float power)
    {
        _damage.power = power;

        return this;
    }

    // 유닛 공격의 넉백을 조정합니다.
    public Unit SetDamageKnockBack(Vector2 knockBack)
    {
        _damage.knockBack = knockBack;

        return this;
    }

    // 유닛 공격의 상태이상을 조정합니다.
    public Unit SetDamageAbnormal(AbnormalState abnormal)
    {
        _damage.abnormal = abnormal;

        return this;
    }

    public string GetUnitName()
    {
        return _unitName;
    }
}
