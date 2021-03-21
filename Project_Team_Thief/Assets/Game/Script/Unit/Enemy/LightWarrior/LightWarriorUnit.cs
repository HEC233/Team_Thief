using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class LightWarriorUnit : Unit
{
    [SerializeField]
    private bool isOnGround = false;

    public SOUnit data;
    public float accelation = 100;

    public Transform footPosition; 

    private void Start()
    {
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
    }

    public override void HandleHit(in Damage inputDamage)
    {

    }
}
