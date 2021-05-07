using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.Serialization;


// Unit은 외부에 보이는 인터페이스.
public class PlayerUnit : Unit
{
    // FSM에게 상태 전이를 전달해 주기 위한 이벤트
    public event UnityAction hitEvent;
    
    [SerializeField] 
    private Rigidbody2D _rigidbody2D;
    
    public Rigidbody2D Rigidbody2D => _rigidbody2D;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    // 방향 관련 변수
    private float _facingDir = 1;
    private bool _isFacingRight = true;
    
    public float FacingDir => _facingDir;

    // Ground Check
    [SerializeField]
    private bool _isGround = true;
    public bool IsGround => _isGround;

    [Header("Ground Check")]
    public Transform groundCheck;    // Ground체크를 위함
    public Vector2 groundCheckBoxSize;
    public LayerMask groundLayer;

    // 시간 관련 변수
    private float _timeScale = 1;
    
    ///////////////////////////// 데이터로 관리 할 변수
    // 기본 스탯
    [SerializeField]
    private float _maxHp;
    [SerializeField]
    private float _curHp;
    [SerializeField]
    private float _decreaseHp;
    [SerializeField] 
    private float _encroachment;
    
    // 이동 관련 변수
    [Header("Move Variable")]
    [SerializeField]
    private float _minSpeed = 0.8f;
    [SerializeField]
    private float _maxSpeed = 6.5f;
    [SerializeField]
    private float _moveStopSpeed = 1.0f;
    private float _curSpeed = 0.0f;

    private bool _isTouchMaxSpeed = false;
    
    // 점프 관련 변수
    private int _jumpCount = 0;
    private int _maxJumpCount = 2;
    private float _maxJumpTime = 0.1f;
    public float MaxJumpTime => _maxJumpTime;

    [Header("Jump Variable")]
    [SerializeField]
    private float _coyoteTime = 0.2f;
    private float _maxCoyoteTime = 0.2f; 
    [SerializeField]
    private float _jumpPower = 5.0f;
    [SerializeField] 
    private float _addAllJumpPpower = 8.0f;
    [SerializeField]
    private float _addJumpPower = 4f;
    
    // 구르기 관련 변수
    //[Header("Roll Variable")]
    //[SerializeField]
    private float _rollGoalX = 10;
    //[SerializeField]
    private float _rollTime = 0.0f;

    public float RollTime => _rollTime;
    
    private float _rollSpeed = 0.5f;
    //[SerializeField]
    private float _rollCoolTime;
    private bool _isRollAble = true;
    public bool IsRollAble => _isRollAble;
    
    //[SerializeField] 
    private PhysicsMaterial2D _dashPhysicMaterial;
    
    [Header("Dash Variable")]
    [SerializeField]
    private float _dashTime = 0;
    [SerializeField]
    private float _dashGoalX = 0;

    [SerializeField]
    private float _dashCoolTime;
    
    private bool _isDashAble = true;
    public bool isDashAble => _isDashAble;
    private float _dashSpeed = 0;
    public float DashTime => _dashTime;
    
    
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
    [SerializeField] 
    private float _wallSlideingGravityScale = 1;
    
    // BasicAttack Variable
    [Header("BasicAttack Variable")] 
    // [SerializeField]
    // private float _basicAttackTime = 0.5f;
    // public float BasicAttackTime => _basicAttackTime;r
    [SerializeField]
    private float _basicAttackDelay = 0.0f;
    public float BasicAttackDelay => _basicAttackDelay;

    [SerializeField] 
    private float[] _basicAttackMoveTimeArr;
    public float[] BasicAttackMoveTimeArr => _basicAttackMoveTimeArr;
    [SerializeField] 
    private float[] _basicAttackMoveGoalXArr;
    private float _basicAttackMoveSpeed = 0.0f;

    [SerializeField] 
    private float _basicAttackMinDamage;
    [SerializeField]
    private float _basicAtaackMaxDamage;


    private Damage _basicAttackDamage;
    [SerializeField]
    private BasicAttackCtrl[] _basicAttackCtrlArr;
    [SerializeField]
    private Vector2[] _basicAttackKnockBackArr;
    [SerializeField]
    private BasicAttackCtrl _basicJumpAttackCtrl;
    
