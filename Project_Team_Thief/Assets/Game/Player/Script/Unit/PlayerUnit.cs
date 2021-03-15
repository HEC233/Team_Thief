using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Unit은 외부에 보이는 인터페이스.
public class PlayerUnit : Unit
{
    // 이동 관련 컨트롤러
    [SerializeField] 
    private PlayerMovementCtrl _playerMovementCtrl;
    
    // 방향 관련 변수
    float _facingDir = 1;
    bool _isFacingRight = true;

    void Start()
    {
        
    }

    public override void Idle()
    {
        base.Idle();
    }

    public override void Move(float delta)
    {
        // Vector2 dir = moveUtil.moveforce(5);
        // Rigidbody2D.addfoce(dir);
        
        _playerMovementCtrl.Move(_facingDir);
    }

    public void MoveStop()
    {
        _playerMovementCtrl.MoveStop();
    }

    public bool IsRunningInertia()
    {
        return _playerMovementCtrl.IsRunningInertia();
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
    
    public void CheckMovementDir(float inputDir)
    {
        if (_isFacingRight && inputDir < 0)
        {
            Flip();
        }
        else if (!_isFacingRight && inputDir > 0)
        {
            Flip();
        }
    }
    
    void Flip()
    {
        _facingDir *= -1;
        _isFacingRight = !_isFacingRight;
        transform.parent.transform.Rotate(0.0f, 180.0f, 0.0f);   // y값 돌리는 숫자를 curANi 머 선형보간 등 애니메이션을 주면 빙글도는 이쁜 애니메이션~
    }
}
