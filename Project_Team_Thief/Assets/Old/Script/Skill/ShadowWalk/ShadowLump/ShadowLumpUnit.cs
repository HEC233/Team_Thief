using System;
using System.Collections;
using System.Collections.Generic;
using LightWarrior;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShadowLumpUnit : Unit
{
    [SerializeField]
    private float _moveSpped;

    [SerializeField] 
    private Rigidbody2D _rigidbody2D;

    [SerializeField] 
    private AnimationCtrl _animationCtrl;

    private bool _isSpawnAniEnd = false;
    private float _controlTime = 0.0f;
    
    public void Init(float controlTime)
    {
        _controlTime = controlTime;
        //_animationCtrl.PlayAni(AniState.ShadowLumpSpawn);
        Move();
    }

    public override void Move()
    {
        Vector2 power = SetMovePower();
        _rigidbody2D.AddForce(power, ForceMode2D.Impulse);
    }

    private Vector2 SetMovePower()
    {
        float leftOrRight = Random.Range(0, 2);
        float powerX = 0.0f;
        float powerY = 0.0f;
        
        if (leftOrRight == 0)   // 왼쪽
        {
            powerX = Random.Range(-1.0f, -3.0f);
        }
        else                    // 오른쪽
        {
            powerX = Random.Range(1.0f, 3.0f);
        }
        
        powerY = Random.Range(1.0f, 5f);

        return new Vector2(powerX, powerY);
    }

    public override void HandleHit(in Damage inputDamage)
    {
        Vector2 power = new Vector2();
        //_animationCtrl.PlayAni(AniState.ShadowLumpHit);
        if (inputDamage.knockBack.x > 0)
        {
            power = new Vector2(_moveSpped, 0);
        }
        else
        {
            power = new Vector2(_moveSpped * -1, 0);
        }

        gameObject.layer = LayerMask.NameToLayer("Attack");
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.gravityScale = 0;
        _rigidbody2D.AddForce(power, ForceMode2D.Impulse);
    }

    public void OnSpawnAniEndEventCall()
    {
        _isSpawnAniEnd = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Shadow"))
        {
            if (other.GetComponent<Shadow>().isControlState == false)
            {
                other.GetComponent<Shadow>().ChangeControlState(_controlTime);
                //_animationCtrl.PlayAni(AniState.ShadowLumpDie);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || 
            other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            if (_isSpawnAniEnd == true)
            {
                //_animationCtrl.PlayAni(AniState.ShadowLumpDie);
            }
        }
    }

    public void OnDieAnimationEndEventCall()
    {
        Destroy(this.gameObject);
    }
}