    // hit Variable
    [Header("Hit Variable")]
    [SerializeField]
    private float _hitInvincibilityTime = 1.0f;
    [SerializeField] 
    private float _hitInvincibilityTwinkleTime = 0.5f;
    [SerializeField]
    private float _hitTime = 0.0f;
    public float HitTime => _hitTime;
    private bool _isInvincibility = false;

    // Skill Variable
    [Header("Skill Variable")]
    /// </기획 변동으로 인해 미사용>
    // // Shadow Walk
    // [SerializeField]
    // private ShadowWalkColCtrl _shadowWalkColCtrl;
    // [SerializeField]
    // private ShadowWalkSkillData _shadowWalkSkillData;
    // public Shadow shadowWalkShadow;
    //
    // public ShadowWalkSkillData ShadowWalkSkillData => _shadowWalkSkillData;
    //
    // private bool _isSkillShadowWalkAble = true;
    // private float _skillShadowWalkNumberOfTimes;
    // private float _skillShadowWalkCoolTime;
    /// </기획 변동으로 인해 미사용>

    [SerializeField, Header("SkillAxe")]
    private SkillAxeData _skillAxeData;
    public SkillAxeData SkillAxeData => _skillAxeData;
    private float _skillAxeNumberOfTimes;
    private float _skillAxeCoolTime;
    private bool _skillAexIsAble = true;

    [SerializeField, Header("SkillSpear")] 
    private SkillSpearData _skillSpearData;
    public SkillSpearData SkillSpearData => _skillSpearData;
    public SkillSpearAttackCtrl _skillSpearAttackCtrl;
    private float _skillSpearNumberOfTimes;
    private float _skillSpearCoolTime;
    private bool _skillSpearIsAble = true;
    
    
    public event UnityAction OnSkillSpearRushEvent = null;
    public event UnityAction OnSkillSpearAttackEvent = null;
    
    //////////////////////////// 데이터로 관리 할 변수

    private float _originalGravityScale = 0;
    
    private Vector2 _hitstopPrevVelocity = Vector2.zero;
    private Damage _hitDamage;

    [SerializeField, Header("")]
    private GameObject _SlideingFx;

    void Start()
    {
        Init();
        Bind();
    }

    void Init()
    {
        SetVariable(0.2f, 2, 0.4f);
    }

    private void Bind()
    {
        for (int i = 0; i < _basicAttackCtrlArr.Length; i++)
        {
            _basicAttackCtrlArr[i].OnChangeDirEvent += OnChangeDirEventCall;
        }

        _basicJumpAttackCtrl.OnChangeDirEvent += OnChangeDirEventCall;
    }

