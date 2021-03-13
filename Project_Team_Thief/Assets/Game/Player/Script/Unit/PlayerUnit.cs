using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
    void Start()
    {
        
    }

    public override void Idle()
    {
        base.Idle();
    }

    public override void Move(float delta)
    {
        base.Move(delta);
    }

    public override void Jump(float jumpForce)
    {
        base.Jump(jumpForce);
    }

    public override void Attack()
    {
        base.Attack();
    }

    public override void HandleHit(ref Damage inputDamage)
    {
        base.HandleHit(ref inputDamage);
    }
}
