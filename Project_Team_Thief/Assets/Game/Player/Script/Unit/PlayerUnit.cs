using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


// Unit은 외부에 보이는 인터페이스.
public class PlayerUnit : Unit
{
    
    [SerializeField] 
    private Rigidbody2D _rigidbody2D;

    // 방향 관련 변수
    private float _facingDir = 1;
    private bool _isFacingRight = true;
    
    // Ground Check
    [SerializeField]
    private bool _isGround = true;
    public bool IsGround => _isGround;

    [Header("Ground Check")]
    public Transform groundCheck;    // Ground체크를 위함
    public Vector2 groundCheckBoxSize;
    public LayerMask groundLayer;

    // 시간 관련 변수
    private float _scale = 1;
    
    ///////////////////////////// 데이터로 관리 할 변수
    // 점프 관련 변수
    private int _jumpCount = 0;
    private float _coyoteTime = 0.2f;
    private float _maxCoyoteTime = 0.2f; 
    private int _maxJumpCount = 2;
    private float _maxJumpTime = 0.1f;
    public float MaxJumpTime => _maxJumpTime;

    [Header("Jump Variable")] 
    [SerializeField]
    private float _jumpScale = 1.0f;
    
    [SerializeField]
    private float _jumpPower = 5.0f;
    
    [SerializeField] 
    private float _addAllJumpPpower = 8.0f;
    
    [SerializeField]
    private float _addJumpPower = 4f;

    // 이동 관련 변수
    [Header("Move Variable")]
    private float _curSpeed = 0.0f;
    [SerializeField]
    private float _minSpeed = 0.8f;
    [SerializeField]
    private float _maxSpeed = 6.5f;
    [SerializeField]
    private float _moveStopSpeed = 1.0f;

    private bool _isTouchMaxSpeed = false;
    
    // 구르기 관련 변수
    [Header("Roll Variable")]
    [SerializeField]
    private float _rollGoalX = 10;
    [SerializeField]
    private float _rollTime = 0.0f;

    public float RollTime => _rollTime;
    
    private float _rollSpeed = 0.5f;
    [SerializeField]
    private float _rollCoolTime;
    private bool _isRollAble = true;
    public bool IsRollAble => _isRollAble;
    
    [SerializeField] 
    private PhysicsMaterial2D _rollPhysicMaterial;
    
    // WallSlideing Variable
    [Header("Wallslideing Variable")]
    [SerializeField]
    private Transform _handTr;
    public LayerMask wallLayerMask;
    RaycastHit2D _wallRayCastHit2D;
    private bool _isWallTouch = false;
    [SerializeField]
    private float _wallJumpPowerX;
    [SerializeField]
    private float _wallJumpPowerY;
    [SerializeField]
    private float _wallSlideingUpPower = 0;
    
    // BasicAttack Variable
    [Header("BasicAttack Variable")] 
    [SerializeField]
    private float _basicAttackTime = 0.5f;
    public float BasicAttackTime => _basicAttackTime;

    [SerializeField] 
    private float[] _basicAttackMoveTimeArr;
    public float[] BasicAttackMoveTimeArr => _basicAttackMoveTimeArr;
    [SerializeField] 
    private float[] _basicAttackMoveGoalXArr;
    private float _basicAttackMoveSpeed = 0.0f;
    
    //////////////////////////// 데이터로 관리 할 변수

    private float _originalGravityScale = 0;

    void Start()
    {
        Init();
    }

    void Init()
    {
        SetVariable(0.2f, 2, 0.4f);
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

        _originalGravityScale = _rigidbody2D.gravityScale;
    }
    

    public override void Idle()
    {
        base.Idle();
    }

    public void Progress()
    {
        CheckGround();

        // if (Input.GetKeyDown(KeyCode.F))
        // {
        //     _rigidbody2D.MovePosition(new Vector2(0,0));
        // }
    }
    
    private void OnDrawGizmos()
    {
        if (groundCheck != null && groundCheckBoxSize != null)
            Gizmos.DrawWireCube(groundCheck.position, groundCheckBoxSize);
    }