    private void UnBind()
    {
        
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

        _curHp = _maxHp;
        
        _basicAttackDamage = new Damage();
        _hitDamage = new Damage();
        
        // _skillShadowWalkNumberOfTimes = _shadowWalkSkillData.NumberOfTimesTheSkill;
        // _skillShadowWalkCoolTime = _shadowWalkSkillData.CoolTime;

        _skillAxeNumberOfTimes = _skillAxeData.NumberOfTimesTheSkill;
        _skillAxeCoolTime = _skillAxeData.CoolTime;

        _skillSpearNumberOfTimes = _skillSpearData.NumberOfTimesTheSkill;
        _skillSpearCoolTime = _skillSpearData.CoolTime;
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

    public override void Move()
    {
        _rigidbody2D.AddForce(new Vector2(_minSpeed * _facingDir, 0) * _timeScale, ForceMode2D.Impulse);

        if (Mathf.Abs(_rigidbody2D.velocity.x) >= _maxSpeed)
        {
            _rigidbody2D.velocity = new Vector2(_maxSpeed * _facingDir, _rigidbody2D.velocity.y);
            _isTouchMaxSpeed = true;
        }

        // Vector2 dir = moveUtil.moveforce(5);
        // Rigidbody2D.addfoce(dir);
    }

    public void SetVelocityMaxMoveSpeed()
    {
        _rigidbody2D.velocity = new Vector2(_maxSpeed * _facingDir, _rigidbody2D.velocity.y);
        _isTouchMaxSpeed = true;
    }

    public void MoveStop()
    {
        if (GameManager.instance.timeMng.IsBulletTime == false)
            _rigidbody2D.velocity = new Vector2(_moveStopSpeed * _facingDir, _rigidbody2D.velocity.y);
    }

    public void MoveEnd()
    {
        _isTouchMaxSpeed = false;
    }

    public bool IsRunningInertia()
    {
        //return Mathf.Abs(_rigidbody2D.velocity.x) >= _maxSpeed - 1.0f ? true : false;
        
        if (_isTouchMaxSpeed == true)
        {
            _isTouchMaxSpeed = false;
            return true;
        }

        return false;
        
    }


    public override void Jump()
    {
        _coyoteTime = -1.0f;
        _jumpCount--;
        _isGround = false;

        var power = new Vector3(0, _jumpPower * _timeScale, 0.0f);
        _rigidbody2D.AddForce(power, ForceMode2D.Impulse);
    }

    public void DoubleJump()
    {
        _jumpCount--;
        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
        var power = new Vector3(0, _jumpPower * _timeScale, 0.0f);
        _rigidbody2D.AddForce(power, ForceMode2D.Impulse);
    }

    public void JumpMoveStop()
    {
        _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
    }

    public void AddJumpForce()
    {
        _rigidbody2D.AddForce((new Vector2(0, _addJumpPower) * _addAllJumpPpower) * _timeScale * GameManager.instance.timeMng.FixedDeltaTime, ForceMode2D.Impulse);
    }

    public bool CheckIsJumpAble()
    {
        if (_coyoteTime > 0.0f)
        {
            _coyoteTime = -1;
            return true;
        }
        
        return false;
    }

    public void ResetJumpVal()
    {
        _jumpCount = _maxJumpCount;
        _coyoteTime = _maxCoyoteTime;
    }

    public void SetRoll()
    {
        _rigidbody2D.sharedMaterial = _dashPhysicMaterial;
        _rollSpeed = (1 / _rollTime) * _rollGoalX;
    }

    public void Roll()
    {
        _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
        var power = new Vector2((_rollSpeed) * _facingDir * _timeScale, 0);
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
    }
    
    
    public void SetDash()
    {
        _rigidbody2D.sharedMaterial = _dashPhysicMaterial;
        _dashSpeed = (1 / _dashTime) * _dashGoalX;
        _rigidbody2D.gravityScale = 0;

    }

    public void Dash()
    {
        _rigidbody2D.velocity = Vector2.zero;

        var power = new Vector2((_dashSpeed) * _facingDir * GameManager.instance.timeMng.TimeScale, 0);
        _rigidbody2D.AddForce(power, ForceMode2D.Impulse);
    }

    public void DashStop()
    {
        _rigidbody2D.velocity = new Vector2(0.0f, 0.0f);
    }

    public void EndDash()
    {
        _rigidbody2D.gravityScale = _originalGravityScale;
        _rigidbody2D.sharedMaterial = null;
        
        if (_isDashAble == true)
            StartCoroutine(DashCoolTimeCoroutine());
    }

    public void WallSlideing()
    {
        _rigidbody2D.gravityScale -= _wallSlideingUpPower;

        if (_rigidbody2D.gravityScale <= 0)
        {
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.gravityScale = 0;
        }
    }

    public void WallSlideStateStart()
    {
        _SlideingFx.SetActive(true);    // FxCtrl로 이전할 예정.
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.gravityScale = _wallSlideingGravityScale;
    }
    
    public void WallJump()
    {
        if (GameManager.instance.timeMng.IsBulletTime == false)
            _rigidbody2D.gravityScale = _originalGravityScale;
        
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.AddForce(new Vector2(_wallJumpPowerX * _facingDir * -1, _wallJumpPowerY) * _timeScale , ForceMode2D.Impulse);
        Flip();
    }

    public void WallEnd()
    {
        _SlideingFx.SetActive(false);
        _rigidbody2D.gravityScale = _originalGravityScale;
    }
    

    public void WallReset()
    {
        if (GameManager.instance.timeMng.IsBulletTime == false)
        {
            _rigidbody2D.gravityScale = _wallSlideingGravityScale;
        }
    }

    private void SetBasicDamage(int attackIndex)
    {
        _basicAttackDamage.power = Random.Range(_basicAttackMinDamage, _basicAtaackMaxDamage);
        _basicAttackDamage.knockBack = new Vector2(_basicAttackKnockBackArr[attackIndex].x * _facingDir, _basicAttackKnockBackArr[attackIndex].y);
        //============== 고재협이 편집함 ======================
        _basicAttackDamage.additionalInfo = attackIndex;
        //=====================================================
    }
    
    public void SetBasicAttack()
    {
        _rigidbody2D.sharedMaterial = _dashPhysicMaterial;
        _rigidbody2D.velocity = Vector2.zero;
    }

    public void BasicAttack(int attackIndex)
    {
        SetBasicDamage(attackIndex);
        _basicAttackCtrlArr[attackIndex].SetDamage(_basicAttackDamage);
        _basicAttackCtrlArr[attackIndex].Progress();
    }

    public void BasicAttackMove(int basicAttackIndex)
    {
        _basicAttackMoveSpeed = 
            (1 / _basicAttackMoveTimeArr[basicAttackIndex]) * _basicAttackMoveGoalXArr[basicAttackIndex];
        
        _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);

        var power = new Vector2((_basicAttackMoveSpeed) * _facingDir * _timeScale, 0);
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
        SetBasicDamage(3);
        _basicJumpAttackCtrl.SetDamage(_basicAttackDamage);
        _basicJumpAttackCtrl.Progress();
    }

