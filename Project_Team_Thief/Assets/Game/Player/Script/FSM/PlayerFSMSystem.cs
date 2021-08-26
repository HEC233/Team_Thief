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

        GameManager.instance.ShadowParticle.RegistCollider(GetComponent<BoxCollider2D>());
        //=======================================================
    }

    private void Bind()
    {
        GameManager.instance.TimeMng.startBulletTimeEvent += StartBulletTimeEvnetCall;
        GameManager.instance.TimeMng.endBulletTimeEvent += EndBulletTimeEventCall;
        GameManager.instance.TimeMng.startHitstopEvent += StartHitStopEventCall;
        GameManager.instance.TimeMng.endHitstopEvent += EndHitStopEvnetCall;
        Unit.hitEvent += UnitHitEventCall;
        Unit.OnPlayerDeadEvent += UnitDeadEventCall;
        
        //GameManager.instance.commandManager.OnCommandCastEvent += OnCommandCastEventCall;
        GameManager.instance.SkillSlotMng.OnCommandCastEvent += OnCommandCastEventCall;
    }

    private void UnBind()
    {
        GameManager.instance.TimeMng.startBulletTimeEvent -= StartBulletTimeEvnetCall;
        GameManager.instance.TimeMng.endBulletTimeEvent -= EndBulletTimeEventCall;
        GameManager.instance.TimeMng.startHitstopEvent -= StartHitStopEventCall;
        GameManager.instance.TimeMng.endHitstopEvent -= EndHitStopEvnetCall;
        Unit.hitEvent -= UnitHitEventCall;
        Unit.OnPlayerDeadEvent -= UnitDeadEventCall;
        
        //GameManager.instance.commandManager.OnCommandCastEvent -= OnCommandCastEventCall;
        GameManager.instance.SkillSlotMng.OnCommandCastEvent -= OnCommandCastEventCall;
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
                //SystemMgr.Unit.SetVelocityMaxMoveSpeed();
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
            // if (SystemMgr.Unit.IsRunningInertia())
            //     SystemMgr.Transition(TransitionCondition.RunningInertia);
            // else
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

            if (SystemMgr.Unit.GetVelocity().y <= -0.1f)
            {
                if (SystemMgr.Unit.CheckWallslideing())
                {
                    SystemMgr.Transition(TransitionCondition.WallClimbing);
                }
                else
                {
                    SystemMgr.Transition(TransitionCondition.Falling);
                }
            }
        }

        public override void EndState()
        {
            SystemMgr.Unit.SetPlayerDefaultPhysicMaterial();
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

                if (condition == TransitionCondition.DoubleJump)
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
            {
                if (SystemMgr.isJumpKeyPress == true)
                    return false;
                
                if (SystemMgr.Unit.CheckIsDoubleJump() == true)
                {
                    SystemMgr.Transition(TransitionCondition.DoubleJump);
                    return true;
                }
            }

            if (SystemMgr.Unit.CheckWallslideing())
            {
                Debug.Log("CheckWallSlideing");
                SystemMgr.Unit.SetPlayerPhysicMaterial();
            }
            else
            {
                SystemMgr.Unit.SetPlayerDefaultPhysicMaterial();
            }
                
            if (condition == TransitionCondition.LeftMove)
            {
                if (SystemMgr.Unit.CheckWallslideing() == false)
                {
                    SystemMgr.Unit.CheckMovementDir(_inputDir * -1);
                    SystemMgr.Unit.Move();
                }
            }

            if (condition == TransitionCondition.RightMove)
            {
                if (SystemMgr.Unit.CheckWallslideing() == false)
                {
                    SystemMgr.Unit.CheckMovementDir(_inputDir);
                    SystemMgr.Unit.Move();
                }
            }
            


            if (condition == TransitionCondition.None)
            {
                SystemMgr.Unit.MoveStop();
                //SystemMgr.Unit.JumpMoveStop();
            }

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
            SystemMgr._fxCtrl.PlayParticle(FxAniEnum.JumpFx);
            WwiseSoundManager.instance.PlayEventSound("PC_jump");

            SystemMgr.Unit.Jump();
            SystemMgr.StartJumpKeyPressDetectCoroutine();

        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
            
            if (SystemMgr.Unit.GetVelocity().y <= -0.1f)
            {
                if (SystemMgr.Unit.CheckWallslideing())
                {
                    SystemMgr.Transition(TransitionCondition.WallClimbing);
                }
                else
                {
                    SystemMgr.Transition(TransitionCondition.Falling);
                }
            }
        }

        public override void EndState()
        {
            SystemMgr.Unit.SetPlayerDefaultPhysicMaterial();
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

                if (condition == TransitionCondition.DoubleJump)
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
            if (SystemMgr.Unit.CheckWallslideing())
            {
                Debug.Log("CheckWallSlideing");
                SystemMgr.Unit.SetPlayerPhysicMaterial();
            }
            else
            {
                SystemMgr.Unit.SetPlayerDefaultPhysicMaterial();
            }
                
            if (condition == TransitionCondition.LeftMove)
            {
                if (SystemMgr.Unit.CheckWallslideing() == false)
                {
                    SystemMgr.Unit.CheckMovementDir(_inputDir * -1);
                    SystemMgr.Unit.Move();
                }
            }

            if (condition == TransitionCondition.RightMove)
            {
                if (SystemMgr.Unit.CheckWallslideing() == false)
                {
                    SystemMgr.Unit.CheckMovementDir(_inputDir);
                    SystemMgr.Unit.Move();
                }
            }
            


            if (condition == TransitionCondition.None)
            {
                SystemMgr.Unit.MoveStop();
                //SystemMgr.Unit.JumpMoveStop();
            }

            if (condition == TransitionCondition.Attack)
                SystemMgr.ChangeState(TransitionCondition.JumpAttack);
            

            return true;
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

                if (condition == TransitionCondition.DoubleJump)
                {
                    if (SystemMgr.Unit.CheckIsDoubleJump() == true)
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
                    if (SystemMgr.isJumpKeyPress == true)
                    {
                        return false;
                    }

                    if (SystemMgr.Unit.CheckIsJumpAble() == true)
                    {
                        SystemMgr.Transition(TransitionCondition.Jump);

                        return false;
                    }
                    else if (SystemMgr.Unit.CheckIsDoubleJump() == true)
                    {
                        SystemMgr.Transition(TransitionCondition.DoubleJump);

                        return false;
                    }
                }
                
                if (condition == TransitionCondition.LeftMove)
                {
                    SystemMgr.Unit.CheckMovementDir(_inputDir * - 1);
                    SystemMgr.Unit.Move();
                    if (SystemMgr.Unit.CheckWallslideing())
                        SystemMgr.Transition(TransitionCondition.WallClimbing);
                }
                if (condition == TransitionCondition.RightMove)
                {
                    SystemMgr.Unit.CheckMovementDir(_inputDir);
                    SystemMgr.Unit.Move();
                    if (SystemMgr.Unit.CheckWallslideing())
                        SystemMgr.Transition(TransitionCondition.WallClimbing);
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
            // if (SystemMgr.Unit.IsRollAble)
            // {
            //     //SystemMgr.AnimationCtrl.PlayAni(AniState.Dash);
            //     SystemMgr.Unit.SetRoll();
            //     SystemMgr.StartCoroutine(RollCoroutine());
            //     //SystemMgr.StartCoroutine(RollCoolTimeCoroutine());
            // }
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
        }

        public override void EndState()
        {
            //SystemMgr.Unit.EndRoll();
            //SystemMgr._isRollAniEnd = false;
        }

        public override bool Transition(TransitionCondition condition)
        {
            // if (SystemMgr.Unit.IsRollAble == false)
            //     return true;
            
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

        // IEnumerator RollCoroutine()
        // {
        //     _isRollEnd = true;
        //     _rollTime = SystemMgr.Unit.RollTime;
        //     _timer = 0.0f;
        //     while (_timer + GameManager.instance.timeMng.FixedDeltaTime < _rollTime)
        //     {
        //         _timer += GameManager.instance.timeMng.FixedDeltaTime;
        //         SystemMgr.Unit.Roll();
        //         yield return new WaitForFixedUpdate();
        //     }
        //     SystemMgr.Unit.RollStop();
        //     _isRollEnd = false;
        // }
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
            while (_timer + GameManager.instance.TimeMng.FixedDeltaTime < _dashTime)
            {
                _timer += GameManager.instance.TimeMng.FixedDeltaTime;
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
        private float _jumpInputTime = 0.0f;
        private float _jumpBeInputTime = 0.0f;
        private float _wallJumpEndTime = 0.02f;
        private bool _isFristCall = true;
        private TransitionCondition beforeCondition = TransitionCondition.None;
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
            _jumpInputTime = 0.0f;
            _jumpBeInputTime = 0.0f;
            _isFristCall = true;
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (SystemMgr.Unit.IsGround)
                return true;
            //
            // if (condition == TransitionCondition.Dash)
            //     return true;
            
            if (condition == TransitionCondition.WallClimbing)
                return true;
            if (condition == TransitionCondition.Falling)
            {
                return true;
            }

            return false;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            if (condition != TransitionCondition.Jump)
            {
                if (_jumpInputTime - _jumpBeInputTime >= _wallJumpEndTime)
                {
                    if (_isFristCall == true)
                    {

                        
                        //Debug.Log("WallJump End");
                        _isFristCall = false;
                        SystemMgr.Transition(TransitionCondition.Falling);
                        return true;
                    }
                }
            }
            


            // if (beforeCondition == TransitionCondition.Jump)
            // {
            //     if (condition == TransitionCondition.None)
            //     {
            //         Debug.Log("WallJump Input Key : " + condition);
            //         SystemMgr.Transition(TransitionCondition.Falling);
            //         return true;
            //     }
            // }

            if (condition == TransitionCondition.Jump)
            {
                _jumpBeInputTime = Time.time;
            }
            else
            {
                _jumpInputTime = Time.time;
            }

            return true;
        }
    }
    
    private class BasicAttackState : CustomFSMStateBase
    {
        private bool _isBasicAttackEnd = false;
        private bool _isInit = false;
        private SkillDataBase _basicAttackDataBase;
        private Damage _damage;
        private Coroutine _waitDelayCoroutine;

        public BasicAttackState(PlayerFSMSystem system) : base(system)
        {
        }

        private void Init()
        {
            if (_isInit == true)
            {
                return;
            }
            
            _isInit = true;
            _basicAttackDataBase = SkillDataBank.instance.GetSkillData(0);
            _damage = new Damage();
        }

        public override void StartState()
        {
            Init();
            SystemMgr.OnBasicAttackEndAniEvent += EndOrNextCheck;
            SystemMgr.OnBasicAttackCallEvent += BasicAttackCall;

            SystemMgr.AnimationCtrl.PlayAni(AniState.Attack);
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.BasicAttack);
            
            SystemMgr.Unit.SetBasicAttack();
            _damage.power = SystemMgr.Unit.CalcSkillDamage(_basicAttackDataBase.Damages[0]);
            _damage.knockBack = new Vector2(_basicAttackDataBase.KnockBackXs[0] * SystemMgr.Unit.FacingDir,
                _basicAttackDataBase.KnockBackYs[0]);
            _damage.additionalInfo = 0;
            _damage.stiffness = _basicAttackDataBase.Stiffness;

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

            StopWaitEndDelayCoroutine();
            
            _isBasicAttackEnd = false;

            SystemMgr.OnBasicAttackEndAniEvent -= EndOrNextCheck;
            SystemMgr.OnBasicAttackCallEvent -= BasicAttackCall;
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Attack)
                return false;
            
            if (condition == TransitionCondition.Hit)
                return true;
            
            
            if (condition == TransitionCondition.SkillAxe)
                return true;
            if (condition == TransitionCondition.SkillDoubleCross)
                return true;
            if (condition == TransitionCondition.SkillSnakeSwordSting)
                return true;
            if (condition == TransitionCondition.SkillKopsh)
                return true;

            if (_isBasicAttackEnd == true)
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
            return true;
        }

        private void EndOrNextCheck()
        {
            _waitDelayCoroutine = SystemMgr.StartCoroutine(WaitEndDelay());
        }
        
        private void BasicAttackCall()
        {
            SystemMgr.Unit.BasicAttack(_damage);
        }

        private void StopWaitEndDelayCoroutine()
        {
            if (_waitDelayCoroutine == null)
            {
                return;
            }
            
            if (_isBasicAttackEnd == false)
            {
                SystemMgr.StopCoroutine(_waitDelayCoroutine);
            }
        }

        IEnumerator WaitEndDelay()
        {
            float timer = 0.0f;
            while (_basicAttackDataBase.EndDelay >= timer)
            {
                timer += GameManager.instance.TimeMng.FixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _isBasicAttackEnd = true;
            SystemMgr.Transition(TransitionCondition.Idle);
        }
    }
    
    private class BasicJumpAttack : CustomFSMStateBase
    {
        private bool _isBasicjumpAttackAniEnd = false;
        private bool _isWaitDelay = false;
        private int _jumpAttackIndex = 0;
        private int _jumpAttackNextIndex = 0;
        private bool _isNotEndCoroutine = false;
        private Coroutine _jumpAttackMoveCoroutine;
        private float _attackInputTime = 0.0f;
        private float _attackBeInputTime = 0.0f;
        private SkillDataBase[] _basicJumpAttackData = new SkillDataBase[2];
        private bool _isInit = false;
        private Damage _damage = new Damage();

        private Vector2 _basicJumpAttackMoveDir = new Vector2(); 
        
        public BasicJumpAttack(PlayerFSMSystem system) : base(system)
        {
        }

        private void Init()
        {
            if (_isInit == true)
            {
                return;
            }

            _isInit = true;
            _damage = new Damage();
            _basicJumpAttackData[0] = SkillDataBank.instance.GetSkillData(1);
            _basicJumpAttackData[1] = SkillDataBank.instance.GetSkillData(2);

        }

        private void SetDamage()
        {
            _damage.power = SystemMgr.Unit.CalcSkillDamage(_basicJumpAttackData[_jumpAttackIndex].Damages[0]);
            _damage.knockBack = new Vector2(_basicJumpAttackData[_jumpAttackIndex].KnockBackXs[0] * SystemMgr.Unit.FacingDir,
                _basicJumpAttackData[_jumpAttackIndex].KnockBackYs[0]);
            _damage.stiffness = _basicJumpAttackData[_jumpAttackIndex].Stiffness;
            _damage.additionalInfo = _jumpAttackIndex;
        }

        public override void StartState()
        {
            Init();
            
            if (SystemMgr.Unit.isBasicJumpAttackAble == true)
            {
                _isNotEndCoroutine = true;
                SystemMgr.AnimationCtrl.PlayAni(AniState.JumpAttack);
                SystemMgr._fxCtrl.PlayAni(FxAniEnum.BasicJumpAttack);
                _jumpAttackMoveCoroutine = SystemMgr.Unit.StartCoroutine(BasicJumpAttackMoveCoroutine());

                SystemMgr.OnBasicAttackEndAniEvent += BasicJumpAttackAniEnd;
                SystemMgr.OnBasicAttackCallEvent += BasicJumpAttackCall;
                SystemMgr.Unit.isBasicJumpAttackAble = false;
                _attackBeInputTime = Time.time;

                SetDamage();
            }
            else
            {
                TransitionIdleOrFalling();
            }
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
        }

        public override void EndState()
        {
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.Idle);
            _isBasicjumpAttackAniEnd = false;
            _jumpAttackIndex = 0;
            _jumpAttackNextIndex = 0;
            _basicJumpAttackMoveDir = Vector2.zero;
            _isWaitDelay = false;

            SystemMgr.OnBasicAttackEndAniEvent -= BasicJumpAttackAniEnd;
            SystemMgr.OnBasicAttackCallEvent -= BasicJumpAttackCall;
            
            //SystemMgr.Unit.EndJumpAttackMove();
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (_isBasicjumpAttackAniEnd == false)
                return false;
            
            if (condition == TransitionCondition.Idle)
                return true;
            if (condition == TransitionCondition.Falling)
                return true;
            if (condition == TransitionCondition.Hit)
                return true;

            if (condition == TransitionCondition.Dash)
                return true;
            

            // else
            // {
            //     if (SystemMgr.Unit.IsGround == false)
            //     {
            //         if (condition == TransitionCondition.Falling)
            //             return true;
            //     }
            //}

            return false;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            if (_isWaitDelay == true)
                return true;
            
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
                _attackInputTime = Time.time;

                if (_attackInputTime - _attackBeInputTime <= SystemMgr.Unit.BasicJumpAttackTime)
                {
                    _jumpAttackNextIndex = _jumpAttackIndex + 1;
                }

            }

            return true;
        }

        private void TransitionIdleOrFalling()
        {
            if(SystemMgr.CurrState != TransitionCondition.JumpAttack)
                return;
            
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

        private void BasicJumpAttackMove()
        {
            _basicJumpAttackMoveDir.x = (1 / _basicJumpAttackData[_jumpAttackIndex].MoveTimes[0]) *
                                        _basicJumpAttackData[_jumpAttackIndex].MoveXs[0] * SystemMgr.Unit.FacingDir;
            _basicJumpAttackMoveDir.y = (1 / _basicJumpAttackData[_jumpAttackIndex].MoveTimes[0]) *
                                        _basicJumpAttackData[_jumpAttackIndex].MoveYs[0];
            _basicJumpAttackMoveDir *= GameManager.instance.TimeMng.TimeScale;
            SystemMgr.Unit.Rigidbody2D.velocity = Vector2.zero;
            SystemMgr.Unit.Rigidbody2D.AddForce(_basicJumpAttackMoveDir, ForceMode2D.Impulse);
        }
        
        IEnumerator BasicJumpAttackMoveCoroutine()
        {
            SystemMgr.Unit.SetJumpAttackMove();
            
            _isNotEndCoroutine = true;
            float _basicJumpAttackMoveTime = _basicJumpAttackData[_jumpAttackIndex].MoveTimes[0];
            float _timer = 0.0f;
            while (_timer < _basicJumpAttackMoveTime)
            {
                _timer += GameManager.instance.TimeMng.FixedDeltaTime;
                BasicJumpAttackMove();
                yield return new WaitForFixedUpdate();
            }
            
            _isNotEndCoroutine = false;
            SystemMgr.Unit.EndJumpAttackMove();
        }

        private void BasicJumpAttack2()
        {
            SetDamage();
            
            if (_isNotEndCoroutine == true)
            {
                SystemMgr.Unit.StopCoroutine(_jumpAttackMoveCoroutine);
            }
            
            SystemMgr.AnimationCtrl.PlayAni(AniState.JumpAttack2);
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.JumpAttackFx2);
            SystemMgr.Unit.StartCoroutine(BasicJumpAttackMoveCoroutine());
        }
        

        private void BasicJumpAttackAniEnd()
        {
            if (_jumpAttackNextIndex > _jumpAttackIndex && _jumpAttackNextIndex < 2)
            {
                _jumpAttackIndex = _jumpAttackNextIndex;
                BasicJumpAttack2();
            }
            else
            {
                SystemMgr.StartCoroutine(WaitEndDelay());
            }
        }

        private void BasicJumpAttackCall()
        {
            SystemMgr.Unit.BasicJumpAttack(_jumpAttackIndex, _damage);
        }
        
        IEnumerator WaitEndDelay()
        {
            _isWaitDelay = true;
            float timer = 0.0f;
            while (_basicJumpAttackData[_jumpAttackIndex].EndDelay >= timer)
            {
                timer += GameManager.instance.TimeMng.FixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _isBasicjumpAttackAniEnd = true;
            _isWaitDelay = false;
            TransitionIdleOrFalling();
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
            
            while (_timer + GameManager.instance.TimeMng.FixedDeltaTime < _hitTime)
            {
                _timer += GameManager.instance.TimeMng.FixedDeltaTime;
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

            if (condition == TransitionCondition.JumpAttack)
            {
                SystemMgr.Unit.CheckMovementDir(SystemMgr.Unit.FacingDir * -1);
                return true;
            }

            if (condition == TransitionCondition.WallJump)
            {
                return true;
            }

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

                if (condition == TransitionCondition.Attack)
                {
                    SystemMgr.Transition(TransitionCondition.JumpAttack);
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
            GameManager.instance.UIMng.PlayerDead();
        }
    }
    
    private class SkillAxeState : CustomFSMStateBase, ISkillStateBase
    {
        private SkillAxeData[] _skillAxeDataArr = new SkillAxeData[2];
        private GameSkillObject _gameSkillObject;
        private GameSkillObject _gameSkillObject2;
        private bool _isInit = false;
        private int _curSkillIndex = 0;
        private int _nextSkillIndex = 0;
        private bool _isSkillEnd = false;
        private float _attackInputTime = 0.0f;
        private float _attackBeInputTime = 0.0f;
        private float _attackTime = 1.5f;
        private Coroutine _waitDelayCoroutine;

        private AniState[] _skillThrowSpinAxeAniArr =
            new AniState[] {AniState.SkillAxe, AniState.SkillAxe2};

        private FxAniEnum[] skillThrowSpinAxeFxAniArr = new FxAniEnum[] {FxAniEnum.SkillAxe, FxAniEnum.SkillAxe2};
        
        public SkillAxeState(PlayerFSMSystem system) : base(system)
        {
        }
        
        private void Init()
        {
            if (_isInit == true)
            {
                return;
            }

            _isInit = true;
            _skillAxeDataArr[0] = SkillDataBank.instance.GetSkillData(3) as SkillAxeData;
            _skillAxeDataArr[1] = SkillDataBank.instance.GetSkillData(4) as SkillAxeData;
        }

        public override void StartState()
        {
            Init();
            SystemMgr.OnAnimationEndEvent += OnAnimationEndEventCall;
            SystemMgr.AnimationCtrl.PlayAni(_skillThrowSpinAxeAniArr[_curSkillIndex]);
            SystemMgr._fxCtrl.PlayAni(skillThrowSpinAxeFxAniArr[_curSkillIndex]);
            WwiseSoundManager.instance.PlayEventSound("PC_axe");
            GameManager.instance.UIMng.TurnXButtonUI(true);
            _attackBeInputTime = Time.time;
            _gameSkillObject = InvokeSkill();
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
        }

        public override void EndState()
        {
            SystemMgr.OnAnimationEndEvent -= OnAnimationEndEventCall;
            GameManager.instance.UIMng.TurnXButtonUI(false);
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.Idle);

            StopWaitEndDelayCoroutine();
            
            ResetValue();
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Hit)
                return true;

            if (condition == TransitionCondition.SkillAxe)
                return true;
            if (condition == TransitionCondition.SkillSnakeSwordSting)
                return true;

            if (_isSkillEnd == false)
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
                _attackInputTime = Time.time;

                if (_attackInputTime - _attackBeInputTime <= _attackTime)
                {
                    _nextSkillIndex = _curSkillIndex + 1;
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
            return true;
        }
        
        private void ResetValue()
        {
            _waitDelayCoroutine = null;
            _isSkillEnd = false;
            _curSkillIndex = 0;
            _nextSkillIndex = 0;
        }
        
        private void OnAnimationEndEventCall()
        {
            if (IsEndOrNextCheck() == true)
            {
                NextAction();
            }
            else
            {
                _waitDelayCoroutine = SystemMgr.StartCoroutine(WaitEndDelay());
            }
        }
        
        private bool IsEndOrNextCheck()
        {
            if (_curSkillIndex != _nextSkillIndex)
            {
                if (_nextSkillIndex > _skillThrowSpinAxeAniArr.Length - 1)
                {
                    return false;
                }
                else
                {
                    _curSkillIndex = _nextSkillIndex;
                    return true;
                }
            }
            
            return false;
        }
        
        private void NextAction()
        {
            SystemMgr.AnimationCtrl.SetSpeed(1);
            SystemMgr._fxCtrl.SetSpeed(1);   
            
            SystemMgr.AnimationCtrl.PlayAni(_skillThrowSpinAxeAniArr[_curSkillIndex]);
            SystemMgr._fxCtrl.PlayAni(skillThrowSpinAxeFxAniArr[_curSkillIndex]);
            GameManager.instance.UIMng.TurnXButtonUI(false);
            WwiseSoundManager.instance.PlayEventSound("PC_axe");

            _gameSkillObject = InvokeSkill();
        }
        
        private GameSkillObject InvokeSkill()
        {
            var skillObejct = GameManager.instance.GameSkillMng.GetSkillObject();

            if (skillObejct == null)
            {
                Debug.LogError("AexSkillObj is Null");
                return null;
            }
            
            skillObejct.InitSkill(_skillAxeDataArr[_curSkillIndex].GetSkillController(skillObejct, SystemMgr.Unit));
            return skillObejct;
        }
        
        private void StopWaitEndDelayCoroutine()
        {
            if (_waitDelayCoroutine == null)
            {
                return;
            }
            
            if (_isSkillEnd == false)
            {
                SystemMgr.StopCoroutine(_waitDelayCoroutine);
            }
        }
        
        IEnumerator WaitEndDelay()
        {
            float timer = 0.0f;
            while (_skillAxeDataArr[_curSkillIndex].EndDelay >= timer)
            {
                timer += GameManager.instance.TimeMng.FixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _isSkillEnd = true;
            SystemMgr.Transition(TransitionCondition.Idle);
        }
    }
    
    // private class SkillPlainSwordState : CustomFSMStateBase, ISkillStateBase
    // {
    //     private SkillPlainSwordData _skillPlainSwordData;
    //     private GameSkillObject _gameSkillObject;
    //
    //     private bool _isAniEnd = false;
    //     private float _attackInputTime = 0.0f;
    //     private float _attackBeInputTime = 0.0f;
    //     private float _attackTime = 0.5f;
    //     private int _skillPlainSwordNextIndex = 0;
    //     private uint _soundID;
    //     
    //     private AniState[] _skillPlainSwordAniArr =
    //         new AniState[] {AniState.SkillPlainSword, AniState.SkillPlainSword2, AniState.SkillPlainSword3};
    //
    //     private FxAniEnum[] _skillPlainSwordFxAniArr = new FxAniEnum[]
    //         {FxAniEnum.SkillPlainSword, FxAniEnum.SkillPlainSword2, FxAniEnum.SkillPlainSword3};
    //
    //     private string[] _skillPlainSwordSoundArr = new string[] {"PC_Snake_Sword1", "PC_Snake_Sword2", "PC_Snake_Sword3"};
    //
    //     public SkillPlainSwordState(PlayerFSMSystem system, SkillPlainSwordData skillPlainSwordData) : base(system)
    //     {
    //         _skillPlainSwordData = skillPlainSwordData;
    //     }
    //
    //     public override void StartState()
    //     {
    //         SystemMgr.OnAnimationEndEvent += OnAnimationEndEventCall;
    //         SystemMgr.AnimationCtrl.PlayAni(_skillPlainSwordAniArr[SystemMgr.Unit.skillPlainSwordIndex]);
    //         SystemMgr._fxCtrl.PlayAni(_skillPlainSwordFxAniArr[SystemMgr.Unit.skillPlainSwordIndex]);
    //         //WwiseSoundManager.instance.PlayEventSound(_skillPlainSwordSoundArr[SystemMgr.Unit.skillPlainSwordIndex]);
    //
    //         _attackTime = SystemMgr.Unit.SkillPlainSwordAttackTime;
    //         _attackBeInputTime = Time.time;
    //
    //         _gameSkillObject = InvokeSkill();
    //         
    //         GameManager.instance.uiMng.TurnXButtonUI(true);
    //     }
    //
    //     public override void Update()
    //     {
    //     }
    //
    //     public override void EndState()
    //     {
    //         if (SystemMgr.Unit.skillPlainSwordIndex == _skillPlainSwordAniArr.Length - 1)
    //         {
    //             SystemMgr.Unit.SkillPlainSwordEnd();
    //             WwiseSoundManager.instance.StopEventSoundFromId(_soundID);
    //         }
    //         
    //         SystemMgr.AnimationCtrl.SetSpeed(1);
    //         SystemMgr._fxCtrl.SetSpeed(1);
    //         
    //         SystemMgr.OnAnimationEndEvent -= OnAnimationEndEventCall;
    //         
    //         _skillPlainSwordNextIndex = 0;
    //         SystemMgr.Unit.skillPlainSwordIndex = 0;
    //         _attackBeInputTime = 0;
    //         _attackInputTime = 0;
    //         _isAniEnd = false;
    //         SystemMgr._fxCtrl.PlayAni(FxAniEnum.Idle);
    //         
    //         GameManager.instance.uiMng.TurnXButtonUI(false);
    //
    //     }
    //
    //     public override bool Transition(TransitionCondition condition)
    //     {
    //         if (condition == TransitionCondition.Hit)
    //             return true;
    //         if (condition == TransitionCondition.Jump)
    //             return true;
    //         
    //         if (condition == TransitionCondition.SkillAxe)
    //             return true;
    //         if (condition == TransitionCondition.SkillHammer)
    //             return true;
    //         if (condition == TransitionCondition.SkillSpear)
    //             return true;
    //         if (condition == TransitionCondition.SkillKopsh)
    //             return true;
    //         
    //         if (_isAniEnd == true)
    //             return true;
    //         
    //         return false;
    //     }
    //
    //     public override bool InputKey(TransitionCondition condition)
    //     {
    //         if (condition == TransitionCondition.Attack)
    //         {
    //             _attackInputTime = Time.time;
    //
    //             if (_attackInputTime - _attackBeInputTime <= _attackTime)
    //             {
    //                 _skillPlainSwordNextIndex = SystemMgr.Unit.skillPlainSwordIndex + 1;
    //                 SystemMgr.AnimationCtrl.SetSpeed(SystemMgr.Unit.AniFastAmount);
    //                 SystemMgr._fxCtrl.SetSpeed(SystemMgr.Unit.AniFastAmount);
    //
    //                 if (SystemMgr.Unit.skillPlainSwordIndex == _skillPlainSwordAniArr.Length - 1)
    //                 {
    //                     SystemMgr.Unit.SkillPlainSwordFastInterval();
    //                 }
    //             }
    //
    //             _attackBeInputTime = Time.time;
    //
    //             return false;
    //         }
    //
    //         if (SystemMgr.Unit.skillPlainSwordIndex == _skillPlainSwordAniArr.Length - 1)
    //         {
    //             if (condition == TransitionCondition.LeftMove)
    //             {
    //                 SystemMgr.Unit.CheckMovementDir(-1);
    //                 SystemMgr.Unit.Move();
    //             }
    //
    //             if (condition == TransitionCondition.RightMove)
    //             {
    //                 SystemMgr.Unit.CheckMovementDir(1);
    //                 SystemMgr.Unit.Move();
    //             }
    //         }
    //
    //         return true;
    //     }
    //
    //     public bool IsAbleTransition()
    //     {
    //         return true;
    //     }
    //     
    //     private void OnAnimationEndEventCall()
    //     {
    //         if(IsEndOrNextCheck() == true)
    //         {
    //             NextAction();
    //         }
    //         else
    //         {
    //             _isAniEnd = true;
    //             SystemMgr.Transition(TransitionCondition.Idle);
    //         }
    //     }
    //
    //     private void NextAction()
    //     {
    //         SystemMgr.AnimationCtrl.SetSpeed(1);
    //         SystemMgr._fxCtrl.SetSpeed(1);   
    //         
    //         SystemMgr.AnimationCtrl.PlayAni(_skillPlainSwordAniArr[SystemMgr.Unit.skillPlainSwordIndex]);
    //         SystemMgr._fxCtrl.PlayAni(_skillPlainSwordFxAniArr[SystemMgr.Unit.skillPlainSwordIndex]);
    //
    //         if (SystemMgr.Unit.skillPlainSwordIndex == 2)
    //         {
    //             // _soundID = WwiseSoundManager.instance.PlayEventSound(
    //             //     _skillPlainSwordSoundArr[SystemMgr.Unit.skillPlainSwordIndex]);
    //             
    //             GameManager.instance.uiMng.TurnXButtonUI(false);
    //
    //         }
    //         else
    //         {
    //             // WwiseSoundManager.instance.PlayEventSound(
    //             //     _skillPlainSwordSoundArr[SystemMgr.Unit.skillPlainSwordIndex]);
    //         }
    //
    //         _gameSkillObject = InvokeSkill();
    //     }
    //
    //     private bool IsEndOrNextCheck()
    //     {
    //         if (SystemMgr.Unit.skillPlainSwordIndex != _skillPlainSwordNextIndex)
    //         {
    //             if (_skillPlainSwordNextIndex > _skillPlainSwordAniArr.Length - 1)
    //             {
    //                 return false;
    //             }
    //             else
    //             {
    //                 SystemMgr.Unit.skillPlainSwordIndex = _skillPlainSwordNextIndex;
    //                 return true;
    //             }
    //         }
    //         
    //         return false;
    //     }
    //     
    //     private GameSkillObject InvokeSkill()
    //     {
    //         var skillObejct = GameManager.instance.GameSkillMgr.GetSkillObject();
    //
    //         if (skillObejct == null)
    //         {
    //             Debug.LogError("SkillObj is Null");
    //             return null;
    //         }
    //
    //         skillObejct.InitSkill(_skillPlainSwordData.GetSkillController(skillObejct, SystemMgr.Unit));
    //         return skillObejct;
    //     }
    // }

    private class SkillDoubleCrossState : CustomFSMStateBase, ISkillStateBase
    {
        private GameSkillObject _gameSkillObject;
        private SkillDoubleCrossData[] _skillDoubleCrossData = new SkillDoubleCrossData[2];
        private int _curSkillIndex = 0;
        private int _nextSkillIndex = 0;
        private float _attackInputTime = 0.0f;
        private float _attackBeInputTime = 0.0f;
        private float _attackTime = 1.5f;
        private bool _isSkillEnd = false;
        private bool _isInit = false;

        private AniState[] _skillDoubleCrossAniArr =
            new AniState[] {AniState.SkillDoubleCross, AniState.SkillDoubleCross2};

        // private FxAniEnum[] _skillPlainSwordFxAniArr = new FxAniEnum[]
        //     {FxAniEnum.SkillPlainSword, FxAniEnum.SkillPlainSword2, FxAniEnum.SkillPlainSword3};
        
        public SkillDoubleCrossState(PlayerFSMSystem system) : base(system)
        {
            
        }

        private void Init()
        {
            _isInit = true;
            _skillDoubleCrossData[0] = SkillDataBank.instance.GetSkillData(8) as SkillDoubleCrossData;
            _skillDoubleCrossData[1] = SkillDataBank.instance.GetSkillData(9) as SkillDoubleCrossData;
        }

        public override void StartState()
        {
            if (_isInit == false)
            {
                Init();
            }
            
            SystemMgr.OnAnimationEndEvent += OnAnimationEndEventCall;
            SystemMgr.AnimationCtrl.PlayAni(AniState.SkillDoubleCross);
            //GameManager.instance.uiMng.TurnXButtonUI(true);
            _attackBeInputTime = Time.time;
            _gameSkillObject = InvokeSkill();
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
        }

        public override void EndState()
        {
            SystemMgr.OnAnimationEndEvent -= OnAnimationEndEventCall;
            ResetValue();
            //GameManager.instance.uiMng.TurnXButtonUI(false);
        }

        private void ResetValue()
        {
            _isSkillEnd = false;
            _curSkillIndex = 0;
            _nextSkillIndex = 0;
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.SkillAxe)
                return true;
            if (condition == TransitionCondition.SkillSnakeSwordSting)
                return true;
            
            if (_isSkillEnd == false)
            {
                return false;
            }
            
            return true;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Attack)
            {
                _attackInputTime = Time.time;

                if (_attackInputTime - _attackBeInputTime <= _attackTime)
                {
                    _nextSkillIndex = _curSkillIndex + 1;
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
            return true;
        }
        
        private bool IsEndOrNextCheck()
        {
            if (_curSkillIndex != _nextSkillIndex)
            {
                if (_nextSkillIndex > _skillDoubleCrossAniArr.Length - 1)
                {
                    return false;
                }
                else
                {
                    _curSkillIndex = _nextSkillIndex;
                    return true;
                }
            }
            
            return false;
        }
        
        private void NextAction()
        {
            SystemMgr.AnimationCtrl.SetSpeed(1);
            SystemMgr._fxCtrl.SetSpeed(1);   
            
            SystemMgr.AnimationCtrl.PlayAni(_skillDoubleCrossAniArr[_curSkillIndex]);
            //SystemMgr._fxCtrl.PlayAni(_skillPlainSwordFxAniArr[SystemMgr.Unit.skillPlainSwordIndex]);
            
            _gameSkillObject = InvokeSkill();
        }
        
        private void OnAnimationEndEventCall()
        {
            if (IsEndOrNextCheck() == true)
            {
                NextAction();
            }
            else
            {
                SystemMgr.StartCoroutine(WaitEndDelay());
            }
        }
        
        private GameSkillObject InvokeSkill()
        {
            var skillObejct = GameManager.instance.GameSkillMng.GetSkillObject();

            if (skillObejct == null)
            {
                Debug.LogError("SkillObj is Null");
                return null;
            }

            skillObejct.InitSkill(_skillDoubleCrossData[_curSkillIndex]
                .GetSkillController(skillObejct, SystemMgr.Unit));
            return skillObejct;
        }

        IEnumerator WaitEndDelay()
        {
            float timer = 0.0f;
            while (_skillDoubleCrossData[_curSkillIndex].EndDelay >= timer)
            {
                timer += GameManager.instance.TimeMng.FixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _isSkillEnd = true;
            SystemMgr.Transition(TransitionCondition.Idle);
        }
    }

    private class SkillWallSummonState : CustomFSMStateBase, ISkillStateBase
    {
        public SkillWallSummonState(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }

        public override void EndState()
        {
            throw new NotImplementedException();
        }

        public override bool Transition(TransitionCondition condition)
        {
            return true;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            return true;
        }

        public bool IsAbleTransition()
        {
            return true;
        }
    }

    private class SkillSnakeSwordStingState : CustomFSMStateBase, ISkillStateBase
    {
        private GameSkillObject _gameSkillObject;
        private SkillSnakeSwordStingData[] _skillSnakeSwordStingDatas = new SkillSnakeSwordStingData[2];
        private bool _isInit = false;
        private int _curSkillIndex = 0;
        private int _nextSkillIndex = 0;
        private float _attackInputTime = 0.0f;
        private float _attackBeInputTime = 0.0f;
        private float _attackTime = 1.5f;
        private bool _isSkillEnd = false;
        private Coroutine _waitDelayCoroutine;

        private AniState[] _skillSnakeSwordStingAniArr =
            new AniState[] {AniState.SkillSnakeSwordSting1, AniState.SkillSnakeSwordSting2};
        
        public SkillSnakeSwordStingState(PlayerFSMSystem system) : base(system) { }
        private void Init()
        {
            if(_isInit == true)
                return;

            _isInit = true;
            _skillSnakeSwordStingDatas[0] = SkillDataBank.instance.GetSkillData(5) as SkillSnakeSwordStingData;
            _skillSnakeSwordStingDatas[1] = SkillDataBank.instance.GetSkillData(6) as SkillSnakeSwordStingData;
        }

        public override void StartState()
        {
            Init();
            
            SystemMgr.OnAnimationEndEvent += OnAnimationEndEventCall;
            SystemMgr.AnimationCtrl.PlayAni(AniState.SkillSnakeSwordStingDelay);
            //GameManager.instance.uiMng.TurnXButtonUI(true);
            _attackBeInputTime = Time.time;
            //_gameSkillObject = InvokeSkill();

            SystemMgr.StartCoroutine(WaitStartDelay());
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
        }

        public override void EndState()
        {
            StopWaitEndDelayCoroutine();
            
            ResetValue();
            SystemMgr.OnAnimationEndEvent -= OnAnimationEndEventCall;
        }
        
        private void ResetValue()
        {
            _isSkillEnd = false;
            _curSkillIndex = 0;
            _nextSkillIndex = 0;
            _waitDelayCoroutine = null;
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.SkillAxe)
                return true;
            if (condition == TransitionCondition.SkillDoubleCross)
                return true;
            if (condition == TransitionCondition.SkillSnakeSwordFlurry)
                return true;
            if (condition == TransitionCondition.SkillSnakeSwordSting)
                return true;
            
            if (_isSkillEnd == false)
            {
                return false;
            }
            return true;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Attack)
            {
                _attackInputTime = Time.time;

                if (_attackInputTime - _attackBeInputTime <= _attackTime)
                {
                    _nextSkillIndex = _curSkillIndex + 1;
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
            return true;
        }
        
        private GameSkillObject InvokeSkill()
        {
            var skillObejct = GameManager.instance.GameSkillMng.GetSkillObject();

            if (skillObejct == null)
            {
                Debug.LogError("SkillObj is Null");
                return null;
            }

            skillObejct.InitSkill(_skillSnakeSwordStingDatas[_curSkillIndex]
                .GetSkillController(skillObejct, SystemMgr.Unit));
            return skillObejct;
        }
        
        private void OnAnimationEndEventCall()
        {
            if (IsEndOrNextCheck() == true)
            {
                NextAction();
            }
            else
            {
                _waitDelayCoroutine = SystemMgr.StartCoroutine(WaitEndDelay());
            }
        }
        
        private bool IsEndOrNextCheck()
        {
            if (_curSkillIndex != _nextSkillIndex)
            {
                if (_nextSkillIndex > _skillSnakeSwordStingAniArr.Length - 1)
                {
                    return false;
                }
                else
                {
                    _curSkillIndex = _nextSkillIndex;
                    return true;
                }
            }
            
            return false;
        }
        
        private void NextAction()
        {
            SystemMgr.AnimationCtrl.SetSpeed(1);
            SystemMgr._fxCtrl.SetSpeed(1);   
            
            SystemMgr.AnimationCtrl.PlayAni(_skillSnakeSwordStingAniArr[_curSkillIndex]);
            //SystemMgr._fxCtrl.PlayAni(_skillPlainSwordFxAniArr[SystemMgr.Unit.skillPlainSwordIndex]);
            
            _gameSkillObject = InvokeSkill();
        }

        IEnumerator WaitStartDelay()
        {
            float timer = 0.0f;
            while (_skillSnakeSwordStingDatas[_curSkillIndex].FristDelay >= timer)
            {
                timer += GameManager.instance.TimeMng.FixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            
            NextAction();
        }
        
        private void StopWaitEndDelayCoroutine()
        {
            if (_waitDelayCoroutine == null)
            {
                return;
            }
            
            if (_isSkillEnd == false)
            {
                SystemMgr.StopCoroutine(_waitDelayCoroutine);
            }
        }

        IEnumerator WaitEndDelay()
        {
            float timer = 0.0f;
            _isSkillEnd = false;
            while (_skillSnakeSwordStingDatas[_curSkillIndex].EndDelay >= timer)
            {
                timer += GameManager.instance.TimeMng.FixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _isSkillEnd = true;
            SystemMgr.Transition(TransitionCondition.Idle);
        }
    }

    private class SkillSnakeSwordFlurryState : CustomFSMStateBase, ISkillStateBase
    {
        private GameSkillObject _gameSkillObject;
        private SkillSnakeSwordFlurryData _skillSnakeSwordFlurryData;
        private bool _isInit = false;
        private bool _isSkillEnd = false;
        private bool _isStartDelayEnd = false;
        private Coroutine _waitDelayCoroutine;
        
        
        public SkillSnakeSwordFlurryState(PlayerFSMSystem system) : base(system) { }

        private void Init()
        {
            if(_isInit == true)
                return;

            _isInit = true;
            _skillSnakeSwordFlurryData = SkillDataBank.instance.GetSkillData(7) as SkillSnakeSwordFlurryData;

            if (_skillSnakeSwordFlurryData == null)
            {
                return;
            }
                
            
            _skillSnakeSwordFlurryData.AnimationTime =
                SystemMgr.AnimationCtrl.GetAniTimeFromName("Ani_PC_SnakeSwordFlurry");
        }
        
        public override void StartState()
        {
            Init();
            SystemMgr.AnimationCtrl.PlayAni(AniState.SkillSnakeSwordFlurryDelay);
            SystemMgr.StartCoroutine(WaitStartDelay());
            SystemMgr.OnAnimationEndEvent += OnAnimationEndEventCall;
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
        }

        public override void EndState()
        {
            StopWaitEndDelayCoroutine();
            ResetValue();
            SystemMgr.OnAnimationEndEvent -= OnAnimationEndEventCall;
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.SkillAxe)
                return true;
            if (condition == TransitionCondition.SkillDoubleCross)
                return true;
            if (condition == TransitionCondition.SkillSnakeSwordSting)
                return true;
            if (condition == TransitionCondition.SkillSnakeSwordFlurry)
                return true;
            
            if (_isSkillEnd == false)
            {
                return false;
            }
            return true;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            if (_isStartDelayEnd == true)
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
            return true;
        }
        
        private GameSkillObject InvokeSkill()
        {
            var skillObejct = GameManager.instance.GameSkillMng.GetSkillObject();

            if (skillObejct == null)
            {
                Debug.LogError("SkillObj is Null");
                return null;
            }

            skillObejct.InitSkill(_skillSnakeSwordFlurryData.GetSkillController(skillObejct, SystemMgr.Unit));
            return skillObejct;
        }
        
        private void Action()
        {
            SystemMgr.AnimationCtrl.PlayAni(AniState.SkillSnakeSwordFlurry);
            //SystemMgr._fxCtrl.PlayAni(_skillPlainSwordFxAniArr[SystemMgr.Unit.skillPlainSwordIndex]);
            
            _gameSkillObject = InvokeSkill();
        }
        
        private void ResetValue()
        {
            _isSkillEnd = false;
            _waitDelayCoroutine = null;
            _isStartDelayEnd = false;
        }
        
        private void OnAnimationEndEventCall()
        {
            SystemMgr._unit.SkillSnakeSwordFlurryEnd();
            _waitDelayCoroutine = SystemMgr.StartCoroutine(WaitEndDelay());
        }
        
        private void StopWaitEndDelayCoroutine()
        {
            if (_waitDelayCoroutine == null)
            {
                return;
            }
            
            if (_isSkillEnd == false)
            {
                SystemMgr.StopCoroutine(_waitDelayCoroutine);
            }
        }
        
        IEnumerator WaitStartDelay()
        {
            float timer = 0.0f;
            while (_skillSnakeSwordFlurryData.FristDelay >= timer)
            {
                timer += GameManager.instance.TimeMng.FixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _isStartDelayEnd = true;
            Action();
        }
        
        IEnumerator WaitEndDelay()
        {
            float timer = 0.0f;
            _isSkillEnd = false;
            _isStartDelayEnd = false;
            while (_skillSnakeSwordFlurryData.EndDelay >= timer)
            {
                timer += GameManager.instance.TimeMng.FixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _isSkillEnd = true;
            SystemMgr.Transition(TransitionCondition.Idle);
        }
    }

    private class SkillBaldoState : CustomFSMStateBase, ISkillStateBase
    {
        private GameSkillObject _gameSkillObject;
        private SkillBaldoData _skillBaldoData;
        private bool _isInit = false;
        private bool _isSkillEnd = false;
        private bool _isStartDelayEnd = false;
        private int _aniIndex = 0;
        private Coroutine _waitDelayCoroutine;
        
        public SkillBaldoState(PlayerFSMSystem system) : base(system) { }

        private void Init()
        {
            if(_isInit == true)
                return;

            _isInit = true;
            _skillBaldoData = SkillDataBank.instance.GetSkillData(17) as SkillBaldoData;
            
        }
        
        public override void StartState()
        {
            Init();
            SystemMgr.OnAnimationEndEvent += OnAnimationEndEventCall;
            SystemMgr.AnimationCtrl.PlayAni(AniState.SkillBaldoCast);
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.SkillBaldoCast);
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
        }

        public override void EndState()
        {
            StopWaitEndDelayCoroutine();
            ResetValue();
            SystemMgr.OnAnimationEndEvent -= OnAnimationEndEventCall;
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.Idle);
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.SkillAxe)
                return true;
            if (condition == TransitionCondition.SkillDoubleCross)
                return true;
            if (condition == TransitionCondition.SkillSnakeSwordSting)
                return true;
            if (condition == TransitionCondition.SkillSnakeSwordFlurry)
                return true;
            if (condition == TransitionCondition.SkillBaldo)
                return true;
            
            if (_isSkillEnd == false)
            {
                return false;
            }
            return true;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            return true;
        }
        
        private void ResetValue()
        {
            _isSkillEnd = false;
            _waitDelayCoroutine = null;
            _isStartDelayEnd = false;
            _aniIndex = 0;
        }
        
        private void OnAnimationEndEventCall()
        {
            if (_aniIndex == 0)
            {
                _aniIndex++;
                SystemMgr.AnimationCtrl.PlayAni(AniState.SkillBaldoDelay);
                SystemMgr._fxCtrl.PlayAni(FxAniEnum.SkillBaldoDelay);

                BulletTime();
                SystemMgr.StartCoroutine(WaitStartDelay());
            }
            else
            {
                _waitDelayCoroutine = SystemMgr.StartCoroutine(WaitEndDelay());
            }
        }

        private void BulletTime()
        {
            GameManager.instance.TimeMng.BulletTime(_skillBaldoData.BulletTimeScale, _skillBaldoData.BulletTimeAmount);
        }
        
        private void Action()
        {
            SystemMgr.AnimationCtrl.PlayAni(AniState.SkillBaldo);
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.SkillBaldo);

            
            _gameSkillObject = InvokeSkill();
        }
        
        private void StopWaitEndDelayCoroutine()
        {
            if (_waitDelayCoroutine == null)
            {
                return;
            }
            
            if (_isSkillEnd == false)
            {
                SystemMgr.StopCoroutine(_waitDelayCoroutine);
            }
        }
        
        private GameSkillObject InvokeSkill()
        {
            var skillObejct = GameManager.instance.GameSkillMng.GetSkillObject();

            if (skillObejct == null)
            {
                Debug.LogError("SkillObj is Null");
                return null;
            }

            skillObejct.InitSkill(_skillBaldoData.GetSkillController(skillObejct, SystemMgr.Unit));
            return skillObejct;
        }
        
        IEnumerator WaitStartDelay()
        {
            float timer = 0.0f;
            while (_skillBaldoData.FristDelay >= timer)
            {
                timer += GameManager.instance.TimeMng.FixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _isStartDelayEnd = true;
            Action();
        }
        
        IEnumerator WaitEndDelay()
        {
            float timer = 0.0f;
            _isSkillEnd = false;
            _isStartDelayEnd = false;
            while (_skillBaldoData.EndDelay >= timer)
            {
                timer += GameManager.instance.TimeMng.FixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _isSkillEnd = true;
            SystemMgr.Transition(TransitionCondition.Idle);
        }

        public bool IsAbleTransition()
        {
            return true;
        }
    }

    private class SkillSheatingState : CustomFSMStateBase, ISkillStateBase
    {
         private GameSkillObject _gameSkillObject;
        private SkillSheatingData _skillSheatingData;
        private bool _isInit = false;
        private bool _isSkillEnd = false;
        private bool _isStartDelayEnd = false;
        private int _aniIndex = 0;
        private Coroutine _waitDelayCoroutine;
        
        public SkillSheatingState(PlayerFSMSystem system) : base(system) { }

        private void Init()
        {
            if(_isInit == true)
                return;

            _isInit = true;
            _skillSheatingData = SkillDataBank.instance.GetSkillData(18) as SkillSheatingData;
            
        }
        
        public override void StartState()
        {
            Init();
            SystemMgr.OnAnimationEndEvent += OnAnimationEndEventCall;
            SystemMgr.AnimationCtrl.PlayAni(AniState.SkillSheatingCast);
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.SkillSheatingCast);
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
        }

        public override void EndState()
        {
            StopWaitEndDelayCoroutine();
            ResetValue();
            SystemMgr.OnAnimationEndEvent -= OnAnimationEndEventCall;
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.Idle);
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.SkillAxe)
                return true;
            if (condition == TransitionCondition.SkillDoubleCross)
                return true;
            if (condition == TransitionCondition.SkillSnakeSwordSting)
                return true;
            if (condition == TransitionCondition.SkillSnakeSwordFlurry)
                return true;
            if (condition == TransitionCondition.SkillBaldo)
                return true;
            if (condition == TransitionCondition.SkillSheating)
                return true;
            
            if (_isSkillEnd == false)
            {
                return false;
            }
            return true;
        }

        public override bool InputKey(TransitionCondition condition)
        {
            return true;
        }
        
        private void ResetValue()
        {
            _isSkillEnd = false;
            _waitDelayCoroutine = null;
            _isStartDelayEnd = false;
            _aniIndex = 0;
        }
        
        private void OnAnimationEndEventCall()
        {
            if (_aniIndex == 0)
            {
                _aniIndex++;
                SystemMgr.AnimationCtrl.PlayAni(AniState.SkillSheatingDelay);
                SystemMgr._fxCtrl.PlayAni(FxAniEnum.SkillSheatingDelay);

                //BulletTime();
                SystemMgr.StartCoroutine(WaitStartDelay());
            }
            else
            {
                _waitDelayCoroutine = SystemMgr.StartCoroutine(WaitEndDelay());
            }
        }

        private void BulletTime()
        {
            //GameManager.instance.TimeMng.BulletTime(_skillSheatingData.BulletTimeScale, _skillSheatingData.BulletTimeAmount);
        }
        
        private void Action()
        {
            SystemMgr.AnimationCtrl.PlayAni(AniState.SkillSheating);
            SystemMgr._fxCtrl.PlayAni(FxAniEnum.SkillSheating);

            
            _gameSkillObject = InvokeSkill();
        }
        
        private void StopWaitEndDelayCoroutine()
        {
            if (_waitDelayCoroutine == null)
            {
                return;
            }
            
            if (_isSkillEnd == false)
            {
                SystemMgr.StopCoroutine(_waitDelayCoroutine);
            }
        }
        
        private GameSkillObject InvokeSkill()
        {
            var skillObejct = GameManager.instance.GameSkillMng.GetSkillObject();

            if (skillObejct == null)
            {
                Debug.LogError("SkillObj is Null");
                return null;
            }

            skillObejct.InitSkill(_skillSheatingData.GetSkillController(skillObejct, SystemMgr.Unit));
            return skillObejct;
        }
        
        IEnumerator WaitStartDelay()
        {
            float timer = 0.0f;
            while (_skillSheatingData.FristDelay >= timer)
            {
                timer += GameManager.instance.TimeMng.FixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _isStartDelayEnd = true;
            Action();
        }
        
        IEnumerator WaitEndDelay()
        {
            float timer = 0.0f;
            _isSkillEnd = false;
            _isStartDelayEnd = false;
            while (_skillSheatingData.EndDelay >= timer)
            {
                timer += GameManager.instance.TimeMng.FixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            _isSkillEnd = true;
            SystemMgr.Transition(TransitionCondition.Idle);
        }

        public bool IsAbleTransition()
        {
            return true;
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
        AddState(TransitionCondition.WallClimbing, new WallClimbingState(this));
        //AddState(TransitionCondition.Wallslideing, new WallslideingState(this));
        AddState(TransitionCondition.WallJump, new WallJumpState(this));
        AddState(TransitionCondition.Dash, new DashState(this));
        AddState(TransitionCondition.Attack, new BasicAttackState(this));
        AddState(TransitionCondition.JumpAttack, new BasicJumpAttack(this));
        AddState(TransitionCondition.Hit, new HitState(this));
        AddState(TransitionCondition.SkillAxe, new SkillAxeState(this));
        //AddState(TransitionCondition.SkillPlainSword, new SkillPlainSwordState(this, Unit.SkillPlainSwordData));
        AddState(TransitionCondition.SkillDoubleCross, new SkillDoubleCrossState(this));
        AddState(TransitionCondition.SkillWallSummon, new SkillWallSummonState(this));
        AddState(TransitionCondition.SkillSnakeSwordSting, new SkillSnakeSwordStingState(this));
        AddState(TransitionCondition.SkillSnakeSwordFlurry, new SkillSnakeSwordFlurryState(this));
        AddState(TransitionCondition.SkillBaldo, new SkillBaldoState(this));
        AddState(TransitionCondition.SkillSheating, new SkillSheatingState(this));
        AddState(TransitionCondition.Die, new DieState(this));
    }
    
    public bool Transition(TransitionCondition condition, object param = null)
    {
        if (GetState(CurrState).InputKey(condition) == false)
        {
            return false;
        }

        // 가져 올 State가 Skill이라는 확정사항이므로 괜찮은걸까?
        // 스킬의 경우 같은 스테이트로 재진입 하는 것을 허용해야 하므로 아래와 같이 처리함.
        var state = GetState(condition) as ISkillStateBase;
        if (state == null)
        {
            if (CurrState == condition)
            {
                return false;
            }
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

        if (GameManager.instance.TimeMng.IsHitStop == true)
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
    

    private void OnCommandCastEventCall(TransitionCondition skillCondition, bool isReverse)
    {
        var condition = skillCondition;
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

    // private TransitionCondition ChangeSkillNameToTransitionCondition(string skillName)
    // {
    //     switch (skillName)
    //     {
    //         case "회전도끼 던지기":
    //             return TransitionCondition.SkillAxe;
    //             break;
    //         case "더블크로스":
    //             return TransitionCondition.SkillDoubleCross;
    //             break;
    //         case "사복검 찌르기":
    //             return TransitionCondition.SkillSnakeSwordSting;
    //             break;
    //         default:
    //             break;
    //     }
    //
    //     return TransitionCondition.None;
    // }

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