    public override void Move(float delta)
    {

        _rigidbody2D.AddForce(new Vector2(_minSpeed * _facingDir, 0), ForceMode2D.Impulse);

        if (Mathf.Abs(_rigidbody2D.velocity.x) >= _maxSpeed)
        {
            _rigidbody2D.velocity = new Vector2(_maxSpeed * _facingDir, _rigidbody2D.velocity.y);
            _isTouchMaxSpeed = true;
        }

        // Vector2 dir = moveUtil.moveforce(5);
        // Rigidbody2D.addfoce(dir);
        //_playerMovementCtrl.Move(_facingDir);
    }

    public void MoveStop()
    {
        _rigidbody2D.velocity = new Vector2(_moveStopSpeed * _facingDir, _rigidbody2D.velocity.y);
        //_playerMovementCtrl.MoveStop();
    }

    public bool IsRunningInertia()
    {
        if (_isTouchMaxSpeed == true)
        {
            _isTouchMaxSpeed = false;
            return true;
        }

        return false;
        //return Mathf.Abs(_rigidbody2D.velocity.x) >= _maxSpeed - 0.2f ? true : false;
        //return _playerMovementCtrl.IsRunningInertia();
    }


    public override void Jump(float jumpForce)
    {
        _coyoteTime = 0.0f;
        _jumpCount--;
        _isGround = false;

        var power = new Vector3(0, _jumpPower * _jumpScale, 0.0f);
        _rigidbody2D.gravityScale = _jumpScale * _jumpScale;
        _rigidbody2D.AddForce(power, ForceMode2D.Impulse);
        
        //_playerMovementCtrl.Jump(0);
    }

    public void DoubleJump()
    {
        _jumpCount--;
        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
        var power = new Vector3(0, _jumpPower * _jumpScale, 0.0f);
        _rigidbody2D.AddForce(power, ForceMode2D.Impulse);
    }

    public void JumpMoveStop()
    {
        _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
    }

    public void AddJumpForce()
    {
        _rigidbody2D.AddForce((new Vector2(0, _addJumpPower) * _addAllJumpPpower) * _jumpScale * Time.fixedDeltaTime, ForceMode2D.Impulse);

        //_playerMovementCtrl.asd();
        //_playerMovementCtrl.AddJumpForce();
    }

    public bool CheckIsJumpAble()
    {
        if (_coyoteTime >= 0.0f)
        {
            _coyoteTime = -1;
            return true;
        }
        else if (_jumpCount >= 1)
            return true;

        return false;
    }

    public void ResetJumpVal()
    {
        _jumpCount = _maxJumpCount;
        _coyoteTime = _maxCoyoteTime;

    }

    public void SetRoll()
    {
        _rigidbody2D.sharedMaterial = _rollPhysicMaterial;
        _rollSpeed = (1 / _rollTime) * _rollGoalX;
    }

    public void Roll()
    {
        _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
        var power = new Vector2((_rollSpeed) * _facingDir, 0);
        _rigidbody2D.AddForce(power, ForceMode2D.Impulse);
    }

    public void EndRoll()
    {
        _rigidbody2D.sharedMaterial = null;
        if (_isRollAble == true)
            StartCoroutine(RollCoolTimeCoroutine());
    }
    
    // private int counter = 0;
    // Vector2 power = Vector2.zero;
    // public void AddRollPower()
    // {
    //     if (counter < (int) (_rollTime / Time.fixedDeltaTime))
    //     {
    //         counter++;
    //         _rollPerMoveX = _rollGoalX / (_rollTime / Time.fixedDeltaTime);
    //         power = new Vector2(_rollPerMoveX * _facingDir, 0);
    //         //Debug.Log(Physics2D.gravity * Time.deltaTime);
    //         //power += Physics2D.gravity * Time.deltaTime;
    //         power += new Vector2(transform.position.x, transform.position.y);
    //
    //         if (IsGround == false)
    //             power += Physics2D.gravity * Time.fixedDeltaTime;
    //         
    //         _rigidbody2D.MovePosition(power);
    //     }
    //     else
    //     {
    //         _rigidbody2D.velocity = new Vector2(0, power.y);
    //         counter = 0;
    //     }
    // }

    public void RollStop()
    {
        if (IsGround)
            _rigidbody2D.velocity = new Vector2(0.0f, 0.0f);
        // else
        //     _rigidbody2D.velocity = new Vector2(0.0f, _rigidbody2D.velocity.y);
    }