    public void BasicJumpMove(int inputDir)
    {
        _rigidbody2D.AddForce(new Vector2(_minSpeed * inputDir * _timeScale, 0), ForceMode2D.Impulse);
        
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
        if(_isInvincibility == true)
            return;

        _hitDamage = inputDamage;
        hitEvent?.Invoke();
    }

    public void Hit()
    {
        _curHp -= _hitDamage.power * _decreaseHp;
        StartCoroutine(InvincibilityTimeCoroutine());
        
        if(_curHp < 0)
            Debug.LogError("플레이어 사망");
    }

    public void HitKnockBack()
    {
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.AddForce(_hitDamage.knockBack, ForceMode2D.Impulse);
    }

    public void ResetHitDamage()
    {
        _hitDamage = new Damage();
    }

    
    // public Shadow GetAbleShadowWalk()
    // {
    //     shadowWalkShadow = null;
    //     
    //     if (_skillShadowWalkNumberOfTimes > 0)
    //     {
    //         shadowWalkShadow = _shadowWalkColCtrl.CheckAreaInsideShadow();
    //
    //         if (shadowWalkShadow == null)
    //             return null;
    //         
    //         _skillShadowWalkNumberOfTimes--;
    //         _encroachment -= _shadowWalkSkillData.EncroachmentPer;
    //     }
    //     
    //     if (_skillShadowWalkNumberOfTimes <= 0)
    //     {
    //         if (_isSkillShadowWalkAble == true)
    //         {
    //             StartCoroutine(ShadowWalkCoolTimeCoroutine());
    //         }
    //     }
    //
    //     return shadowWalkShadow;
    // }

    // public GameSkillObject InvokeShadowWalkSkill()
    // {
    //     var skillObject = GameManager.instance.GameSkillMgr.GetSkillObject();
    //     if (skillObject == null)
    //         return null;
    //     
    //     skillObject.InitSkill(_shadowWalkSkillData.GetSkillController(skillObject, this));
    //     return skillObject;
    // }

    public bool IsAbleSkillAxe()
    {
        if (_skillAexIsAble == false)
        {
            return false;
        }

        _skillAxeNumberOfTimes--;

        if (_skillAxeNumberOfTimes <= 0)
        {
            StartCoroutine(SkillAxeCoolTimeCoroutine());
        }
        
        return true;
    }

    public bool IsAbleSkillSpear()
    {
        if (_skillSpearIsAble == false)
        {
            return false;
        }

        _skillSpearNumberOfTimes--;

        if (_skillSpearNumberOfTimes <= 0)
        {
            StartCoroutine(SkillSpearCoolTimeCoroutine());
        }

        return true;
    }
    
    public void OnSkillSpearRushEventCall()
    {
        OnSkillSpearRushEvent?.Invoke();
    }

    public void OnSkillSpearAttackEventCall()
    {
        OnSkillSpearAttackEvent?.Invoke();
    }

    public void SKillSpearAttack(Damage damage)
    {
        _skillSpearAttackCtrl.SetDamage(damage);
        _skillSpearAttackCtrl.Progress();
    }

