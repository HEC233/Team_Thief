using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightWarriorUnit : Unit
{
    [SerializeField]
    private bool isOnGround = false;

    public SOUnit data;
    public float accelation = 100;

    public Transform footPosition;

    // 유닛이 피해를 입으면 애니메이션을 관장하는 액터또한 알아야 한다. 사이의 중계를 위한 이벤트
    [HideInInspector]
    public UnityEvent hitEvent;
    [HideInInspector]
    public UnityEvent dieEvent;

    [SerializeField]
    private float _hp;

    // 공격에 관한 값들, 계속 힙에 할당되는걸 막고자 클래스 변수로 만들어 놓았다.
    public List<BoxCollider2D> attackBox = new List<BoxCollider2D>();
    private int _curAttackBox = 0;
    public LayerMask hitBoxLayer;
    ContactFilter2D contactFilter = new ContactFilter2D();
    List<Collider2D> result = new List<Collider2D>();

    private void Start()
    {
        _unitName = data.unitName;

        _hp = data.hp;

        contactFilter.useTriggers = true;
        contactFilter.useLayerMask = true;
        contactFilter.SetLayerMask(hitBoxLayer);

        //--------------------------
        SetDamagePower(0).SetDamageKnockBack(new Vector2(200, 200));
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        isOnGround = Physics2D.Raycast(footPosition.position, Vector2.down, 0.2f, _groundLayer);

        if (isOnGround)
        {
            Vector2 velocity = _rigid.velocity;
            if (velocity.sqrMagnitude > data.maxSpeed * data.maxSpeed)
                _rigid.velocity = velocity.normalized * data.maxSpeed;
        }
    }

    public override void Idle()
    {
        _rigid.velocity = new Vector2(0.0f, _rigid.velocity.y);
    }

    public override void Move(float delta)
    {
        _rigid.AddForce(new Vector2(delta * accelation, 0));
    }

    public override void MoveTo(Vector3 position)
    {
    }

    public override void Jump(float jumpForce)
    {
        _rigid.AddForce(new Vector2(0, jumpForce));
    }

    public void SetAttackBox(bool isDirectionRight, int index = -1)
    {
        if (isDirectionRight)
        {
            // 보는 방향이 달라졌을때의 처리를 위한 공간
        }

        if (index > -1)
            _curAttackBox = index;
    }

    public override void Attack()
    {
        // 히트박스 레이어와 접촉해 있는지 판단
        if(attackBox[_curAttackBox].IsTouchingLayers(hitBoxLayer))
        {
            // 접촉해 있는 히트박스레이어인 콜라이더들 가져오기
            attackBox[_curAttackBox].OverlapCollider(contactFilter, result);
            foreach(var c in result)
            {
                // 콜라이더로 부터 Unit 추출
                var u = c.GetComponentInParent<Unit>();
                // 유닛이 자기자신이거나 없으면 예외처리
                if (u == null || u == this)
                    continue;
                u.HandleHit(_damage);
            }
        }
    }

    public override void HandleHit(in Damage inputDamage)
    {
        // 대미지 상정방식 기획서에 맞게 변경 필요
        _hp -= inputDamage.power;
        _rigid.AddForce(inputDamage.knockBack);

        if (_hp <= 0)
        {
            dieEvent.Invoke();
        }
        else
        {
            hitEvent.Invoke();
        }
    }
}
