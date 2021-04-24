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
    

    public void Init()
    {
        _animationCtrl.PlayAni(AniState.ShadowLumpSpawn);
        Move();
    }

    public override void Move()
    {
        float leftOrRight = Random.Range(0, 2);
        float powerX = 0.0f;
        float powerY = 0.0f;
        
        Debug.Log("leftOrRight" + leftOrRight);
        if (leftOrRight == 0)   // 왼쪽
        {
            powerX = Random.Range(-1.0f, -3.0f);
        }
        else                    // 오른쪽
        {
            powerX = Random.Range(1.0f, 3.0f);
        }
        
        powerY = Random.Range(1.0f, 5f);
        Vector2 power = new Vector2(powerX, powerY);
        _rigidbody2D.AddForce(power, ForceMode2D.Impulse);
        Debug.Log(power);

    }

    public override void HandleHit(in Damage inputDamage)
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
    }
}
