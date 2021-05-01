using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using LightWarrior;
using UnityEngine;
using UnityEngine.Events;

public abstract class CustomFSMStateBase : IFSMStateBase
{
    private PlayerFSMSystem _systemMgr = null;

    public PlayerFSMSystem SystemMgr => _systemMgr;

    public CustomFSMStateBase(PlayerFSMSystem system)
    {
        _systemMgr = system;
    }

    public abstract void StartState();

    public abstract void Update();

    public abstract void EndState();

    public abstract bool Transition(TransitionCondition condition);

    public abstract bool InputKey(TransitionCondition condition);
}

//FSM은 스테이트 닦이가 아니고 스테이트들이 공통적으로 필요로 하는 함수 가져도 OK
// OnTrriger와 같은 유니티 이벤트 함수 가져도 OK.
public class PlayerFSMSystem : FSMSystem<TransitionCondition, CustomFSMStateBase>, IActor
{
    [SerializeField]
    private PlayerUnit _unit = null;

    public PlayerUnit Unit => _unit;

    [SerializeField] 
    private AnimationCtrl _animationCtrl;

    [SerializeField]
    private FxCtrl _fxCtrl;

    public AnimationCtrl AnimationCtrl => _animationCtrl;

    [SerializeField]
    private BattleIdleCtrl _battleIdleCtrl;

    // 애니메이션 관련 상태 변수
    public bool isJumpKeyPress = false;
    private bool _isBattleIdle = false;
    private bool _beforeFalling = false;
    
    // 각 상태와 연결 될 유니티 이벤트
    // 애니메이션 관련 이벤트
    public event UnityAction OnAnimationEndEvent = null;
    
    // 기본 공격 관련
    public event UnityAction OnBasicAttackEndAniEvent = null;
    public event UnityAction OnBasicAttackCallEvent = null;

    // Start is called before the first frame update
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        Bind();
        
