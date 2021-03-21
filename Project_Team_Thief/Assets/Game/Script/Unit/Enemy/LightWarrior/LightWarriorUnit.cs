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

    [HideInInspector]
    public UnityEvent hitEvent;
    [HideInInspector]
    public UnityEvent dieEvent;

    [SerializeField]
    private float _hp;

    public BoxCollider2D attackBox;
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

    public override void Attack()
    {
        if(attackBox.IsTouchingLayers(hitBoxLayer))
        {
            attackBox.OverlapCollider(contactFilter, result);
            foreach(var c in result)
            {
                var u = c.GetComponentInParent<Unit>();
                if (u == null || u == this)
                    continue;
                u.HandleHit(_damage);
            }
        }
    }

    public override void HandleHit(in Damage inputDamage)
    {
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
