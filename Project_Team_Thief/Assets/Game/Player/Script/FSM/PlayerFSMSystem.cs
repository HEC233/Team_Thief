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

    private TransitionCondition _hitstopCondition;

    //[SerializeField]
    //private BattleIdleCtrl _battleIdleCtrl;

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

    private void OnDestroy()
    {
        ChangeState(TransitionCondition.Idle);
        UnBind();
    }

    private void Init()
    {
        Bind();
        //GameManager.instance.SetControlActor(this);

        //==================== 고재협이 편집함 ==================
        GameManager.instance.SetPlayerActor(this);
        GameManager.instance.ChangeActorToPlayer();

        GameManager.instance.shadow.RegistCollider(GetComponent<BoxCollider2D>());
        //=======================================================
    }

    private void Bind()
    {
        GameManager.instance.timeMng.startBulletTimeEvent += StartBulletTimeEvnetCall;
        GameManager.instance.timeMng.endBulletTimeEvent += EndBulletTimeEventCall;
        GameManager.instance.timeMng.startHitstopEvent += StartHitStopEventCall;
        GameManager.instance.timeMng.endHitstopEvent += EndHitStopEvnetCall;
        Unit.hitEvent += UnitHitEventCall;
        Unit.OnPlayerDeadEvent += UnitDeadEventCall;
        
        GameManager.instance.commandManager.OnCommandCastEvent += OnCommandCastEventCall;
    }

    private void UnBind()
    {
        GameManager.instance.timeMng.startBulletTimeEvent -= StartBulletTimeEvnetCall;
        GameManager.instance.timeMng.endBulletTimeEvent -= EndBulletTimeEventCall;
        GameManager.instance.timeMng.startHitstopEvent -= StartHitStopEventCall;
        GameManager.instance.timeMng.endHitstopEvent -= EndHitStopEvnetCall;
        Unit.hitEvent -= UnitHitEventCall;
        Unit.OnPlayerDeadEvent -= UnitDeadEventCall;
        
        GameManager.instance.commandManager.OnCommandCastEvent -= OnCommandCastEventCall;
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
            {
                //=============================
                GameManager.instance.PushEventQueue();
                return false;
                //=============================
            }
            if (condition == TransitionCondition.None)
                return false;
            
            if (condition == TransitionCondition.Jump)
            {
                if (SystemMgr.Unit.CheckIsJumpAble() == false)
                {
                    return false;
                }
            }

            if (condition == TransitionCondition.Dash)
            {
                if (SystemMgr.Unit.isDashAble == false)
                {
                    return false;
                }
            }
            
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
        private uint _soundId = 0;

        public MoveState(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            SystemMgr.AnimationCtrl.PlayAni(AniState.Move);
            _soundId = WwiseSoundManager.instance.PlayEventSound("PC_FS");
            
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
            WwiseSoundManager.instance.StopEventSoundFromId(_soundId);
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
            
            if (condition == TransitionCondition.Jump)
            {
                if (SystemMgr.Unit.CheckIsJumpAble() == false)
                {
                    return false;
                }
            }
            
            if (condition == TransitionCondition.Dash)
            {
                if (SystemMgr.Unit.isDashAble == false)
                {
                    return false;
                }
            }
            
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

            if (condition == TransitionCondition.RunningInertia)
                return true;

            if (condition == TransitionCondition.Idle)
                return true;
            
            return false;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            return true;
        }
    }

    private class RunningInertiaState : CustomFSMStateBase
    {
        private bool _isAniEnd = false;
        private uint _movestopSoundId = 0;
        
        public RunningInertiaState(PlayerFSMSystem system) : base(system)
        {
            
        }

        public override void StartState()
        {
            SystemMgr.AnimationCtrl.PlayAni(AniState.RunningInertia);
            SystemMgr.OnAnimationEndEvent += OnAnimationEndEventFunc;
            _movestopSoundId = WwiseSoundManager.instance.PlayEventSound("PC_stop");
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
            
            WwiseSoundManager.instance.StopEventSoundFromId(_movestopSoundId);
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Wallslideing)
                return false;
            if (condition == TransitionCondition.None)
            {
                return false;
            }
            if (condition == TransitionCondition.Jump)
            {
                if (SystemMgr.Unit.CheckIsJumpAble() == false)
                {
                    return false;
                }
            }
            
            if (condition == TransitionCondition.Dash)
            {
                if (SystemMgr.Unit.isDashAble == false)
                {
                    return false;
                }
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
            WwiseSoundManager.instance.PlayEventSound("PC_jump");
            
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
            if (condition == TransitionCondition.Hit)
                return true;

            if (condition == TransitionCondition.SkillAxe)
                return true;
            if (condition == TransitionCondition.SkillSpear)
                return true;
            if (condition == TransitionCondition.SkillKopsh)
                return true;
            
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
                SystemMgr.Transition(TransitionCondition.Idle);
            }
        }

        public override void EndState()
        {
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Hit)
                return true;
            
            if (condition == TransitionCondition.SkillAxe)
                return true;
            if (condition == TransitionCondition.SkillSpear)
                return true;
            if (condition == TransitionCondition.SkillKopsh)
                return true;
            
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

                if (condition == TransitionCondition.Jump)
                {
                    if (SystemMgr.Unit.CheckIsJumpAble() == true)
                    {
                        return true;
                    }
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
                        return true;
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

                if (condition == TransitionCondition.Wallslideing)
                {
                    SystemMgr.Transition(TransitionCondition.Idle);
                    return false;
                }

                if (condition == TransitionCondition.None)
                {
                    SystemMgr.Transition(TransitionCondition.Idle);

                }
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
                WwiseSoundManager.instance.PlayEventSound("PC_dash");
                
                SystemMgr.Unit.SetDash();
                SystemMgr.StartCoroutine(DashCoroutine());
            }
            else
            {
                // 대쉬가 불가능한 경우 대쉬 상태에서 멈추는 문제 있음.
                // 리팩토링 할 때 넘기는 것 보단 대쉬가 현재 가능한 상태인지 묻는게 먼저일듯.
                SystemMgr.Transition(TransitionCondition.Idle);
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
            if (_isDashEnd == false)
            {
                return false;
            }

            if (condition == TransitionCondition.WallClimbing)
                return false;
            
            // if (SystemMgr.Unit.isDashAble == false)
            //     return true;
            //

         
            if (condition == TransitionCondition.Jump)
            {
                if (SystemMgr.Unit.CheckIsJumpAble() == false)
                {
                    return false;
                }
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
            new AniState[] {AniState.Attack, AniState.Attack2, AniState.Attack3, AniState.Attack4};

        private FxAniEnum[] _basicAttackFxAniArr = new FxAniEnum[]
            {FxAniEnum.BasicAttack, FxAniEnum.BasicAttack2, FxAniEnum.BasicAttack3, FxAniEnum.BasicAttack4};

        private string[] _basicAttackSoundArr = new string[] {"PC_BA1", "PC_BA2", "PC_BA3", "PC_BA4"};
        
        public BasicAttackState(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
                SystemMgr.OnBasicAttackEndAniEvent += EndOrNextCheck;
            SystemMgr.OnBasicAttackCallEvent += BasicAttackCall;

            SystemMgr.AnimationCtrl.PlayAni(_basicAttackAniArr[_basicAttackIndex]);
            SystemMgr._fxCtrl.PlayAni(_basicAttackFxAniArr[_basicAttackIndex]);
            WwiseSoundManager.instance.PlayEventSound(_basicAttackSoundArr[0]);
            
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
            SystemMgr.AnimationCtrl.SetSpeed(1);
            SystemMgr.AnimationCtrl.SetSpeed(1);
            
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
            if (condition == TransitionCondition.Hit)
                return true;
            
            
            if (condition == TransitionCondition.SkillAxe)
                return true;
            if (condition == TransitionCondition.SkillSpear)
                return true;
            if (condition == TransitionCondition.SkillHammer)
                return true;
            if (condition == TransitionCondition.SkillKopsh)
                return true;
            if (condition == TransitionCondition.SkillPlainSword)
                return true;

            
            if (_isBasicAttackEnd == true || _isBasicAttackAble == false)
            {
                if (condition == TransitionCondition.None)
                    return false;
                
                return true;
            }
            
            if (SystemMgr.AnimationCtrl.GetCurAniTime() >= SystemMgr.Unit.BasicAttackCansleTime)
            {
                if (condition == TransitionCondition.Dash)
                {
                    if (SystemMgr.Unit.isDashAble == false)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                if (condition == TransitionCondition.Jump)
                {
                    if (SystemMgr.Unit.CheckIsJumpAble() == false)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
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
                    SystemMgr.AnimationCtrl.SetSpeed(SystemMgr.Unit.AniFastAmount);
                    SystemMgr.AnimationCtrl.SetSpeed(SystemMgr.Unit.AniFastAmount);
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
                    // _basicAttackIndex = 0;
                    // _nextBasicAttackIndex = 0;
                    _isBasicAttackEnd = true;
                    SystemMgr.Transition(TransitionCondition.Idle);
                    
                }
                else
                {
                    _basicAttackIndex = _nextBasicAttackIndex;
                    
                    SystemMgr.AnimationCtrl.SetSpeed(1);
                    SystemMgr.AnimationCtrl.SetSpeed(1);

                    SystemMgr.AnimationCtrl.PlayAni(_basicAttackAniArr[_basicAttackIndex]);
                    SystemMgr._fxCtrl.PlayAni(_basicAttackFxAniArr[_basicAttackIndex]);
                    WwiseSoundManager.instance.PlayEventSound(_basicAttackSoundArr[_basicAttackIndex]);
                }
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
            if (_basicAttackIndex == 2)
            {
                SystemMgr.Unit.BasicAttackMoveStop();
            }
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
        private int _jumpAttackIndex = 0;
        private bool _isBasicJumpAttackStart = false;
        private bool _isNotEndCoroutine = false;
        private Coroutine _jumpAttackMoveCoroutine;
        private float _attackInputTime = 0.0f;
        private float _attackBeInputTime = 0.0f;
        
        public BasicJumpAttack(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            if (SystemMgr.Unit.isBasicJumpAttackAble == true)
            {
                _isNotEndCoroutine = true;
                SystemMgr.AnimationCtrl.PlayAni(AniState.JumpAttack);
                SystemMgr._fxCtrl.PlayAni(FxAniEnum.BasicJumpAttack);
                WwiseSoundManager.instance.PlayEventSound("PC_JA1");
                _jumpAttackMoveCoroutine = SystemMgr.Unit.StartCoroutine(BasicJumpAttackMoveCoroutine());

                SystemMgr.OnBasicAttackEndAniEvent += BasicJumpAttackAniEnd;
                SystemMgr.OnBasicAttackCallEvent += BasicJumpAttackCall;
                SystemMgr.Unit.isBasicJumpAttackAble = false;
                _attackBeInputTime = Time.time;
                _isBasicJumpAttackStart = true;
            }
            else
            {
                _isBasicJumpAttackStart = false;
 
                if (SystemMgr.Unit.IsGround == true)
                {
                    SystemMgr.Transition(TransitionCondition.Idle);
                }
                else
                    SystemMgr.Transition(TransitionCondition.Falling);
            }
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
                else
                    SystemMgr.Transition(TransitionCondition.Falling);
            }
        }

        public override void EndState()
        {
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.Idle);
            _isBasicjumpAttackAniEnd = false;
            _isBasicJumpAttackStart = false;
            _jumpAttackIndex = 0;
            
            SystemMgr.OnBasicAttackEndAniEvent -= BasicJumpAttackAniEnd;
            SystemMgr.OnBasicAttackCallEvent -= BasicJumpAttackCall;
            
            //SystemMgr.Unit.EndJumpAttackMove();
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Idle)
                return true;
            if (condition == TransitionCondition.Falling)
                return true;
            if (condition == TransitionCondition.Hit)
                return true;

            if (condition == TransitionCondition.Dash)
                return true;
            
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

                }
                
            }

            return false;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            if (condition == TransitionCondition.LeftMove)
            {
                if (_isNotEndCoroutine == false)
                    SystemMgr.Unit.BasicJumpMove(-1);
            }
            if (condition == TransitionCondition.RightMove)
            {
                if (_isNotEndCoroutine == false)
                    SystemMgr.Unit.BasicJumpMove(1);
            }

            if (condition == TransitionCondition.Attack)
            {
                if (_isBasicjumpAttackAniEnd == true)
                    return true;
                
                _attackInputTime = Time.time;

                if (_attackInputTime - _attackBeInputTime <= SystemMgr.Unit.BasicJumpAttackTime)
                {
                    BasicJumpAttack2();
                }

            }

            return true;
        }
        
        IEnumerator BasicJumpAttackMoveCoroutine()
        {
            SystemMgr.Unit.SetJumpAttackMove();
            
            _isNotEndCoroutine = true;
            float _basicJumpAttackMoveTime = SystemMgr.Unit.BasicJumpAttackMoveTimeArr[_jumpAttackIndex];
            float _timer = 0.02f;
            while (_timer < _basicJumpAttackMoveTime)
            {
                _timer += GameManager.instance.timeMng.FixedDeltaTime;
                SystemMgr.Unit.BasicJumpAttackMove(_jumpAttackIndex);
                yield return new WaitForFixedUpdate();
            }
            
            _isNotEndCoroutine = false;
            SystemMgr.Unit.EndJumpAttackMove();
        }

        private void BasicJumpAttack2()
        {
            if(_jumpAttackIndex >= 1)
                return;

            if (_isNotEndCoroutine == true)
            {
                SystemMgr.Unit.StopCoroutine(_jumpAttackMoveCoroutine);
            }
            
            _jumpAttackIndex++;
            SystemMgr.AnimationCtrl.PlayAni(AniState.JumpAttack2);
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.JumpAttackFx2);
            SystemMgr.Unit.StartCoroutine(BasicJumpAttackMoveCoroutine());
            WwiseSoundManager.instance.PlayEventSound("PC_JA2");

        }
        

        private void BasicJumpAttackAniEnd()
        {
            if (_jumpAttackIndex >= 1 && _isNotEndCoroutine)
            {
                return;
            }

            SystemMgr._fxCtrl.PlayAni(FxAniEnum.Idle);
            _isBasicjumpAttackAniEnd = true;
        }

        private void BasicJumpAttackCall()
        {
            SystemMgr.Unit.BasicJumpAttack(_jumpAttackIndex);
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
            WwiseSoundManager.instance.PlayEventSound("PC_hurt");
            
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

            if (condition == TransitionCondition.WallClimbing)
                return false;
            
            return true;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            // if (SystemMgr.Unit.CheckIsJumpAble() == false)
            // {
            //     return false;
            // }
            
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
        private uint _slidingSoundId = 0;
        
        public WallClimbingState(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            SystemMgr.AnimationCtrl.PlayAni(AniState.Wallslideing);
            WwiseSoundManager.instance.PlayEventSound("PC_Wall");
            _slidingSoundId = WwiseSoundManager.instance.PlayEventSound("PC_slide");

            SystemMgr.Unit.WallSlideStateStart();
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();

            
            if (SystemMgr.Unit.IsGround || SystemMgr.Unit.CheckWallslideing() == false)
                SystemMgr.Transition(TransitionCondition.Idle);
        }

        public override void EndState()
        {
            SystemMgr.Unit.WallReset();
            SystemMgr.Unit.WallEnd();
            WwiseSoundManager.instance.StopEventSoundFromId(_slidingSoundId);
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Hit)
                return true;
            
            if (SystemMgr.Unit.IsGround)
            {
                if (condition == TransitionCondition.None)
                    return false;
                
                return true;
            }

            if (condition == TransitionCondition.WallJump)
                return true;

            if (condition == TransitionCondition.Idle)
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
    
    private class DieState : CustomFSMStateBase
    {
        private bool _isAniEnd = false;
        public DieState(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            SystemMgr.OnAnimationEndEvent += OnAnimationEndEvnetCall;
            SystemMgr.AnimationCtrl.PlayAni(AniState.Die);

            //WwiseSoundManager.instance.PlayEventSound("PC_dead");
        }

        public override void Update()
        {
            
        }

        public override void EndState()
        {
            
        }

        public override bool Transition(TransitionCondition condition)
        {
            return false;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            return false;
        }
        
        private void OnAnimationEndEvnetCall()
        {
            _isAniEnd = true;
            SystemMgr.OnAnimationEndEvent += OnAnimationEndEvnetCall;
            GameManager.instance.isPlayerDead = true;
            GameManager.instance.uiMng.PlayerDead();
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
            //SystemMgr.AnimationCtrl.PlayAni(AniState.SkillShadowWalk);
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

    private class SkillAxeState : CustomFSMStateBase, ISkillStateBase
    {
        private bool _isAniEnd = false;
        private bool _isAxe2Action = false;
        private SkillAxeData _skillAxeData;
        private GameSkillObject _gameSkillObject;
        private GameSkillObject _gameSkillObject2;
        
        public SkillAxeState(PlayerFSMSystem system, SkillAxeData skillAxeData) : base(system)
        {
            _skillAxeData = skillAxeData;
        }

        public override void StartState()
        {
            SystemMgr.OnAnimationEndEvent += OnAnimationEndEvnetCall;
            SystemMgr.AnimationCtrl.PlayAni(AniState.SkillAxe);
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.SkillAxe);
            WwiseSoundManager.instance.PlayEventSound("PC_axe");
            GameManager.instance.uiMng.TurnXButtonUI(true);
            _gameSkillObject = InvokeSkill();
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
        }

        public override void EndState()
        {
            SystemMgr.OnAnimationEndEvent -= OnAnimationEndEvnetCall;
            _isAniEnd = false;
            _isAxe2Action = false;
            GameManager.instance.uiMng.TurnXButtonUI(false);
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.Idle);

        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Hit)
                return true;
            
            if (condition == TransitionCondition.SkillSpear)
                return true;
            if (condition == TransitionCondition.SkillHammer)
                return true;
            if (condition == TransitionCondition.SkillKopsh)
                return true;
            if (condition == TransitionCondition.SkillPlainSword)
                return true;

            if (_isAniEnd == false)
                return false;
            
            if (condition == TransitionCondition.Jump)
            {
                if (SystemMgr.Unit.CheckIsJumpAble() == false)
                {
                    return false;
                }
            }
            
            if (condition == TransitionCondition.Dash)
            {
                if (SystemMgr.Unit.isDashAble == false)
                {
                    return false;
                }
            }
            
            return true;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Attack)
            {
                TwoAxeAction();
                return false;
            }
            return true;
        }

        public bool IsAbleTransition()
        {
            return SystemMgr.Unit.IsAbleSkillAxe();
        }

        private void OnAnimationEndEvnetCall()
        {
            _isAniEnd = true;
            SystemMgr.Transition(TransitionCondition.Idle);
        }

        private void TwoAxeAction()
        {
            if (_isAxe2Action == false)
            {
                SystemMgr.AnimationCtrl.PlayAni(AniState.SkillAxe2);
                SystemMgr._fxCtrl.PlayAni(FxAniEnum.SkillAxe2);
                WwiseSoundManager.instance.PlayEventSound("PC_axe");
                GameManager.instance.uiMng.TurnXButtonUI(false);
                _gameSkillObject2 = InvokeSkill();
            }

            _isAxe2Action = true;
        }

        private void CheckPlayerDir(string skillName)
        {
            
        }

        private GameSkillObject InvokeSkill()
        {
            var skillObejct = GameManager.instance.GameSkillMgr.GetSkillObject();

            if (skillObejct == null)
            {
                Debug.LogError("AexSkillObj is Null");
                return null;
            }

            skillObejct.InitSkill(_skillAxeData.GetSkillController(skillObejct, SystemMgr.Unit));
            return skillObejct;
        }
    }

    class SkillSpearState : CustomFSMStateBase, ISkillStateBase
    {
        private SkillSpearData _skillSpearData;
        private bool _isAniEnd = false;
        private GameSkillObject _gameSkillObject;
        
        public SkillSpearState(PlayerFSMSystem system, SkillSpearData skillSpearData) : base(system)
        {
            _skillSpearData = skillSpearData;
        }

        public override void StartState()
        {
            SystemMgr.OnAnimationEndEvent += OnAnimationEndEvnetCall;
            SystemMgr.AnimationCtrl.PlayAni(AniState.SkillSpear);
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.SkillSpear);

            _gameSkillObject = InvokeSkill();
        }

        public override void Update()
        {
        }

        public override void EndState()
        {
            SystemMgr.OnAnimationEndEvent -= OnAnimationEndEvnetCall;
            _isAniEnd = false;
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.Idle);
            
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Hit)
                return true;
            
            if (condition == TransitionCondition.SkillSpear)
                return true;
            if (condition == TransitionCondition.SkillHammer)
                return true;
            if (condition == TransitionCondition.SkillKopsh)
                return true;
            if (condition == TransitionCondition.SkillPlainSword)
                return true;
            
            if (_isAniEnd == true)
            {
                return true;
            }

            return false;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            return true;
        }

        public bool IsAbleTransition()
        {
            return SystemMgr.Unit.IsAbleSkillSpear();
        }
        
        private void OnAnimationEndEvnetCall()
        {
            _isAniEnd = true;
            SystemMgr.Transition(TransitionCondition.Idle);
        }
        
        private GameSkillObject InvokeSkill()
        {
            var skillObejct = GameManager.instance.GameSkillMgr.GetSkillObject();

            if (skillObejct == null)
            {
                Debug.LogError("AexSkillObj is Null");
                return null;
            }

            skillObejct.InitSkill(_skillSpearData.GetSkillController(skillObejct, SystemMgr.Unit));
            return skillObejct;
        }
    }

    private class SkillHammerState : CustomFSMStateBase, ISkillStateBase
    {
        private SkillHammerData _skillHammerData;
        private bool _isAniEnd = false;
        private GameSkillObject _gameSkillObject;
        
        public SkillHammerState(PlayerFSMSystem system, SkillHammerData skillHammerData) : base(system)
        {
            _skillHammerData = skillHammerData;
        }

        public override void StartState()
        {
            SystemMgr.AnimationCtrl.PlayAni(AniState.SkillHammer);
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.SkillHammer);
            SystemMgr.OnAnimationEndEvent += OnAnimationEndEvnetCall;
            WwiseSoundManager.instance.PlayEventSound("PC_hammaer_Swing");

            _gameSkillObject = InvokeSkill();
        }

        public override void Update()
        {
        }

        public override void EndState()
        {
            SystemMgr.OnAnimationEndEvent -= OnAnimationEndEvnetCall;
            _isAniEnd = false;
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.Idle);

        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Hit)
                return true;
            
            if (condition == TransitionCondition.SkillAxe)
                return true;
            if (condition == TransitionCondition.SkillSpear)
                return true;
            if (condition == TransitionCondition.SkillKopsh)
                return true;
            if (condition == TransitionCondition.SkillPlainSword)
                return true;
            
            if (_isAniEnd == false)
                return false;
            
            return true;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            return true;
        }

        public bool IsAbleTransition()
        {
            return SystemMgr.Unit.IsAbleSkillHammer();
        }
        
        private void OnAnimationEndEvnetCall()
        {
            _isAniEnd = true;
            SystemMgr.Transition(TransitionCondition.Idle);
        }
        
        private GameSkillObject InvokeSkill()
        {
            var skillObejct = GameManager.instance.GameSkillMgr.GetSkillObject();

            if (skillObejct == null)
            {
                Debug.LogError("HammerSkillObj is Null");
                return null;
            }

            skillObejct.InitSkill(_skillHammerData.GetSkillController(skillObejct, SystemMgr.Unit));
            return skillObejct;
        }
    }

    private class SkillKopshState : CustomFSMStateBase, ISkillStateBase
    {
        private SkillKopshData _skillKopshData;
        private bool _isAniEnd = false;
        private GameSkillObject _gameSkillObject;
        
        private float _attackInputTime = 0.0f;
        private float _attackBeInputTime = 0.0f;
        private float _attackTime = 0.5f;
        private int _skillKopshNextIndex = 0;
        
        private AniState[] _skillKopshAniArr =
            new AniState[] {AniState.SkillKopsh, AniState.SkillKopsh2, AniState.SkillKopsh3};

        private FxAniEnum[] _skillKopshFxAniArr = new FxAniEnum[]
            {FxAniEnum.SkillKopsh, FxAniEnum.SkillKopsh2, FxAniEnum.SkillKopsh3};

        private string[] _skillKopshSoundArr = new string[] {"PC_Kopsh1", "PC_Kopsh2", "PC_Kopsh3"};
        
        public SkillKopshState(PlayerFSMSystem system, SkillKopshData skillKopshData) : base(system)
        {
            _skillKopshData = skillKopshData;
        }

        public override void StartState()
        {
            SystemMgr.OnAnimationEndEvent += OnAnimationEndEvnetCall;
            SystemMgr.AnimationCtrl.PlayAni(_skillKopshAniArr[SystemMgr.Unit.skillKopshIndex]);
            SystemMgr._fxCtrl.PlayAni(_skillKopshFxAniArr[SystemMgr.Unit.skillKopshIndex]);
            WwiseSoundManager.instance.PlayEventSound(_skillKopshSoundArr[SystemMgr.Unit.skillKopshIndex]);
            GameManager.instance.uiMng.TurnXButtonUI(true);

            _gameSkillObject = InvokeSkill();
            
            _attackTime = SystemMgr.Unit.SkillKopshAttackTime;
            _attackBeInputTime = Time.time;
        }

        public override void Update()
        {
        }

        public override void EndState()
        {
            SystemMgr.AnimationCtrl.SetSpeed(1);
            SystemMgr._fxCtrl.SetSpeed(1);
            
            SystemMgr.OnAnimationEndEvent -= OnAnimationEndEvnetCall;
            
            _skillKopshNextIndex = 0;
            SystemMgr.Unit.skillKopshIndex = 0;
            _attackBeInputTime = 0;
            _attackInputTime = 0;
            _isAniEnd = false;
            
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.Idle);
            GameManager.instance.uiMng.TurnXButtonUI(false);

        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Hit)
                return true;
            
            if (condition == TransitionCondition.SkillAxe)
                return true;
            if (condition == TransitionCondition.SkillHammer)
                return true;
            if (condition == TransitionCondition.SkillSpear)
                return true;
            if (condition == TransitionCondition.SkillPlainSword)
                return true;
            
            if (_isAniEnd == true)
                return true;
            
            return false;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Attack)
            {
                _attackInputTime = Time.time;

                if (_attackInputTime - _attackBeInputTime <= _attackTime)
                {
                    _skillKopshNextIndex = SystemMgr.Unit.skillKopshIndex + 1;
                    SystemMgr.AnimationCtrl.SetSpeed(SystemMgr.Unit.AniFastAmount);
                    SystemMgr._fxCtrl.SetSpeed(SystemMgr.Unit.AniFastAmount);
                }

                _attackBeInputTime = Time.time;

                return false;
            }
            return true;
        }

        public bool IsAbleTransition()
        {
            return SystemMgr.Unit.IsAbleSkillKopsh();
        }
        
        private void OnAnimationEndEvnetCall()
        {
            if(IsEndOrNextCheck() == true)
            {
                NextKopshAction();
            }
            else
            {
                _isAniEnd = true;
                SystemMgr.Transition(TransitionCondition.Idle);
            }
        }

        private void NextKopshAction()
        {
            SystemMgr.AnimationCtrl.SetSpeed(1);
            SystemMgr._fxCtrl.SetSpeed(1);
            
            SystemMgr.AnimationCtrl.PlayAni(_skillKopshAniArr[SystemMgr.Unit.skillKopshIndex]);
            SystemMgr._fxCtrl.PlayAni(_skillKopshFxAniArr[SystemMgr.Unit.skillKopshIndex]);
            WwiseSoundManager.instance.PlayEventSound(_skillKopshSoundArr[SystemMgr.Unit.skillKopshIndex]);

            _gameSkillObject = InvokeSkill();

            if (SystemMgr.Unit.skillKopshIndex == _skillKopshAniArr.Length - 1)
            {
                GameManager.instance.uiMng.TurnXButtonUI(false);
            }
        }
        
        private bool IsEndOrNextCheck()
        {
            if (SystemMgr.Unit.skillKopshIndex != _skillKopshNextIndex)
            {
                if (_skillKopshNextIndex > _skillKopshAniArr.Length - 1)
                {
                    return false;
                }
                else
                {
                    SystemMgr.Unit.skillKopshIndex = _skillKopshNextIndex;
                    return true;
                }
            }
            
            return false;
        }
        
        private GameSkillObject InvokeSkill()
        {
            var skillObejct = GameManager.instance.GameSkillMgr.GetSkillObject();

            if (skillObejct == null)
            {
                Debug.LogError("AexSkillObj is Null");
                return null;
            }

            skillObejct.InitSkill(_skillKopshData.GetSkillController(skillObejct, SystemMgr.Unit));
            return skillObejct;
        }
    }

    private class SkillPlainSwordState : CustomFSMStateBase, ISkillStateBase
    {
        private SkillPlainSwordData _skillPlainSwordData;
        private GameSkillObject _gameSkillObject;

        private bool _isAniEnd = false;
        private float _attackInputTime = 0.0f;
        private float _attackBeInputTime = 0.0f;
        private float _attackTime = 0.5f;
        private int _skillPlainSwordNextIndex = 0;
        private uint _soundID;
        
        private AniState[] _skillPlainSwordAniArr =
            new AniState[] {AniState.SkillPlainSword, AniState.SkillPlainSword2, AniState.SkillPlainSword3};

        private FxAniEnum[] _skillPlainSwordFxAniArr = new FxAniEnum[]
            {FxAniEnum.SkillPlainSword, FxAniEnum.SkillPlainSword2, FxAniEnum.SkillPlainSword3};

        private string[] _skillPlainSwordSoundArr = new string[] {"PC_Snake_Sword1", "PC_Snake_Sword2", "PC_Snake_Sword3"};

        public SkillPlainSwordState(PlayerFSMSystem system, SkillPlainSwordData skillPlainSwordData) : base(system)
        {
            _skillPlainSwordData = skillPlainSwordData;
        }

        public override void StartState()
        {
            SystemMgr.OnAnimationEndEvent += OnAnimationEndEventCall;
            SystemMgr.AnimationCtrl.PlayAni(_skillPlainSwordAniArr[SystemMgr.Unit.skillPlainSwordIndex]);
            SystemMgr._fxCtrl.PlayAni(_skillPlainSwordFxAniArr[SystemMgr.Unit.skillPlainSwordIndex]);
            //SystemMgr._fxCtrl.PlayAni(FxAniEnum.SkillPlainSword2);
            WwiseSoundManager.instance.PlayEventSound(_skillPlainSwordSoundArr[SystemMgr.Unit.skillPlainSwordIndex]);

            _attackBeInputTime = Time.time;
            _attackTime = SystemMgr.Unit.SkillPlainSwordAttackTime;

            _gameSkillObject = InvokeSkill();
            
            GameManager.instance.uiMng.TurnXButtonUI(true);
        }

        public override void Update()
        {
        }

        public override void EndState()
        {
            if (SystemMgr.Unit.skillPlainSwordIndex == _skillPlainSwordAniArr.Length - 1)
            {
                SystemMgr.Unit.SkillPlainSwordEnd();
                WwiseSoundManager.instance.StopEventSoundFromId(_soundID);
            }
            
            SystemMgr.AnimationCtrl.SetSpeed(1);
            SystemMgr._fxCtrl.SetSpeed(1);
            
            SystemMgr.OnAnimationEndEvent -= OnAnimationEndEventCall;
            
            _skillPlainSwordNextIndex = 0;
            SystemMgr.Unit.skillPlainSwordIndex = 0;
            _attackBeInputTime = 0;
            _attackInputTime = 0;
            _isAniEnd = false;
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.Idle);
            
            GameManager.instance.uiMng.TurnXButtonUI(false);

        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Hit)
                return true;
            
            if (condition == TransitionCondition.SkillAxe)
                return true;
            if (condition == TransitionCondition.SkillHammer)
                return true;
            if (condition == TransitionCondition.SkillSpear)
                return true;
            if (condition == TransitionCondition.SkillKopsh)
                return true;
            
            if (_isAniEnd == true)
                return true;
            
            return false;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Attack)
            {
                _attackInputTime = Time.time;

                if (_attackInputTime - _attackBeInputTime <= _attackTime)
                {
                    _skillPlainSwordNextIndex = SystemMgr.Unit.skillPlainSwordIndex + 1;
                    SystemMgr.AnimationCtrl.SetSpeed(SystemMgr.Unit.AniFastAmount);
                    SystemMgr._fxCtrl.SetSpeed(SystemMgr.Unit.AniFastAmount);

                    if (SystemMgr.Unit.skillPlainSwordIndex == _skillPlainSwordAniArr.Length - 1)
                    {
                        SystemMgr.Unit.SkillPlainSwordFastInterval();
                    }
                }

                _attackBeInputTime = Time.time;

                return false;
            }

            if (SystemMgr.Unit.skillPlainSwordIndex == _skillPlainSwordAniArr.Length - 1)
            {
                if (condition == TransitionCondition.LeftMove)
                {
                    SystemMgr.Unit.CheckMovementDir(-1);
                    SystemMgr.Unit.Move();
                }

                if (condition == TransitionCondition.RightMove)
                {
                    SystemMgr.Unit.CheckMovementDir(1);
                    SystemMgr.Unit.Move();
                }
            }

            return true;
        }

        public bool IsAbleTransition()
        {
            return SystemMgr.Unit.IsAbleSkillPlainSword();
        }
        
        private void OnAnimationEndEventCall()
        {
            if(IsEndOrNextCheck() == true)
            {
                NextAction();
            }
            else
            {
                _isAniEnd = true;
                SystemMgr.Transition(TransitionCondition.Idle);
            }
        }

        private void NextAction()
        {
            SystemMgr.AnimationCtrl.SetSpeed(1);
            SystemMgr._fxCtrl.SetSpeed(1);   
            
            SystemMgr.AnimationCtrl.PlayAni(_skillPlainSwordAniArr[SystemMgr.Unit.skillPlainSwordIndex]);
            SystemMgr._fxCtrl.PlayAni(_skillPlainSwordFxAniArr[SystemMgr.Unit.skillPlainSwordIndex]);

            if (SystemMgr.Unit.skillPlainSwordIndex == 2)
            {
                _soundID = WwiseSoundManager.instance.PlayEventSound(
                    _skillPlainSwordSoundArr[SystemMgr.Unit.skillPlainSwordIndex]);
                
                GameManager.instance.uiMng.TurnXButtonUI(false);

            }
            else
            {
                WwiseSoundManager.instance.PlayEventSound(
                    _skillPlainSwordSoundArr[SystemMgr.Unit.skillPlainSwordIndex]);
            }

            _gameSkillObject = InvokeSkill();
        }

        private bool IsEndOrNextCheck()
        {
            if (SystemMgr.Unit.skillPlainSwordIndex != _skillPlainSwordNextIndex)
            {
                if (_skillPlainSwordNextIndex > _skillPlainSwordAniArr.Length - 1)
                {
                    return false;
                }
                else
                {
                    SystemMgr.Unit.skillPlainSwordIndex = _skillPlainSwordNextIndex;
                    return true;
                }
            }
            
            return false;
        }
        
        private GameSkillObject InvokeSkill()
        {
            var skillObejct = GameManager.instance.GameSkillMgr.GetSkillObject();

            if (skillObejct == null)
            {
                Debug.LogError("SkillObj is Null");
                return null;
            }

            skillObejct.InitSkill(_skillPlainSwordData.GetSkillController(skillObejct, SystemMgr.Unit));
            return skillObejct;
        }
    }
    
    
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
        //AddState(TransitionCondition.Wallslideing, new WallslideingState(this));
        AddState(TransitionCondition.WallJump, new WallJumpState(this));
        AddState(TransitionCondition.Dash, new DashState(this));
        AddState(TransitionCondition.Attack, new BasicAttackState(this));
        AddState(TransitionCondition.JumpAttack, new BasicJumpAttack(this));
        AddState(TransitionCondition.Hit, new HitState(this));
        //AddState(TransitionCondition.SkillShadowWalk, new SkillShadowWalkState(this, Unit.ShadowWalkSkillData));
        AddState(TransitionCondition.SkillAxe, new SkillAxeState(this, Unit.SkillAxeData));
        AddState(TransitionCondition.SkillSpear, new SkillSpearState(this, Unit.SkillSpearData));
        AddState(TransitionCondition.SkillHammer, new SkillHammerState(this, Unit.SkillHammerData));
        AddState(TransitionCondition.SkillKopsh, new SkillKopshState(this, Unit.SkillKopshData));
        AddState(TransitionCondition.SkillPlainSword, new SkillPlainSwordState(this, Unit.SkillPlainSwordData));
        AddState(TransitionCondition.Die, new DieState(this));
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
        {
            if (condition != TransitionCondition.None)
            {
                Debug.Log("Hitstop con : " + condition);
                _hitstopCondition = condition;
            }

            return false;
        }
        else
        {
            _hitstopCondition = TransitionCondition.None;
        }

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

    public void UnitDeadEventCall()
    {
        UnBind();
        // 캐릭터 죽음은 강제성이 필요해보임.
        // 그러므로  Transition이 아닌 강제 변경.
        ChangeState(TransitionCondition.Die);
    }

    public void StartJumpKeyPressDetectCoroutine()
    {
        StartCoroutine(JumpKeyPressDetectCoroutine());
    }

    private void OnIsBattleIdleEventCall(bool isBattle)
    {
        _isBattleIdle = isBattle;
    }
    

    private void OnCommandCastEventCall(string skillName, bool isReverse)
    {
        var condition = ChangeSkillNameToTransitionCondition(skillName);
        if (condition == TransitionCondition.None)
            return;

        
        // 스킬 사용이 불가능한 조건
        if (condition == TransitionCondition.SkillAxe || condition == TransitionCondition.SkillSpear ||
            condition == TransitionCondition.SkillKopsh)
        {
            if (CurrState == TransitionCondition.WallClimbing)
                return;
            if (CurrState == TransitionCondition.Hit)
                return;
        }
        else
        {
            if (CurrState == TransitionCondition.Jump)
                return;
            if (CurrState == TransitionCondition.Falling)
                return;
            if (CurrState == TransitionCondition.WallClimbing)
                return;
            if (CurrState == TransitionCondition.Hit)
                return;            
        }


        if (CheckSkillPossibleConditions(condition) == true)
        {
            CheckSkillActionPlayerDir(isReverse);
            Transition(condition);
        }

    }

    private TransitionCondition ChangeSkillNameToTransitionCondition(string skillName)
    {
        switch (skillName)
        {
            case "Skill1Axe":
                return TransitionCondition.SkillAxe;
                break;
            case "Skill2Spear":
                return TransitionCondition.SkillSpear;
                break;
            case "Skill3Hammer":
                return TransitionCondition.SkillHammer;
                break;
            case "Skill4Kopsh":
                return TransitionCondition.SkillKopsh;
                break;
            case "Skill5PlainSword":
                return TransitionCondition.SkillPlainSword;
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

    private void CheckSkillActionPlayerDir(bool isReverse)
    {
        if (isReverse == true)
        {
            Unit.CheckMovementDir(-1);
        }
        else
        {
            Unit.CheckMovementDir(1);
        }
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
        
        if (_hitstopCondition != TransitionCondition.None)
        {
            Transition(_hitstopCondition);
        }

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