        //==================== 고재협이 편집함 ==================
        GameManager.instance.shadow.RegistCollider(GetComponent<BoxCollider2D>());
        //=======================================================
    }

    private void Bind()
    {
        GameManager.instance.SetControlUnit(this);
        GameManager.instance.timeMng.startBulletTimeEvent += StartBulletTimeEvnetCall;
        GameManager.instance.timeMng.endBulletTimeEvent += EndBulletTimeEventCall;
        GameManager.instance.timeMng.startHitstopEvent += StartHitStopEventCall;
        GameManager.instance.timeMng.endHitstopEvent += EndHitStopEvnetCall;
        Unit.hitEvent += UnitHitEventCall;
        _battleIdleCtrl.OnIsBattleIdleEvent += OnIsBattleIdleEventCall;

        GameManager.instance.commandManager.OnCommandCastEvent += OnCommandCastEventCall;
    }

    private class IdleState : CustomFSMStateBase
    {
        public IdleState(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            if (SystemMgr._isBattleIdle == true)
                SystemMgr.AnimationCtrl.PlayAni(AniState.BattleIdle);
            else
                SystemMgr.AnimationCtrl.PlayAni(AniState.Idle);

            SystemMgr._beforeFalling = false;
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
            
            if(SystemMgr.Unit.GetVelocity().y <= -0.1f)
                SystemMgr.Transition(TransitionCondition.Falling);
        }

        public override void EndState()
        {
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Wallslideing)
                return false;
            if (condition == TransitionCondition.None)
                return false;
            return true;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            return true;
        }
    }
    // 스테이트 단계에서 상태를 바꿀 지 체크를 하면서 변경 가능 상태가 되면 bool 값을 바꾸는 형태
    // 로 하든 머 어떻게 컨트롤 해서 그 때부터 키매니저가 주는 상태 전이 이벤트를 받을 수 있도록.
    private  class MoveState : CustomFSMStateBase
    {
        float _inputDir = 1;

        public MoveState(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            SystemMgr.AnimationCtrl.PlayAni(AniState.Move);
            
            if (SystemMgr._beforeFalling == true)
            {
                SystemMgr.Unit.SetVelocityMaxMoveSpeed();
                SystemMgr._beforeFalling = false;
            }

        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
            
            if(SystemMgr.Unit.GetVelocity().y <= -0.1f)
                SystemMgr.Transition(TransitionCondition.Falling);
        }

        public override void EndState()
        {
            //SystemMgr.Unit.MoveEnd();
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.None)
            {
                SystemMgr.Transition(TransitionCondition.StopMove);
                return false;
            }

            if (condition == TransitionCondition.Wallslideing)
                return false;
            
            return true;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            if (condition == TransitionCondition.LeftMove)
            {
                SystemMgr.Unit.CheckMovementDir(_inputDir * - 1);
                SystemMgr.Unit.Move();
                return false;
            }
            else if (condition == TransitionCondition.RightMove)
            {
                SystemMgr.Unit.CheckMovementDir(_inputDir);
                SystemMgr.Unit.Move();
                return false;
            }
            return true;
        }
        
    }
    
    private class StopMoveState : CustomFSMStateBase
    {
        public StopMoveState(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            if (SystemMgr.Unit.IsRunningInertia())
                SystemMgr.Transition(TransitionCondition.RunningInertia);
            else
                SystemMgr.Transition(TransitionCondition.Idle);
            
            SystemMgr.Unit.MoveStop();
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();   
        }

        public override void EndState()
        {
            
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Wallslideing)
                return false;

            if (condition == TransitionCondition.None)
                return false;
            
            return true;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            return true;
        }
    }

    private class RunningInertiaState : CustomFSMStateBase
    {
        private bool _isAniEnd = false;
        public RunningInertiaState(PlayerFSMSystem system) : base(system)
        {
            
        }

        public override void StartState()
        {
            SystemMgr.AnimationCtrl.PlayAni(AniState.RunningInertia);
            SystemMgr.OnAnimationEndEvent += OnAnimationEndEventFunc;
            _isAniEnd = false;
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
        }

        public override void EndState()
        {
            SystemMgr.OnAnimationEndEvent -= OnAnimationEndEventFunc;
            _isAniEnd = false;
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Wallslideing)
                return false;
            if (condition == TransitionCondition.None)
            {
                return false;
            }
            
            return true;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            return true;
        }

        private void OnAnimationEndEventFunc()
        {
            _isAniEnd = true;
            SystemMgr.Transition(TransitionCondition.Idle);
        }
    }
    
    private class JumpState : CustomFSMStateBase
    {
        private float _inputDir = 1;

        public JumpState(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            SystemMgr.AnimationCtrl.PlayAni(AniState.Jump);
            SystemMgr._fxCtrl.PlayParticle(FxAniEnum.JumpFx);

            SystemMgr.Unit.Jump();
            SystemMgr.StartJumpKeyPressDetectCoroutine();
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();

            if (SystemMgr.Unit.CheckWallslideing())
                SystemMgr.Transition(TransitionCondition.WallClimbing);
            else if (SystemMgr.Unit.GetVelocity().y <= -0.1f)
                SystemMgr.Transition(TransitionCondition.Falling);
        }

        public override void EndState()
        {

        }
        
        public override bool Transition(TransitionCondition condition)
        {
            if (SystemMgr.Unit.IsGround == true)
            {
                if (condition == TransitionCondition.Wallslideing)
                    return false;
                
                return true;
            }
            else
            {
                if (condition == TransitionCondition.JumpAttack)
                    return true;

                if (condition == TransitionCondition.Falling)
                    return true;

                if (condition == TransitionCondition.WallClimbing)
                    return true;
                
                if (condition == TransitionCondition.Dash)
                    return true;
            }
            
            return false;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Jump)
                SystemMgr.Unit.AddJumpForce();

            if (condition == TransitionCondition.LeftMove)
            {
                SystemMgr.Unit.CheckMovementDir(_inputDir * -1);
                SystemMgr.Unit.Move();
            }

            if (condition == TransitionCondition.RightMove)
            {
                SystemMgr.Unit.CheckMovementDir(_inputDir);
                SystemMgr.Unit.Move();
            }
            
            if (condition == TransitionCondition.None)
                SystemMgr.Unit.JumpMoveStop();
            
            if (condition == TransitionCondition.Attack)
                SystemMgr.ChangeState(TransitionCondition.JumpAttack);

            return true;
        }
    }
    
    private class DoubleJump : CustomFSMStateBase
    {
        private float _inputDir = 1;
        
        public DoubleJump(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            SystemMgr.AnimationCtrl.PlayAni(AniState.Jump);
            SystemMgr.Unit.DoubleJump();
            SystemMgr.StartJumpKeyPressDetectCoroutine();

        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
            
            if (SystemMgr.Unit.CheckWallslideing())
                SystemMgr.Transition(TransitionCondition.WallClimbing);
            else if(SystemMgr.Unit.GetVelocity().y <= -0.1f)
                SystemMgr.Transition(TransitionCondition.Falling);
        }

        public override void EndState()
        {
            
        }

        public override bool Transition(TransitionCondition condition)
        {
            // 기능 삭제
            return false;
            
            if (SystemMgr.Unit.IsGround == true)
                return true;
            
            if (condition == TransitionCondition.Wallslideing)
                return true;
            
            if (condition == TransitionCondition.Idle)
            {
                SystemMgr.Unit.JumpMoveStop();
            }

            if (condition == TransitionCondition.Jump)
            {
                SystemMgr.Unit.AddJumpForce();
            }
            if (condition == TransitionCondition.LeftMove)
            {
                SystemMgr.Unit.CheckMovementDir(_inputDir * - 1);
                SystemMgr.Unit.Move(0);
            }
            if (condition == TransitionCondition.RightMove)
            {
                SystemMgr.Unit.CheckMovementDir(_inputDir);
                SystemMgr.Unit.Move(0);
            }
            
            if (condition == TransitionCondition.Attack)
            {
                SystemMgr.Transition(TransitionCondition.JumpAttack);
            }
            return false;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            return false;
        }
    }
    
    private class FallingState : CustomFSMStateBase
    {
        private bool _isFaill = true;
        private float _inputDir = 1;

        public FallingState(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            SystemMgr.AnimationCtrl.PlayAni(AniState.Fall);
            SystemMgr._beforeFalling = true;
            _isFaill = true;
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();

            if (SystemMgr.Unit.CheckWallslideing())
            {
                SystemMgr.Transition(TransitionCondition.WallClimbing);
            }

            if (SystemMgr.Unit.IsGround == true)
            {
                _isFaill = false;
            }
        }

        public override void EndState()
        {
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (_isFaill)
            {
                if (condition == TransitionCondition.WallClimbing)
                {
                    return true;
                }

                if (condition == TransitionCondition.Dash)
                    return true;

                if (condition == TransitionCondition.JumpAttack)
                    return true;

                if (condition == TransitionCondition.None)
                {
                    return false;
                }

                return false;
            }
            else
            {
                if (condition == TransitionCondition.Wallslideing)
                    return false;
            
                return true;
            }
        }

        public override bool InputKey(TransitionCondition condition)
        {
            if (_isFaill)
            {
                if (condition == TransitionCondition.Jump)
                {
                    if (SystemMgr.isJumpKeyPress == false)
                    {
                        if (SystemMgr.Unit.CheckIsJumpAble() == true)
                        {
                            SystemMgr.Transition(TransitionCondition.Jump);
                            return false;
                        }
                    }
                }
                
                if (condition == TransitionCondition.LeftMove)
                {
                    SystemMgr.Unit.CheckMovementDir(_inputDir * - 1);
                    SystemMgr.Unit.Move();
                }
                if (condition == TransitionCondition.RightMove)
                {
                    SystemMgr.Unit.CheckMovementDir(_inputDir);
                    SystemMgr.Unit.Move();
                }
                if (condition == TransitionCondition.Attack)
                    SystemMgr.Transition(TransitionCondition.JumpAttack);

                //return false;
            }
            else
            {
                if (condition == TransitionCondition.Jump)
                {
                    if (SystemMgr.isJumpKeyPress == true)
                    {
                        SystemMgr.Transition(TransitionCondition.Idle);
                        return false;
                    }
                }
                
                if (condition == TransitionCondition.None)
                    SystemMgr.Transition(TransitionCondition.Idle);
            }

            return true;
        }
    }
    
    private class RollState : CustomFSMStateBase
    {
        private float _rollTime = 1.5f;
        private float _timer = 0.0f;
        private bool _isRollEnd = true;
        private bool _isRollAble = true;
        private float _rollCoolTime = 0;
        
        public RollState(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            if (SystemMgr.Unit.IsRollAble)
            {
                //SystemMgr.AnimationCtrl.PlayAni(AniState.Dash);
                SystemMgr.Unit.SetRoll();
                SystemMgr.StartCoroutine(RollCoroutine());
                //SystemMgr.StartCoroutine(RollCoolTimeCoroutine());
            }
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
        }

        public override void EndState()
        {
            SystemMgr.Unit.EndRoll();
            //SystemMgr._isRollAniEnd = false;
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (SystemMgr.Unit.IsRollAble == false)
                return true;
            
            if (_isRollEnd == true)
            {
                return false;
            }
            
            return true;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            return false;
        }

        IEnumerator RollCoroutine()
        {
            _isRollEnd = true;
            _rollTime = SystemMgr.Unit.RollTime;
            _timer = 0.0f;
            while (_timer + GameManager.instance.timeMng.FixedDeltaTime < _rollTime)
            {
                _timer += GameManager.instance.timeMng.FixedDeltaTime;
                SystemMgr.Unit.Roll();
                yield return new WaitForFixedUpdate();
            }
            SystemMgr.Unit.RollStop();
            _isRollEnd = false;
        }
    }
    
    private class DashState : CustomFSMStateBase
    {
        private float _dashTime = 0.5f;
        private float _timer = 0.0f;
        private bool _isDashEnd = true;
        public DashState(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            if (SystemMgr.Unit.isDashAble)
            {
                SystemMgr.AnimationCtrl.PlayAni(AniState.Dash);
                SystemMgr._fxCtrl.PlayParticle(FxAniEnum.DashFx, SystemMgr.Unit.FacingDir);
                
                SystemMgr.Unit.SetDash();
                SystemMgr.StartCoroutine(DashCoroutine());
            }
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
        }

        public override void EndState()
        {
            SystemMgr.Unit.EndDash();
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (SystemMgr.Unit.isDashAble == false)
                return true;
            
            if (_isDashEnd == false)
            {
                return false;
            }
            
            return true;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            return true;
        }

        IEnumerator DashCoroutine()
        {
            _isDashEnd = false;
            _dashTime = SystemMgr.Unit.DashTime;
            _timer = 0.0f;
            while (_timer + GameManager.instance.timeMng.FixedDeltaTime < _dashTime)
            {
                _timer += GameManager.instance.timeMng.FixedDeltaTime;
                SystemMgr.Unit.Dash();
                yield return new WaitForFixedUpdate();
            }
            SystemMgr.Unit.DashStop();
            _isDashEnd = true;

            SystemMgr.Transition(TransitionCondition.Idle);
        }
        
    }

    private class WallslideingState : CustomFSMStateBase
    {
        private bool _isFristWallJump = false;
        
        public WallslideingState(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            SystemMgr.AnimationCtrl.PlayAni(AniState.Wallslideing);

            _isFristWallJump = true;
            SystemMgr.Unit.WallSlideStateStart();

            
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();

            if (SystemMgr.Unit.IsGround)
                SystemMgr.Transition(TransitionCondition.Idle);
        }

        public override void EndState()
        {
            SystemMgr.Unit.WallReset();
            SystemMgr.Unit.WallEnd();
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (SystemMgr.Unit.IsGround)
                return true;

            if (condition == TransitionCondition.WallJump)
                return true;

            if (SystemMgr.Unit.CheckWallslideing() == true)
            {
                if (condition == TransitionCondition.Wallslideing)
                {
                    SystemMgr.Unit.WallSlideing();
                }

                if (SystemMgr.isJumpKeyPress == false)
                {
                    if (condition == TransitionCondition.Jump)
                    {
                        SystemMgr.Transition(TransitionCondition.WallJump);
                    }
                }
                
                if (condition == TransitionCondition.None)
                    SystemMgr.Unit.WallReset();

            }

            return false;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            return false;
        }
    }
    
    private class WallJumpState : CustomFSMStateBase
    {
        private float _inputDir = 0;
        public WallJumpState(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            SystemMgr.AnimationCtrl.PlayAni(AniState.Jump);
            SystemMgr.Unit.WallJump();
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
            
            if (SystemMgr.Unit.CheckWallslideing())
                SystemMgr.Transition(TransitionCondition.WallClimbing);
            else if (SystemMgr.Unit.GetVelocity().y <= 0)
                SystemMgr.Transition(TransitionCondition.Falling);
        }

        public override void EndState()
        {
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (SystemMgr.Unit.IsGround)
                return true;

            if (condition == TransitionCondition.WallClimbing)
                return true;

            if (condition == TransitionCondition.Falling)
                return true;
            
            return false;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            return true;
        }
    }
    
    private class BasicAttackState : CustomFSMStateBase
    {
        private int _basicAttackIndex = 0;
        private int _nextBasicAttackIndex = 0;
        private float _attackInputTime = 0.0f;
        private float _attackBeInputTime = 0.0f;
        private float _attackTime = 0.5f;
        private bool _isBasicAttackEnd = false;
        private float _timer = 0.0f;
        private float _BasicAttackMoveTime = 0.0f;
        private bool _isNotEndCoroutine = false;
        private bool _isBasicAttackAniEnd = false;
        private float _basicAttackDelay = 0.0f;
        private bool _isBasicAttackAble = true;

        private AniState[] _basicAttackAniArr =
            new AniState[] {AniState.Attack, AniState.Attack2, AniState.Attack3};

        private FxAniEnum[] _basicAttackFxAniArr = new FxAniEnum[]
            {FxAniEnum.BasicAttack, FxAniEnum.BasicAttack2, FxAniEnum.BasicAttack3};
        
        
        public BasicAttackState(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
                SystemMgr.OnBasicAttackEndAniEvent += EndOrNextCheck;
            SystemMgr.OnBasicAttackCallEvent += BasicAttackCall;

            SystemMgr.AnimationCtrl.PlayAni(_basicAttackAniArr[_basicAttackIndex]);
            SystemMgr._fxCtrl.PlayAni(_basicAttackFxAniArr[_basicAttackIndex]);

            _attackBeInputTime = Time.time;
            SystemMgr.Unit.SetBasicAttack();
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();

            if(SystemMgr.Unit.GetVelocity().y <= -0.1f)
                SystemMgr.Transition(TransitionCondition.Falling);
            
        }

        public override void EndState()
        {
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.Idle);

            SystemMgr.Unit.EndBasicAttack();
            SystemMgr.Unit.BasicAttackMoveStop();
            
            _basicAttackIndex = 0;
            _nextBasicAttackIndex = 0;
            _attackInputTime = 0;
            _attackBeInputTime = 0;
            _timer = 0.0f;
            _BasicAttackMoveTime = 0.0f;
            _isBasicAttackEnd = false;
            _isNotEndCoroutine = false;
            _isBasicAttackAniEnd = false;
            
            SystemMgr.OnBasicAttackEndAniEvent -= EndOrNextCheck;
            SystemMgr.OnBasicAttackCallEvent -= BasicAttackCall;
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (_isBasicAttackEnd == true || _isBasicAttackAble == false)
            {
                if (condition == TransitionCondition.None)
                    return false;
                
                return true;
            }
            
            if (SystemMgr.AnimationCtrl.GetCurAniTime() >= 0.6f)
            {
                if (condition == TransitionCondition.Dash)
                    return true;

                if (condition == TransitionCondition.Jump)
                    return true;
            }
            
            return false;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Attack)
            {
                _attackInputTime = Time.time;

                if (_attackInputTime - _attackBeInputTime <= _attackTime)
                {
                    _nextBasicAttackIndex = _basicAttackIndex + 1;
                }

                _attackBeInputTime = Time.time;

                return false;
            }

            if (_isBasicAttackAniEnd)
            {
                if (condition == TransitionCondition.LeftMove)
                {
                    SystemMgr.Unit.CheckMovementDir(-1);
                    
                    if (_isNotEndCoroutine == false)
                        SystemMgr.Unit.StartCoroutine(BasicAttackMoveCoroutine());
                }

                if (condition == TransitionCondition.RightMove)
                {
                    SystemMgr.Unit.CheckMovementDir(1);
                    
                    if (_isNotEndCoroutine == false)
                        SystemMgr.Unit.StartCoroutine(BasicAttackMoveCoroutine());
                }

                _isBasicAttackAniEnd = false;
            }
            
            return true;
        }

        private void EndOrNextCheck()
        {
            if (_basicAttackIndex != _nextBasicAttackIndex)
            {
                if (_nextBasicAttackIndex > _basicAttackAniArr.Length - 1)
                {
                    _basicAttackIndex = 0;
                    _nextBasicAttackIndex = 0;
                }
                else
                {
                    _basicAttackIndex = _nextBasicAttackIndex;
                }
                
                
                SystemMgr.AnimationCtrl.PlayAni(_basicAttackAniArr[_basicAttackIndex]);
                SystemMgr._fxCtrl.PlayAni(_basicAttackFxAniArr[_basicAttackIndex]);
            }
            else
            {
                _isBasicAttackEnd = true;
                 SystemMgr.Transition(TransitionCondition.Idle);
            }
            _isBasicAttackAniEnd = true;
        }
        
        private void BasicAttackCall()
        {
            SystemMgr.Unit.BasicAttack(_basicAttackIndex);
        }

        IEnumerator BasicAttackMoveCoroutine()
        {
            _isNotEndCoroutine = true;
            _BasicAttackMoveTime = SystemMgr.Unit.BasicAttackMoveTimeArr[_basicAttackIndex];
            _timer = 0.02f;
            while (_timer < _BasicAttackMoveTime)
            {
                _timer += GameManager.instance.timeMng.FixedDeltaTime;
                SystemMgr.Unit.BasicAttackMove(_basicAttackIndex);
                yield return new WaitForFixedUpdate();
            }
            
            SystemMgr.Unit.EndBasicAttack();
            _isNotEndCoroutine = false;
        }

        IEnumerator BasicAttackDelayCoroutine()
        {
            float _timer = 0.0f;

            _isBasicAttackAble = false;
            _basicAttackDelay = SystemMgr.Unit.BasicAttackDelay;
            
            while (_timer < _basicAttackDelay)
            {
                _timer += GameManager.instance.timeMng.FixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _isBasicAttackAble = true;
        }
        
    }
    
    private class BasicJumpAttack : CustomFSMStateBase
    {
        private bool _isBasicjumpAttackAniEnd = false;
        
        public BasicJumpAttack(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            SystemMgr.AnimationCtrl.PlayAni(AniState.JumpAttack);
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.BasicJumpAttack);

            SystemMgr.OnBasicAttackEndAniEvent += BasicJumpAttackAniEnd;
            SystemMgr.OnBasicAttackCallEvent += BasicJumpAttackCall;
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();

            if (_isBasicjumpAttackAniEnd == true)
            {
                if (SystemMgr.Unit.IsGround == true)
                {
                    SystemMgr.Transition(TransitionCondition.Idle);
                }
                else if (SystemMgr.Unit.GetVelocity().y <= 0)
                    SystemMgr.Transition(TransitionCondition.Falling);
            }
        }

        public override void EndState()
        {
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.Idle);
            
            _isBasicjumpAttackAniEnd = false;
            
            SystemMgr.OnBasicAttackEndAniEvent -= BasicJumpAttackAniEnd;
            SystemMgr.OnBasicAttackCallEvent -= BasicJumpAttackCall;
        }

        public override bool Transition(TransitionCondition condition)
        {
            
            // if (condition == TransitionCondition.Jump)
            // {
            //     if (SystemMgr.isJumpKeyPress == false)
            //     {
            //         if (SystemMgr.Unit.CheckIsJumpAble() == true)
            //         {
            //             if (SystemMgr.AnimationCtrl.GetCurAniTime() >= 0.6)
            //             {
            //                 SystemMgr.Transition(TransitionCondition.DoubleJump);
            //                 return false;
            //             }
            //         }
            //     }
            // }

            // if (_isBasicjumpAttackAniEnd == true)
            // {
            //     if (SystemMgr.Unit.IsGround == false)
            //     {
            //         if (condition == TransitionCondition.Falling)
            //             return true;
            //     }
            //     else
            //     {
            //         if (condition == TransitionCondition.Idle)
            //             return true;
            //     }
            //
            //     if (condition == TransitionCondition.Attack)
            //         SystemMgr.Transition(TransitionCondition.Idle);
            // }

            if (_isBasicjumpAttackAniEnd == false)
                return false;
            else
            {
                if (SystemMgr.Unit.IsGround == false)
                {
                    if (condition == TransitionCondition.Falling)
                        return true;
                }
                else
                {
                    if (condition == TransitionCondition.Idle)
                        return true;
                }
                
            }

            return false;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            if (condition == TransitionCondition.LeftMove)
            {
                SystemMgr.Unit.BasicJumpMove(-1);
            }
            if (condition == TransitionCondition.RightMove)
            {
                SystemMgr.Unit.BasicJumpMove(1);
            }

            return true;
        }

        private void BasicJumpAttackAniEnd()
        {
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.Idle);
            _isBasicjumpAttackAniEnd = true;
            
            SystemMgr.Transition(TransitionCondition.Idle);
        }

        private void BasicJumpAttackCall()
        {
            SystemMgr.Unit.BasicJumpAttack();
        }
    }
    
    private class HitState : CustomFSMStateBase
    {
        private bool _isHitEnd = false;
        public HitState(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            SystemMgr.AnimationCtrl.PlayAni(AniState.Hit);
            
            SystemMgr.Unit.Hit();
            SystemMgr.Unit.HitKnockBack();
            SystemMgr.StartCoroutine(HitTimeCalcCorotine());
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
        }

        public override void EndState()
        {
            SystemMgr.Unit.ResetHitDamage();
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (_isHitEnd == true)
                return false;

            if (condition == TransitionCondition.None)
                return false;
            return true;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            return true;
        }

        IEnumerator HitTimeCalcCorotine()
        {
            _isHitEnd = true;
            float _timer = 0.00f;
            float _hitTime = SystemMgr.Unit.HitTime;
            
            while (_timer + GameManager.instance.timeMng.FixedDeltaTime < _hitTime)
            {
                _timer += GameManager.instance.timeMng.FixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _isHitEnd = false;
            SystemMgr.Transition(TransitionCondition.Idle);
        }
    }

    private class WallClimbingState : CustomFSMStateBase
    {
        public WallClimbingState(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            SystemMgr.AnimationCtrl.PlayAni(AniState.Wallslideing);
            
            SystemMgr.Unit.WallSlideStateStart();
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();

            if (SystemMgr.Unit.IsGround)
                SystemMgr.Transition(TransitionCondition.Idle);
        }

        public override void EndState()
        {
            SystemMgr.Unit.WallReset();
            SystemMgr.Unit.WallEnd();
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (SystemMgr.Unit.IsGround)
            {
                if (condition == TransitionCondition.None)
                    return false;
                
                return true;
            }

            if (condition == TransitionCondition.WallJump)
                return true;
            
            return false;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            if (SystemMgr.Unit.CheckWallslideing() == true)
            {
                if (condition == TransitionCondition.None)
                {
                    SystemMgr.Unit.WallReset();
                }
                
                if (condition == TransitionCondition.Wallslideing)
                {
                    SystemMgr.Unit.WallSlideing();
                }

                if (SystemMgr.isJumpKeyPress == false)
                {
                    if (condition == TransitionCondition.Jump)
                    {
                        SystemMgr.Transition(TransitionCondition.WallJump);
                    }
                }
            }

            return true;
        }
    }

    /// </기획 변경으로 인해 미사용>
    private class SkillShadowWalkState : CustomFSMStateBase, ISkillStateBase
    {
        private Shadow _inAreaShadow = null;
        private GameSkillObject _gameSkillObject;
        private ShadowWalkSkillData _shadowWalkSkillData;
        
        public SkillShadowWalkState(PlayerFSMSystem system, ShadowWalkSkillData shadowWalkSkillData) : base(system)
        {
            _shadowWalkSkillData = shadowWalkSkillData;
        }

        public override void StartState()
        {
            SystemMgr.OnAnimationEndEvent += OnAnimationEndEvnetCall;
            SystemMgr.AnimationCtrl.PlayAni(AniState.SkillShadowWalk);
            _gameSkillObject = InvokeShadowWalkSkill();
        }
 

        public override void Update()
        {
            SystemMgr.Unit.Progress();
        }

        public override void EndState()
        {
            SystemMgr.OnAnimationEndEvent -= OnAnimationEndEvnetCall;
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Idle)
                return true;
            
            return false;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            return false;
        }

        private void OnAnimationEndEvnetCall()
        {
            SystemMgr.Transition(TransitionCondition.Idle);
        }

        private GameSkillObject InvokeShadowWalkSkill()
        {
            var skillObject = GameManager.instance.GameSkillMgr.GetSkillObject();

            if (skillObject == null)
            {
                Debug.LogError("Skill Obejct Is Null");
                return null;
            }

            skillObject.InitSkill(_shadowWalkSkillData.GetSkillController(skillObject, SystemMgr.Unit));
            return skillObject;
        }

        public bool IsAbleTransition()
        {
            //_inAreaShadow = SystemMgr.Unit.GetAbleShadowWalk();

            if (_inAreaShadow == null)
                return false;

            return true;
        }
    }
    /// </기획 변경으로 인해 미사용>
    protected override void RegisterState()
    {
        AddState(TransitionCondition.Idle, new IdleState(this));
        AddState(TransitionCondition.Move, new MoveState(this));
        AddState(TransitionCondition.StopMove, new StopMoveState(this));
        AddState(TransitionCondition.RunningInertia, new RunningInertiaState(this));
        AddState(TransitionCondition.Jump, new JumpState(this));
        AddState(TransitionCondition.DoubleJump, new DoubleJump(this));
        AddState(TransitionCondition.Falling, new FallingState(this));
        //AddState(TransitionCondition.Roll, new RollState(this));
        AddState(TransitionCondition.WallClimbing, new WallClimbingState(this));
        AddState(TransitionCondition.Wallslideing, new WallslideingState(this));
        AddState(TransitionCondition.WallJump, new WallJumpState(this));
        AddState(TransitionCondition.Dash, new DashState(this));
        AddState(TransitionCondition.Attack, new BasicAttackState(this));
        AddState(TransitionCondition.JumpAttack, new BasicJumpAttack(this));
        AddState(TransitionCondition.Hit, new HitState(this));
        //AddState(TransitionCondition.SkillShadowWalk, new SkillShadowWalkState(this, Unit.ShadowWalkSkillData));
    }
    
    public bool Transition(TransitionCondition condition, object param = null)
    {
        if (GetState(CurrState).InputKey(condition) == false)
        {
            return false;
        }
        
        if (CurrState == condition)
        {
            return false;
        }

        if (CheckStateChangeAbleCondition(condition) == false)
            return false;

        if (condition == TransitionCondition.Jump)
        {
            if (isJumpKeyPress == true) // 점프키가 계속 눌려있는 경우를 체크
                return false;
        }
        
        // Left Move, Right Move에 대한 스테이트를 새로 만들기 싫어서 예외 처리 진행 함.
        if (condition == TransitionCondition.LeftMove || condition == TransitionCondition.RightMove)
        {
            ChangeState(TransitionCondition.Move);
        }

        // if (condition == TransitionCondition.None)
        // {
        //     if (GetState(CurrState).InputKey(condition) == false)
        //         return false;
        // }
        // else if (GetState(condition).InputKey(condition) == false)
        // {
        //     return false;
        // }

        if (GameManager.instance.timeMng.IsHitStop == true)
            return false;

        ChangeState(condition);
        return true;
    }
 
    
    // 애니메이션 이벤트 관련 호출 함수
    public void AniEndEvent()
    {
        OnAnimationEndEvent?.Invoke();
    }

    public void BasicAttackAniEndEvent()
    {
        OnBasicAttackEndAniEvent?.Invoke();
    }

    public void BasicAttackCallEvent()
    {
        OnBasicAttackCallEvent?.Invoke();
    }

    public void UnitHitEventCall()
    {
        Transition(TransitionCondition.Hit);
    }

    public void StartJumpKeyPressDetectCoroutine()
    {
        StartCoroutine(JumpKeyPressDetectCoroutine());
    }

    private void OnIsBattleIdleEventCall(bool isBattle)
    {
        _isBattleIdle = isBattle;
    }

    private void OnCommandCastEventCall(string skillName)
    {
        var condition = ChangeSkillNameToTransitionCondition(skillName);

        if (condition == TransitionCondition.None)
            return;

        if (CheckSkillPossibleConditions(condition) == true)
            Transition(condition);
        
    }

    private TransitionCondition ChangeSkillNameToTransitionCondition(string skillName)
    {
        switch (skillName)
        {
            case "ShadowWalk":
                return TransitionCondition.SkillShadowWalk;
                break;
            
            default:
                break;
        }

        return TransitionCondition.None;
    }

    private bool CheckSkillPossibleConditions(TransitionCondition condition)
    {
        // 공용 체크
        if (CurrState == TransitionCondition.Hit)
            return false;

        // 가져 올 State가 Skill이라는 확정사항이므로 괜찮은걸까?
        var state = GetState(condition) as ISkillStateBase;

        // 특수 체크
        return state.IsAbleTransition();
    }

    // Time 관련

    private void StartBulletTimeEvnetCall(float _timeScale)
    {
        Unit.StartBulletTime(_timeScale);
        AnimationCtrl.SetAnimationTimeSclae(_timeScale);
        _fxCtrl.SetAnimationTimeSclae(_timeScale);
    }

    private void EndBulletTimeEventCall(float _timeScale)
    {
        Unit.EndBulletTime(_timeScale);
        AnimationCtrl.SetAnimationTimeSclae(_timeScale);
        _fxCtrl.SetAnimationTimeSclae(_timeScale);

    }

    private void StartHitStopEventCall(float _timeScale)
    {
        Unit.StartHitStop(_timeScale);
        AnimationCtrl.SetAnimationTimeSclae(_timeScale);
        _fxCtrl.SetAnimationTimeSclae(_timeScale);

    }

    private void EndHitStopEvnetCall(float _timeScale)
    {
        Unit.EndHitStop(_timeScale);
        AnimationCtrl.SetAnimationTimeSclae(_timeScale);
        _fxCtrl.SetAnimationTimeSclae(_timeScale);

    }
    
    // Time 관련

    IEnumerator JumpKeyPressDetectCoroutine()
    {
        isJumpKeyPress = true;
        
        while (isJumpKeyPress)
        {
            if (Input.GetKeyUp(KeyCode.C))
                isJumpKeyPress = false;
                
            yield return null;
        }
    }

    private void SetUnit(PlayerUnit unit)
    {
        if(unit == null)
        {
            Debug.LogError("Unit is Null");
            return;
        }

        _unit = unit;
    }

    public Unit GetUnit()
    {
        return _unit;
    }
}
