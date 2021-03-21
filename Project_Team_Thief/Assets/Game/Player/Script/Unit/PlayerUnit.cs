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
    
    // Ground Check
    private bool _isGround = true;
    public bool IsGround => _isGround;

    [Header("Ground Check")]
    public Transform groundCheck;    // Ground체크를 위함
    public Vector2 groundCheckBoxSize;
    public LayerMask groundLayer;

    // 점프 관련 변수
    //private bool _isJumpAble = false;
    private int _jumpCount = 0;
    private float _coyoteTime = 0.2f;
    
    // 데이터로 관리 할 변수
    private float _maxCoyoteTime = 0.2f; 
    private int _maxJumpCount = 2;
    private float _maxJumpTime = 0.1f;

    public float MaxJumpTime => _maxJumpTime;

    void Start()
    {
        Init();
    }

    void Init()
    {
        SetVariable(0.2f, 2, 0.08f);
    }
    
    // 향후에는 데이터 센터 클래스라던가 데이터를 가지고 있는 함수에서 직접 호출로 받아 올 수 있도록
    // 수정 할 예정
    public void SetVariable(float coyoteTime, int maxJumpCount, float maxJumpTime)
    {
        _maxCoyoteTime = coyoteTime;
        _maxJumpCount = maxJumpCount;
        _maxJumpTime = maxJumpTime;

        _jumpCount = _maxJumpCount;
        _coyoteTime = _maxCoyoteTime;
        
    }
    

    public override void Idle()
    {
        base.Idle();
    }

    public void Progress()
    {
        CheckGround();
    }
    
    private void OnDrawGizmos()
    {
        if (groundCheck != null && groundCheckBoxSize != null)
            Gizmos.DrawWireCube(groundCheck.position, groundCheckBoxSize);
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
        _jumpCount--;
        
        _playerMovementCtrl.Jump(0);
    }

    public bool CheckIsJumpAble()
    {
        if (_coyoteTime >= 0.0f)
        {
            if (_jumpCount >= 1)
            {
                return true;
            }
        }

        return false;
    }

    public void ResetJumpVal()
    {
        _jumpCount = _maxJumpCount;
        _coyoteTime = _maxCoyoteTime;
    }

    public override void Attack()
    {
        base.Attack();
    }

    public override void HandleHit(in Damage inputDamage)
    {
        base.HandleHit(in inputDamage);
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

    public void CheckGround()
    {
        _isGround = Physics2D.OverlapBox(groundCheck.position, groundCheckBoxSize, 0, groundLayer);

        if(_isGround == false)
        {
            _coyoteTime -= Time.deltaTime;
        }
        else if (_isGround == true)
        {
            ResetJumpVal();
        }
    }
    
}