    public void StartBulletTime(float timeScale)
    {
        _timeScale = timeScale;

        _maxSpeed = _maxSpeed * _timeScale;
        _rigidbody2D.velocity *= _timeScale;
        _rigidbody2D.gravityScale *= _timeScale * _timeScale;
    }

    public void EndBulletTime(float timeScale)
    {
        _maxSpeed = _maxSpeed * (1 / _timeScale);
        _rigidbody2D.velocity *= 1 / _timeScale; 

        _timeScale = timeScale;
        
        _rigidbody2D.gravityScale = _originalGravityScale; 
    }

    private bool _isHitstop = false;
    
    public void StartHitStop(float timeScale)
    {
        _timeScale = timeScale;
        _isHitstop = true;
        _hitstopPrevVelocity = _rigidbody2D.velocity;
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void EndHitStop(float timeScale)
    {
        _isHitstop = false;
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        _timeScale = timeScale;
        _rigidbody2D.velocity = _hitstopPrevVelocity;
    }

    public Vector3 GetVelocity()
    {
        return _rigidbody2D.velocity;
    }

    private void OnChangeDirEventCall()
    {
        Flip();
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
            _coyoteTime -= GameManager.instance.timeMng.FixedDeltaTime;
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

    IEnumerator HitstopMoveCoroutine()
    {
        while (_isHitstop)
        {
            _rigidbody2D.velocity = _hitstopPrevVelocity;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator RollCoolTimeCoroutine()
    {
        _isRollAble = false;
        float timer = 0.0f;
        while (timer < _rollCoolTime)
        {
            timer += GameManager.instance.timeMng.FixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _isRollAble = true;
    }
    
    IEnumerator DashCoolTimeCoroutine()
    {
        _isDashAble = false;
        float timer = 0.0f;
        while (timer < _dashCoolTime)
        {
            timer += GameManager.instance.timeMng.FixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _isDashAble = true;
    }

    IEnumerator SkillAxeCoolTimeCoroutine()
    {
        _skillAexIsAble = false;
        float timer = 0.0f;

        while (timer < _skillAxeCoolTime)
        {
            timer += GameManager.instance.timeMng.FixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _skillAexIsAble = true;
        _skillAxeNumberOfTimes = _skillAxeData.NumberOfTimesTheSkill;
    }
    
    IEnumerator SkillSpearCoolTimeCoroutine()
    {
        _skillSpearIsAble = false;
        float timer = 0.0f;

        while (timer < _skillSpearCoolTime)
        {
            timer += GameManager.instance.timeMng.FixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _skillSpearIsAble = true;
        _skillSpearNumberOfTimes = _skillSpearData.NumberOfTimesTheSkill;
    }


    // IEnumerator ShadowWalkCoolTimeCoroutine()
    // {
    //     _isSkillShadowWalkAble = false;
    //     float timer = 0.0f;
    //     while (timer < _shadowWalkSkillData.CoolTime)
    //     {
    //         timer += GameManager.instance.timeMng.FixedDeltaTime;
    //         yield return new WaitForFixedUpdate();
    //     }
    //
    //     _isSkillShadowWalkAble = true;
    //     _skillShadowWalkNumberOfTimes = _shadowWalkSkillData.NumberOfTimesTheSkill;
    // }
    
    IEnumerator InvincibilityTimeCoroutine()
    {
        _isInvincibility = true;
        float _totalTick = 0.0f;
        float _tick = 0.0f;
        int count = 0;
        
        _spriteRenderer.color = new Color32(255, 255, 255, 127);
        
        while (_totalTick <= _hitInvincibilityTime)
        {
            _totalTick += GameManager.instance.timeMng.FixedDeltaTime;
            if (_totalTick >= _hitInvincibilityTime * _hitInvincibilityTwinkleTime)
            {

                _tick += GameManager.instance.timeMng.FixedDeltaTime;
                if (_tick >= 0.1f)
                {
                    count++;

                    if (count % 2 == 1)
                    {
                        _spriteRenderer.color = new Color32(255, 255, 255, 127);
                    }
                    else
                    {
                        _spriteRenderer.color = new Color32(255, 255, 255, 255);
                    }
                    _tick = 0;
                }

            }
            
            yield return new WaitForFixedUpdate();
        }
        _spriteRenderer.color = new Color32(255, 255, 255, 255);
        _isInvincibility = false;
    }
    
}
