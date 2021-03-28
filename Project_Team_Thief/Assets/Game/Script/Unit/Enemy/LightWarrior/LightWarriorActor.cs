using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using LightWarrior;

public class LightWarriorActor : MonoBehaviour, IActor
{
    public LightWarriorUnit unit;
    public AnimationCtrl animCtrl;
    LWState _curState;

    public Idle idle = new Idle();
    public Move move = new Move();
    public Hit hit = new Hit();
    public Die die = new Die();
    public Attack attack = new Attack();

    private void Awake()
    {
        unit = GetComponentInParent<LightWarriorUnit>();
        Assert.IsNotNull(unit);
        unit.hitEvent.AddListener(HitTransition);
        unit.dieEvent.AddListener(DieTransition);
    }

    private void Start()
    {
        _curState = idle;
        _curState.Enter(this);
    }

    private void Update()
    {
        _curState.Process(this);
    }

    public bool Transition(TransitionCondition condition, object param = null)
    {
        // common change, this transition can occur in any state
        switch (condition)
        {
            case TransitionCondition.Hit:
                ChangeState(hit);
                return true;
            case TransitionCondition.Die:
                ChangeState(die);
                return true;
            case TransitionCondition.Attack:
                ChangeState(attack);
                return true;
            case TransitionCondition.ForceKill:
                ChangeState(die);
                return true;
        }
            
        return _curState.Transition(this, condition);
    }

    public void ChangeState(LWState newState)
    {
        if (_curState == newState)
            return;
        _curState.Exit(this);
        _curState = newState;
        _curState.Enter(this);
    }
    
    // 피격, 사망 이벤트는 간단하게 트랜지션을 발동시켜주는 것으로 구현하였다.
    private void HitTransition() { Transition(TransitionCondition.Hit); }
    private void DieTransition() { Transition(TransitionCondition.Die); }
}

namespace LightWarrior
{
    public abstract class LWState
    {
        public abstract void Enter(LightWarriorActor actor);
        public abstract void Process(LightWarriorActor actor);
        public abstract void Exit(LightWarriorActor actor);
        public abstract bool Transition(LightWarriorActor actor, TransitionCondition condition);
    }

    //=====================================================================
    public class Idle : LWState
    {
        public override void Enter(LightWarriorActor actor)
        {
            actor.animCtrl.PlayAni(AniState.Idle);
        }

        public override void Exit(LightWarriorActor actor)
        {
        }

        public override void Process(LightWarriorActor actor)
        {
        }

        public override bool Transition(LightWarriorActor actor, TransitionCondition condition)
        {
            switch(condition)
            {
                case TransitionCondition.RightMove:
                case TransitionCondition.LeftMove:
                    actor.ChangeState(actor.move);
                    return actor.Transition(condition);
            }

            return false;
        }
    }

    //=====================================================================
    public class Move : LWState
    {
        private enum InnerState
        {
            right,left,stop
        }
        private float _horizontalSpeed;
        private InnerState innerState;

        public override void Enter(LightWarriorActor actor)
        {
            innerState = InnerState.stop;
        }

        public override void Exit(LightWarriorActor actor)
        {
        }

        public override void Process(LightWarriorActor actor)
        {
            _horizontalSpeed = actor.unit.GetSpeed().x;
            if (Mathf.Approximately(_horizontalSpeed, 0))
                innerState = InnerState.stop;
            else if (_horizontalSpeed > 0)
                innerState = InnerState.right;
            else if (_horizontalSpeed < 0)
                innerState = InnerState.left;
        }

        public override bool Transition(LightWarriorActor actor, TransitionCondition condition)
        {
            switch (condition)
            {
                case TransitionCondition.RightMove:
                    if (innerState == InnerState.left)
                        actor.unit.Idle();
                    actor.animCtrl.SetFlip(true);
                    actor.animCtrl.PlayAni(AniState.Move);
                    actor.unit.Move(1);
                    return true;
                case TransitionCondition.LeftMove:
                    if (innerState == InnerState.right)
                        actor.unit.Idle();
                    actor.animCtrl.SetFlip(false);
                    actor.animCtrl.PlayAni(AniState.Move);
                    actor.unit.Move(-1);
                    return true;
                case TransitionCondition.StopMove:
                    actor.animCtrl.PlayAni(AniState.Idle);
                    actor.unit.MoveStop();
                    return true;
                case TransitionCondition.Idle:
                    actor.unit.Idle();
                    actor.ChangeState(actor.idle);
                    return true;
            }
            return false;
        }
    }

    //=====================================================================
    public class Hit : LWState
    {
        public override void Enter(LightWarriorActor actor)
        {
        }

        public override void Exit(LightWarriorActor actor)
        {
        }

        public override void Process(LightWarriorActor actor)
        {
        }

        public override bool Transition(LightWarriorActor actor, TransitionCondition condition)
        {
            return false;
        }
    }

    //=====================================================================
    public class Die : LWState
    {
        public override void Enter(LightWarriorActor actor)
        {
            Debug.Log("으악 죽었다");
            actor.unit.HandleDeath();
        }

        public override void Exit(LightWarriorActor actor)
        {
        }

        public override void Process(LightWarriorActor actor)
        {
        }

        public override bool Transition(LightWarriorActor actor, TransitionCondition condition)
        {
            return false;
        }
    }

    //=====================================================================s
    public class Attack : LWState
    {
        public override void Enter(LightWarriorActor actor)
        {
            actor.unit.Idle();
            actor.unit.Attack();
        }

        public override void Exit(LightWarriorActor actor)
        {

        }

        public override void Process(LightWarriorActor actor)
        {
            Transition(actor, TransitionCondition.Idle);
        }

        public override bool Transition(LightWarriorActor actor, TransitionCondition condition)
        {
            switch(condition)
            {
                case TransitionCondition.Idle:
                    actor.ChangeState(actor.idle);
                    return true;
            }

            return false;
        }
    }
}