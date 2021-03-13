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
    private PlayerUnit _unit = null;

    public PlayerUnit Unit => _unit;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Init()
    {
        SetUnit(GetComponentInChildren<PlayerUnit>());
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
            
        }
    }
    
    private  class MoveState : CustomFSMStateBase
    {
        public MoveState(PlayerFSMSystem system) : base(system)
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
