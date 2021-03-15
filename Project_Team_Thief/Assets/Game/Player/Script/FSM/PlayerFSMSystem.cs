using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public AnimationCtrl AnimationCtrl => _animationCtrl;

    // 애니메이션 관련 상태 변수
    private bool _isRunningInertiaAniEnd = false;
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        GameManager.instance.SetControlUnit(this);
    }

    private class IdleState : CustomFSMStateBase
    {
        public IdleState(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            SystemMgr.AnimationCtrl.PlayAni(AniState.Idle);
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
            // if (condition == TransitionCondition.LeftMove || condition == TransitionCondition.RightMove)
            //     return true;

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
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
            SystemMgr.Unit.Move(0);
        }

        public override void EndState()
        {
            SystemMgr.Unit.MoveStop();
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.LeftMove)
            {
                SystemMgr.Unit.CheckMovementDir(_inputDir * - 1);
                return false;
            }
            else if (condition == TransitionCondition.RightMove)
            {
                SystemMgr.Unit.CheckMovementDir(_inputDir);
                return false;
            }
            else if (condition == TransitionCondition.Idle)
            {
                if (SystemMgr.Unit.IsRunningInertia())
                {
                    SystemMgr.ChangeState(TransitionCondition.RunningInertia);
                    return false;
                }
            }
            
            return true;
        }
    }
    
    private class RunningInertiaState : CustomFSMStateBase
    {
        public RunningInertiaState(PlayerFSMSystem system) : base(system)
        {
            
        }

        public override void StartState()
        {
            SystemMgr.AnimationCtrl.PlayAni(AniState.RunningInertia);
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();
        }

        public override void EndState()
        {
            SystemMgr._isRunningInertiaAniEnd = false;
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Idle)
            {
                if (SystemMgr._isRunningInertiaAniEnd == false)
                    return false;
            }
            
            return true;
        }
    }
    
    private class JumpState : CustomFSMStateBase
    {
        private bool _isJumping = false;
        private float _jumpTime = 0.0f;
        private bool _isJumpKeyPress = true;
        
        public JumpState(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            if (SystemMgr.Unit.CheckIsJumpAble())
            {
                SystemMgr.AnimationCtrl.PlayAni(AniState.Jump);
                SystemMgr.Unit.Jump(0);
                _jumpTime = SystemMgr.Unit.MaxJumpTime;
                _isJumping = true;
                _isJumpKeyPress = true;
            }
        }

        public override void Update()
        {
            SystemMgr.Unit.Progress();

            if (Input.GetKeyUp(KeyCode.C))
            {
                _isJumpKeyPress = false;
            }

            if (_isJumpKeyPress == true)
            {
                _jumpTime -= 0.016f;

                if (_jumpTime >= 0)
                {
                    Debug.Log("asdasdsad");
                    SystemMgr.Unit.Jump(0);
                }
            }

            if (SystemMgr.Unit.IsGround == true)
            {
                _isJumping = false;
            }
        }

        public override void EndState()
        {
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (_isJumping == true)
            {
                return false;
            }
            
            return true;
        }
    }

    // Update is called once per frame
    protected override void RegisterState()
    {
        AddState(TransitionCondition.Idle, new IdleState(this));
        AddState(TransitionCondition.Move, new MoveState(this));
        AddState(TransitionCondition.RunningInertia, new RunningInertiaState(this));
        AddState(TransitionCondition.Jump, new JumpState(this));
    }
    
    public bool Transition(TransitionCondition condition)
    {
        if (CheckStateChangeAbleCondition(condition) == false)
            return false;

        // Left Move, Right Move에 대한 스테이트를 새로 만들기 싫어서 예외 처리 진행 함.
        if (condition == TransitionCondition.LeftMove || condition == TransitionCondition.RightMove)
        {
            ChangeState(TransitionCondition.Move);
        }

        if (CurrState == condition)
            return true;

        if (CurrState == condition)
            return false;

        ChangeState(condition);
        return true;
    }
 
    
    // 애니메이션 이벤트 관련 호출 함수
    public void RunningInertiaAniEndEvent()
    {
        _isRunningInertiaAniEnd = true;
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
    
}
