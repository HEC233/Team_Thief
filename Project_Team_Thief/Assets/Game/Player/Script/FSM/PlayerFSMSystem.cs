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

public class PlayerFSMSystem : FSMSystem<TransitionCondition, CustomFSMStateBase>, IActor
{
    [SerializeField]
    private PlayerUnit _unit = null;

    public PlayerUnit Unit => _unit;

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
        }

        public override void Update()
        {
        }

        public override void EndState()
        {
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.LeftMove || condition == TransitionCondition.RightMove)
                return true;

            return false;
        }
    }
    
    private  class MoveState : CustomFSMStateBase
    {
        float _inputDir = 1;

        public MoveState(PlayerFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
        }

        public override void Update()
        {
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
            
            return true;
        }
    }

    // Update is called once per frame
    protected override void RegisterState()
    {
        AddState(TransitionCondition.Idle, new IdleState(this));
        AddState(TransitionCondition.Move, new MoveState(this));
    }
    
    public bool Transition(TransitionCondition condition)
    {
        if (CurrState == condition)
            return false;

        if (CheckStateChangeAbleCondition(condition) == false)
            return false;

        // Left Move, Right Move에 대한 스테이트를 새로 만들기 싫어서 예외 처리 진행 함.
        if (condition == TransitionCondition.LeftMove || condition == TransitionCondition.RightMove)
        {
            ChangeState(TransitionCondition.Move);
        }

        ChangeState(condition);
        return true;
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