    private float _dashTime = 0;
    private float _dashGoalX = 0;
    private float _dashSpeed = 0;
    public float DashTime => _dashTime;
    
    public void SetDash()
    {
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
        _dashSpeed = (1 / _dashTime) * _dashGoalX;
    }

    public void Dash()
    {
        _rigidbody2D.velocity = Vector2.zero;

        var power = new Vector2((_rollSpeed) * _facingDir, 0);
        _rigidbody2D.AddForce(power, ForceMode2D.Impulse);
    }

    public void EndDash()
    {
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void WallSlideing()
    {
        //_rigidbody2D.AddForce(new Vector2(0, _wallSlideingUpPower), ForceMode2D.Impulse);
        _rigidbody2D.gravityScale -= _wallSlideingUpPower;

        if (_rigidbody2D.gravityScale <= 0)
        {
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.gravityScale = 0;
        }
    }

    public void WallSlideStateStart()
    {
        //_rigidbody2D.velocity = Vector2.zero;
    }
    
    public void WallJump()
    {
        _rigidbody2D.gravityScale = _originalGravityScale;
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.AddForce(new Vector2(_wallJumpPowerX * _facingDir * -1, _wallJumpPowerY) , ForceMode2D.Impulse);
        Flip();
    }

    public void WallReset()
    {
        _rigidbody2D.gravityScale = _originalGravityScale;
    }
    
    public void SetBasicAttack()
    {
        _rigidbody2D.sharedMaterial = _rollPhysicMaterial;
        _rigidbody2D.velocity = Vector2.zero;
    }

    public void BasicAttack(int attackIndex)
    {
        Debug.Log("AttackIndex : " + attackIndex);
    }

    public void BasicAttackMove(int basicAttackIndex)
    {
        _basicAttackMoveSpeed = 
            (1 / _basicAttackMoveTimeArr[basicAttackIndex]) * _basicAttackMoveGoalXArr[basicAttackIndex];
        
        _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);

        var power = new Vector2((_basicAttackMoveSpeed) * _facingDir, 0);
        _rigidbody2D.AddForce(power, ForceMode2D.Impulse);
    }
    
    public void EndBasicAttack()
    {
        _rigidbody2D.sharedMaterial = null;
    }
    
    public void BasicAttackMoveStop()
    {
        if (IsGround)
            _rigidbody2D.velocity = new Vector2(0.0f, 0.0f);
    }

    public override void Attack()
    {
        base.Attack();
    }

    public void BasicJumpAttack()
    {
        
    }

    public void BasicJumpMove(int inputDir)
    {
        _rigidbody2D.AddForce(new Vector2(_minSpeed * inputDir, 0), ForceMode2D.Impulse);
        
        if (Mathf.Abs(_rigidbody2D.velocity.x) >= _maxSpeed)
            _rigidbody2D.velocity = new Vector2(_maxSpeed * inputDir, _rigidbody2D.velocity.y);
    }
    
    public override void HandleHit(in Damage inputDamage)
    {
        // hit는 fsm에게 unityAction을 이용해서
        // 신호를 넘겨줘서 해당 스테이트에서 알아서 처리하도록.
        // 왜? FSM은 상태 변화를 담당하는거고
        // 유닛은 기능에 대한 내용만 있으니 유닛에서 FSM의 changeState를 호출해버리면
        // FSM의 기능이 사라지기 때문에.
        
        base.HandleHit(in inputDamage);
    }

    public Vector3 GetVelocity()
    {
        return _rigidbody2D.velocity;
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
            if (GetVelocity().y >= 1.0f)
            {
                _isGround = false;
                return;
            }

            ResetJumpVal();
        }
    }

    // raycast 말고 그냥  OnTrriger는 어떨까?
    public bool CheckWallslideing()
    {
        _isWallTouch = Physics2D.Raycast(_handTr.position, Vector2.right * _facingDir, 0.1f, wallLayerMask);
        return _isWallTouch;
    }

    IEnumerator RollCoolTimeCoroutine()
    {
        _isRollAble = false;
        float timer = 0.0f;
        while (timer < _rollCoolTime)
        {
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _isRollAble = true;
    }
    
}
