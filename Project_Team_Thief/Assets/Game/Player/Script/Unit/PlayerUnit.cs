using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PS.FX;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


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

    //---
    public SOPlayer playerInfo;
    //---

    private List<Dictionary<string, object>> _playerData;
    
    ///////////////////////////// 데이터로 관리 할 변수

    /// 스테이트 레벨
    private int _maxHpStateLevel = 1;
    private int _moveSppedStateLevel = 1;
    private int _jumpPowerStateLevel = 1;
    private int _dashRangeStateLevel = 1;
    private int _dashCoolTimeStateLevel = 1;
    private int _deathCountStateLevel = 1;
    private int _deathHPStateLevel = 1;
    private int _criticalStateLevel = 1;
    private int _attackSppedStateLevel = 1;
    private int _defStateLevel = 1;
    
    
    // 기본 스탯
    [SerializeField]
    private float _maxHp;
    [SerializeField]
    private float _curHp;
    private float _decreaseHp;
    public float DecreaseHp => _decreaseHp;

    private float def;
    
    // 특수 효과 관련 변수
    private bool _isSuperArmor = false;
    public bool IsSuperArmor => _isSuperArmor;
    
    [Header("Encroachment")]
    private float _encroachment;
    public float Encroachment => _encroachment;
    
    [SerializeField, Tooltip("잠식력에 비례해서 플레이어가 더 맞는 최대 보정 값(1.0 미만)")]
    private float _encroachmentPerPlayerHitDamageMax;

    [SerializeField, Tooltip("잠식력에 비례해서 플레이어가 더 때리는 데미지 최대 보정 값(1.0 미만)")]
    private float _encroachmentPerPlayerAttackDamageMax;
    public float EncroachmentPerPlayerAttackDamageMax => _encroachmentPerPlayerAttackDamageMax;

    [SerializeField, Tooltip("몇 초마다 잠식력이 회복 될 지")]
    private float _encroachmentRecoveryPerTime;

    [SerializeField, Tooltip("잠식력이 얼마나 회복 될 지")]
    private float _encroachmentRecoveryAmount;
    [SerializeField]
    private float _encroachmentProductionFadeOutTime;
    
    [SerializeField]
    private ParticleSystem[] _encroachmentProductionParticleSystems;

    [SerializeField] 
    private EffectController _encrochmentDeadEffectController;
    //private bool _nonEncroachment = true;
    private bool[] _encroachmentLevelArr = new bool[5] {false, false, false, false, false};
    //private int _encroachmentLevelIndex = 0;
    private bool _isEncrochmentDeadCoroutine = false;
    [SerializeField]
    private float _waitEncrochmentDeadTime = 0.0f;
    private Color _encroachmentColor;
    
    [SerializeField, Header("Ani Variable")]
    private float _aniFastAmount;
    public float AniFastAmount => _aniFastAmount;
    
    // 이동 관련 변수
    [Header("Move Variable")]
    private float _moveSpeed;
    public float MoveSpped => _moveSpeed;
    
    // 점프 관련 변수
    [Header("Jump Variable")]
    [SerializeField]
    private float _coyoteTime = 0.2f;
    private float _maxCoyoteTime = 0.2f; 
    private float _jumpPower = 5.0f;

    private int _jumpCount = 0;
    private int _maxJumpCount = 2;

    //[SerializeField] 
    private PhysicsMaterial2D _dashPhysicMaterial;
    
    [Header("Dash Variable")]
    [SerializeField]
    private float _dashTime = 0;
    [SerializeField]
    private float _dashGoalX = 0;

    
    private bool _isDashAble = true;
    public bool isDashAble => _isDashAble;
    private float _dashSpeed = 0;
    public float DashTime => _dashTime;
    private float _dashRange;
    private float _dashCoolTime;

    // DeathRevers Variable
    private int _deathCount;
    private float _deathHp;

    // Attack Variable
    private float _critical;
    private float _criticalAmount = 1.5f;
    private float _addDamage;
    private float _decreaseDamage;
    private float _attackSpeed;
    

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
    
    [Header("BasicAttack Variable")]
    [SerializeField]
    private BasicAttackCtrl _basicAttackCtrl;
    [SerializeField]
    private BasicAttackCtrl[] _basicJumpAttackCtrlArr;
    [SerializeField]
    private float _basicJumpAttackTime;
    public float BasicJumpAttackTime => _basicJumpAttackTime;
    [SerializeField] 
    private float _basicAttackCancelTime;
    public float BasicAttackCancelTime => _basicAttackCancelTime;
    public bool isBasicJumpAttackAble = true;

    [Header("Combo Variable")]
    [SerializeField]
    private float _comboTime;
    [SerializeField]
    private int _comboRecoveryAmount;
    
    private float _comboTimer;
    private int _curCombo = 0;
    private int _comboRemainder = 0;
    private int _drainSaveCombo = 0;
    private int _drainCombo = 0;
    private bool _isContinuingCombo = false;
    private Coroutine _comboCoroutine;
    
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
    private float _def;
    

    [Header("Encroachment Variable")] 
    [SerializeField]
    private int _encroachmentDecreaseCombo;
    [SerializeField]
    private int _baiscAttackEncroachmentDecrease;


    [Header("SkillDoubleCross")] [SerializeField]
    private AttackBase[] _skillDoubleCrossAttackBase;

    public AttackBase[] SkillDoubleCrossAttackBase => _skillDoubleCrossAttackBase;

    public event UnityAction OnSkillDoubleCrossAttackEvent = null;

    [Header("SkillSnakeSwordSting")]
    [SerializeField]
    private AttackBase[] _skillSnakeSwordStingAttackBase;
    public AttackBase[] SkillSnakeSwordStingAttackBase => _skillSnakeSwordStingAttackBase;
    public event UnityAction OnSkillSnakeSwordStingAttackEvent = null;

    [Header("SkillSnakeSwordFlurry")]
    [SerializeField]
    private AttackBase _skillSnakeSwordFlurryAttackBase;
    public AttackBase SkillSnakeSwordFlurryAttackBase => _skillSnakeSwordFlurryAttackBase;
    public event UnityAction skillSnakeSwordFlurryEndEvent = null;
    
    [Header("SkillBaldo")]
    [SerializeField]
    private AttackBase _skillBaldoAttackBase;
    public AttackBase SkillBaldoAttackBase => _skillBaldoAttackBase;
    public event UnityAction OnSkillBaldoAttackEvent = null;

    [Header("SkillSheating")] 
    [SerializeField]
    private AttackBase _skillSheatingAttackBase;
    public AttackBase SkillSheatingAttackBase => _skillSheatingAttackBase;
    public event UnityAction OnSkillSheatingAttackEvent = null;

    [Header("MagicMissile")] 
    [SerializeField]
    private AttackBase _skillMagicMissileAttackBase;
    public AttackBase SkillMagicMissileAttackBase => _skillMagicMissileAttackBase;


    //////////////////////////// 데이터로 관리 할 변수
    
    //  BlessingPenalty Variable
    private bool _isLivingHard = false;
    public bool IsLivingHard => _isLivingHard;
    //

    private float _originalGravityScale = 0;

    public float OriginalGravityScale => _originalGravityScale;

    private Vector2 _hitstopPrevVelocity = Vector2.zero;
    private Damage _hitDamage;

    private bool _isPlayerDead = false;
    public bool IsPlayerDead => _isPlayerDead;
    public event UnityAction OnPlayerDeadEvent;

    private int _mapCount = 0;
    public int MapCount => _mapCount; 
    
    [SerializeField, Header("")]
    private GameObject _SlideingFx;
    
       
    // Zoom In and Out Event
    public UnityAction ZoomInEvent;
    public UnityAction ZoomOutEvent;

    void Start()
    {
        LoadPlayerData();
        
        Init();
        Bind();
    }

    void Init()
    {
        SetVariable();
        GameManager.instance.AddMapEndEventListener(MapEndEventCall);
    }

    private void LoadPlayerData()
    {
        _playerData = CSVReader.Read("PlayerData");
    }

    private void Bind()
    {
        _basicAttackCtrl.OnEnemyHitEvent += OnAddComboEventCall;

        for (int i = 0; i < _basicJumpAttackCtrlArr.Length; i++)
        {
            _basicJumpAttackCtrlArr[i].OnEnemyHitEvent += OnAddComboEventCall;
        }
    }

    private void UnBind()
    {
        _basicAttackCtrl.OnEnemyHitEvent -= OnAddComboEventCall;

        for (int i = 0; i < _basicJumpAttackCtrlArr.Length; i++)
        {
            _basicJumpAttackCtrlArr[i].OnEnemyHitEvent -= OnAddComboEventCall;
        }
        
    }
    
    // 향후에는 데이터 센터 클래스라던가 데이터를 가지고 있는 함수에서 직접 호출로 받아 올 수 있도록
    // 수정 할 예정
    public void SetVariable()
    {
        _maxCoyoteTime = 0.2f;

        _jumpCount = _maxJumpCount;
        _coyoteTime = _maxCoyoteTime;

        _originalGravityScale = _rigidbody2D.gravityScale;

        _maxHp = (float)Convert.ToDouble(GetDataFromStateLevel(_maxHpStateLevel, "maxHp"));
        _moveSpeed = Convert.ToInt32(GetDataFromStateLevel(_moveSppedStateLevel, "moveSpeed"));
        _jumpPower = Convert.ToInt32(GetDataFromStateLevel(_jumpPowerStateLevel, "jumpPower"));
        _dashRange = Convert.ToInt32(GetDataFromStateLevel(_dashRangeStateLevel, "dashRange"));
        _dashCoolTime = (float)Convert.ToDouble(GetDataFromStateLevel(_dashCoolTimeStateLevel, "dashCoolTime"));
        _deathCount = Convert.ToInt32(GetDataFromStateLevel(_deathCountStateLevel, "deathCount"));
        _deathHp = (float)Convert.ToDouble(GetDataFromStateLevel(_deathHPStateLevel, "deathHP"));
        _critical = Convert.ToInt32(GetDataFromStateLevel(_criticalStateLevel, "critical"));
        _attackSpeed = (float)Convert.ToDouble(GetDataFromStateLevel(_attackSppedStateLevel, "attackSpeed"));
        _def = (float)Convert.ToDouble(GetDataFromStateLevel(_defStateLevel, "def"));
        _maxJumpCount = 2;
        _addDamage = 0;
        _decreaseDamage = 1;
        _curHp = _maxHp;
        _decreaseHp = 1;

        //---
        playerInfo.CurHP = _curHp;
        playerInfo.MaxHP = _maxHp;
        playerInfo.CurEncroachment = _encroachment;
        playerInfo.MaxEncroachment = 100;
        //---
        
        _hitDamage = new Damage();

    }


    private object GetDataFromStateLevel(int stateLevel, string dataName)
    {
        int index = 0;
        
        for (int i = 0; i < _playerData.Count; i++)
        {
            if ((int)_playerData[i]["stateLevel"] == stateLevel)
            {
                index = i;
                break;
            }
        }

        return _playerData[index][dataName];
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
        _rigidbody2D.velocity = new Vector2(_moveSpeed * _facingDir, _rigidbody2D.velocity.y);
        //_rigidbody2D.AddForce(new Vector2(_minSpeed * _facingDir, 0) * GameManager.instance.timeMng.TimeScale, ForceMode2D.Impulse);

        // if (Mathf.Abs(_rigidbody2D.velocity.x) >= _maxSpeed)
        // {
        //     _rigidbody2D.velocity = new Vector2(_maxSpeed * _facingDir, _rigidbody2D.velocity.y);
        //     _isTouchMaxSpeed = true;
        // }

        // Vector2 dir = moveUtil.moveforce(5);
        // Rigidbody2D.addfoce(dir);
    }

    // public void SetVelocityMaxMoveSpeed()
    // {
    //     _rigidbody2D.velocity = new Vector2(_maxSpeed * _facingDir, _rigidbody2D.velocity.y);
    //     _isTouchMaxSpeed = true;
    // }

    public void MoveStop()
    {
        if (GameManager.instance.TimeMng.IsBulletTime == false)
        {
            _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
        }
        // if (GameManager.instance.timeMng.IsBulletTime == false)
        //     _rigidbody2D.velocity = new Vector2(_moveStopSpeed * _facingDir, _rigidbody2D.velocity.y);
    }
    

    // public bool IsRunningInertia()
    // {
    //     //return Mathf.Abs(_rigidbody2D.velocity.x) >= _maxSpeed - 1.0f ? true : false;
    //     
    //     if (_isTouchMaxSpeed == true)
    //     {
    //         _isTouchMaxSpeed = false;
    //         return true;
    //     }
    //
    //     return false;
    // }


    public override void Jump()
    {
        _coyoteTime = -1.0f;
        _jumpCount--;
        _isGround = false;

        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
        
        var power = new Vector3(0, _jumpPower * GameManager.instance.TimeMng.TimeScale, 0.0f);
        _rigidbody2D.AddForce(power, ForceMode2D.Impulse);
    }

    // public void DoubleJump()
    // {
    //     _jumpCount--;
    //     _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
    //     var power = new Vector3(0, _jumpPower * GameManager.instance.timeMng.TimeScale, 0.0f);
    //     _rigidbody2D.AddForce(power, ForceMode2D.Impulse);
    // }

    public bool CheckIsJumpAble()
    {
        if (_coyoteTime > 0.0f)
        {
            _coyoteTime = -1;
            return true;
        }
        
        return false;
    }

    public bool CheckIsDoubleJump()
    {
        if (_jumpCount >= 1)
        {
            return true;
        }

        return false;
    }

    public void ResetJumpVal()
    {
        _jumpCount = _maxJumpCount;
        isBasicJumpAttackAble = true;
        _coyoteTime = _maxCoyoteTime;
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

        var power = new Vector2((_dashSpeed) * _facingDir * GameManager.instance.TimeMng.TimeScale, 0);
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

    public void SetPlayerPhysicMaterial()
    {
        _rigidbody2D.sharedMaterial = _dashPhysicMaterial;
    }

    public void SetPlayerDefaultPhysicMaterial()
    {
        _rigidbody2D.sharedMaterial = null;
    }
    
    public void WallJump()
    {
        if (GameManager.instance.TimeMng.IsBulletTime == false)
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
        if (GameManager.instance.TimeMng.IsBulletTime == false)
        {
            _rigidbody2D.gravityScale = _wallSlideingGravityScale;
        }
    }

    private Damage CalcDamageAbnormalIsDrain(Damage damage)
    {
        if (_comboRecoveryAmount == 0)
        {
            return damage;
        }

        _drainCombo = _curCombo;
        
        if (_drainCombo == 0 || (_drainCombo - _drainSaveCombo) <= 19)
        {
            damage.abnormal = 0;
            return damage;
        }


        
        _comboRemainder = (_drainCombo - _drainSaveCombo) & _comboRecoveryAmount;

        if (_comboRemainder >= 0)
        {
            _drainSaveCombo = _drainCombo;
            damage.abnormal = (int)AbnormalState.Spare8;
        }

        _comboRemainder = 0;
        
        return damage;
    }

    public float GetDamageWeightFromEencroachment()
    {
        return _encroachmentPerPlayerAttackDamageMax + (1 + (1 + _encroachment) / 100);
    }

    public float GetHitDamageWeightFromEncroachment()
    {
        return _encroachmentPerPlayerHitDamageMax + (1 + (1 + _encroachment) / 100);
    }

    public void SetBasicAttack()
    {
        _rigidbody2D.sharedMaterial = _dashPhysicMaterial;
        _rigidbody2D.velocity = Vector2.zero;
    }

    // 데미지에 추가 보정을 해야하는 경우 여기서 수정해서 넘기면 될 듯
    public void BasicAttack(Damage damage)
    {
        //SetBasicDamage(attackIndex);
        _basicAttackCtrl.Init(damage);
        _basicAttackCtrl.Progress();
    }
    
    
    public void EndBasicAttack()
    {
        _rigidbody2D.sharedMaterial = null;
    }
    
    public void BasicAttackMoveStop()
    {
        //if (IsGround)
        _rigidbody2D.velocity = new Vector2(0.0f, 0.0f);
    }
    
    public void BasicJumpAttack(int jumpAttackIndex, Damage damage)
    {
        _basicJumpAttackCtrlArr[jumpAttackIndex].Init(damage);
        _basicJumpAttackCtrlArr[jumpAttackIndex].Progress();
    }

    public void SetJumpAttackMove()
    {
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.gravityScale = 0;
    }

    public void EndJumpAttackMove()
    {
        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.gravityScale = _originalGravityScale;
    }
    
    // public void BasicJumpAttackMove(int basicJumpAttackIndex)
    // {
    //     _basicJumpAttackMoveSpeed = 
    //         (1 / _basicJumpAttackMoveTimeArr[basicJumpAttackIndex]) *
    //         _basicJumpAttackMoveGoalArr[basicJumpAttackIndex].x;
    //     
    //     float _basicJumpAttackMoveYSpeed = (1 / _basicJumpAttackMoveTimeArr[basicJumpAttackIndex]) *
    //                                        _basicJumpAttackMoveGoalArr[basicJumpAttackIndex].y;
    //     
    //     _rigidbody2D.velocity = new Vector2(0, 0);
    //
    //     var power = new Vector2((_basicJumpAttackMoveSpeed) * _facingDir * GameManager.instance.timeMng.TimeScale,
    //         _basicJumpAttackMoveYSpeed * GameManager.instance.timeMng.TimeScale);
    //     _rigidbody2D.AddForce(power, ForceMode2D.Impulse);
    // }
    

    public void BasicJumpMove(int inputDir)
    {
        _rigidbody2D.velocity = new Vector2(_moveSpeed * inputDir, _rigidbody2D.velocity.y);
        
        // _rigidbody2D.AddForce(new Vector2(_minSpeed * inputDir * GameManager.instance.timeMng.TimeScale, 0), ForceMode2D.Impulse);
        //
        // if (Mathf.Abs(_rigidbody2D.velocity.x) >= _maxSpeed)
        //     _rigidbody2D.velocity = new Vector2(_maxSpeed * inputDir, _rigidbody2D.velocity.y);
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
        //_hitDamage.power = _hitDamage.power;
        hitEvent?.Invoke();
    }

    public void Hit()
    {
        _curHp -= _hitDamage.power * _decreaseHp;
        HitKnockBack();
        StartCoroutine(InvincibilityTimeCoroutine());

        //---
        playerInfo.CurHP = _curHp;
        //---

        if ((int)_curHp <= 0)
        {
            _curHp = 0;
            playerInfo.CurHP = _curHp;
            Dead();
        }
    }

    public override void HandleHpRecovery(Damage damage)
    {
        _curHp += damage.power;

        if (_curHp >= 100)
        {
            _curHp = 100;
        }
        
        playerInfo.CurHP = _curHp;
    }

    private void Dead()
    {
        _isPlayerDead = true;
        OnPlayerDeadEvent?.Invoke();
        StopAllCoroutines();
        UnBind();
    }
    

    public void HitKnockBack()
    {
        if (_isSuperArmor == true)
        {
            return;
        }

        _rigidbody2D.velocity = Vector2.zero;
        _rigidbody2D.AddForce(_hitDamage.knockBack, ForceMode2D.Impulse);
    }

    public void ResetHitDamage()
    {
        _hitDamage = new Damage();
    }
    
    
    private void ChangeEncroachment(float encroachmentIncrease)
    {
        playerInfo.CurEncroachment = _encroachment;
    }

    public void OnSkillDoubleCrossAttackEventCall()
    {
        OnSkillDoubleCrossAttackEvent?.Invoke();
    }
    
    public void OnSkillSnakeSwordStingAttackEventCall()
    {
        OnSkillSnakeSwordStingAttackEvent?.Invoke();
    }

    public void OnSkillBaldoAttackEventCall()
    {
        OnSkillBaldoAttackEvent?.Invoke();
    }

    public void OnSkillSheatingAttackEventCall()
    {
        OnSkillSheatingAttackEvent?.Invoke();
    }
    

    public void SkillSnakeSwordFlurryEnd()
    {
        skillSnakeSwordFlurryEndEvent?.Invoke();
    }
    

    //  리팩토링 할 때  skillData를 다 따로 가지고 있지 말고
    // skillDataBase 리스트를 하나 만들어서 거기 담아놓자.
    public void OnAddComboEventCall()
    {
        _curCombo++;

        if (_isContinuingCombo == true)
        {
            _comboTimer = _comboTime;
        }
        else
        {
            _comboCoroutine = StartCoroutine(ComboCoroutine());    
        }
        
        // 기능 삭제
        // if (_curCombo >= _encroachmentDecreaseCombo)
        // {
        //     FindEncroachmentDecreaseFromSkillData(skillname);
        // }

        GameManager.instance.UIMng.SetCombo(_curCombo);
    }
    
    public void StartBulletTime(float timeScale)
    {
        _timeScale = timeScale;

        //_moveSpeed = _moveSpeed * _moveSpeed;
        //_maxSpeed = _maxSpeed * _timeScale;
        _rigidbody2D.velocity *= _timeScale;
        _rigidbody2D.gravityScale *= _timeScale * _timeScale;
    }

    public void EndBulletTime(float timeScale)
    {
        //_moveSpeed = _moveSpeed * (1 / _timeScale);
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

        
        // 현재 잠식력 연출이 쉐이더로 작동하기 때문에 파티클 시스템을 건들여도 멈추지 않음
        // AD랑 얘기해봐야할 듯.
        // ParticleSystem[] particleSystems;
        // ParticleSystem.MainModule mainModule;
        //
        // for (int i = 0; i < _encroachmentProductionParticleSystems.Length; i++)
        // {
        //     particleSystems = _encroachmentProductionParticleSystems[i].GetComponentsInChildren<ParticleSystem>();
        //     foreach (var item in particleSystems)
        //     {
        //         mainModule = item.main;
        //         mainModule.simulationSpeed = 0;
        //     }
        // }
    }

    public void EndHitStop(float timeScale)
    {
        _isHitstop = false;
        _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        _timeScale = timeScale;
        _rigidbody2D.velocity = _hitstopPrevVelocity;
        
        // ParticleSystem[] particleSystems;
        // ParticleSystem.MainModule mainModule;
        
        // for (int i = 0; i < _encroachmentProductionParticleSystems.Length; i++)
        // {
        //     particleSystems = _encroachmentProductionParticleSystems[i].GetComponentsInChildren<ParticleSystem>();
        //     foreach (var item in particleSystems)
        //     {
        //         mainModule = item.main;
        //         mainModule.simulationSpeed = 1;
        //     }
        // }
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
            _coyoteTime -= GameManager.instance.TimeMng.FixedDeltaTime;
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
    

    private void MapEndEventCall()
    {
        _mapCount++;
    }

    public void ActiveSuperArmor(float superArmorTime)
    {
        StartCoroutine(SuperArmorCoroutine(superArmorTime));
    }

    public void ChangeCritical(float amount)
    {
        _critical += amount;
    }

    public float CalcCriticalOutDamage(float power)
    {
        float rand = UnityEngine.Random.Range(0, 1.0f);

        if (_critical > rand)
        {
            return power * _criticalAmount;
        }
        else
        {
            return power;
        }
    }

    public float CalcSkillDamage(float power)
    {
        float damage = power + (power * (_addDamage / 100));
        return CalcCriticalOutDamage(damage) * _decreaseDamage;
    }
    
    // BlessingPenalty

    public void MaxHpDegradation(int maxHpDegradationAmount)
    {
        _maxHp -= maxHpDegradationAmount;
        playerInfo.MaxHP = _maxHp;

    }

    public void ChangeMoveSpeed(float changeMoveSpeed)
    {
        Debug.Log("Be _moveSpeed : " + _moveSpeed);

        _moveSpeed *= changeMoveSpeed;
        Debug.Log("Af _moveSpeed : " + _moveSpeed);

    }

    public void ApplyLivingHard()
    {
        _isLivingHard = true;
    }

    public void TurnOffLivingHard()
    {
        _isLivingHard = false;
    }

    public void ChangeDamage(float changeDamageAmount)
    {
        _decreaseDamage *= changeDamageAmount;
    }

    public void ChangeDecreaseHp(float decreaseHpAmount)
    {
        _decreaseHp = decreaseHpAmount;
    }
    
    //

    public void OnZoomInEventCall()
    {
        ZoomInEvent.Invoke();
    }

    public void OnZoomOutEventCall()
    {
        ZoomOutEvent.Invoke();
    }

    IEnumerator ComboCoroutine()
    {
        _isContinuingCombo = true;
        _comboTimer = _comboTime;


        while (_comboTimer >= 0)
        {
            _comboTimer -= GameManager.instance.TimeMng.FixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        _isContinuingCombo = false;
        _curCombo = 0;
        _drainSaveCombo = 0;
    }

    IEnumerator HitstopMoveCoroutine()
    {
        while (_isHitstop)
        {
            _rigidbody2D.velocity = _hitstopPrevVelocity;
            yield return new WaitForFixedUpdate();
        }
    }
    
    
    IEnumerator DashCoolTimeCoroutine()
    {
        _isDashAble = false;
        float timer = 0.0f;
        while (timer < _dashCoolTime)
        {
            timer += GameManager.instance.TimeMng.FixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _isDashAble = true;
    }

    IEnumerator EncroachmentDaedProduction()
    {
        float timer = 0.0f;
        _isEncrochmentDeadCoroutine = true;
        
        WwiseSoundManager.instance.PlayEventSound("Encroachment_Die");
        _encrochmentDeadEffectController.gameObject.SetActive(true);
        _encrochmentDeadEffectController.Play();

        yield return null;
        
        
        
        while (timer <= _waitEncrochmentDeadTime)
        {
            timer += GameManager.instance.TimeMng.FixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        ParticleSystem.MainModule mainModule = new ParticleSystem.MainModule();
        
        foreach (var item in _encrochmentDeadEffectController.particleSystems)
        {
            mainModule = item.main;
            mainModule.simulationSpeed = 0.0f;
        }
        
        Dead();
    }

    IEnumerator SuperArmorCoroutine(float superArmorTime)
    {
        float timer = 0.0f;
        _isSuperArmor = true;
        
        while (superArmorTime >= timer)
        {
            timer += GameManager.instance.TimeMng.FixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _isSuperArmor = false;
    }
    
    IEnumerator InvincibilityTimeCoroutine()
    {
        _isInvincibility = true;
        float _totalTick = 0.0f;
        float _tick = 0.0f;
        int count = 0;
        
        _spriteRenderer.color = new Color32(255, 255, 255, 127);
        
        while (_totalTick <= _hitInvincibilityTime)
        {
            _totalTick += GameManager.instance.TimeMng.FixedDeltaTime;
            if (_totalTick >= _hitInvincibilityTime * _hitInvincibilityTwinkleTime)
            {

                _tick += GameManager.instance.TimeMng.FixedDeltaTime;
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
