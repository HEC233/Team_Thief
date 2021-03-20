using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using LightWarrior;

public class LightWarriorActor : MonoBehaviour, IActor
{
    public LightWarriorUnit unit;
    LWState _curState;

    private void Awake()
    {
        unit = GetComponentInParent<LightWarriorUnit>();
        Assert.IsNotNull(unit);
    }

    private void Start()
    {
        _curState = new Idle();
        _curState.Enter(this);
    }

    private void Update()
    {
        _curState.Process(this);
    }

    public bool Transition(TransitionCondition condition)
    {
        _curState.Transition(this, condition);
        return false;
    }

    public void ChangeState(LWState newState)
    {
        _curState.Exit(this);
        _curState = newState;
        _curState.Enter(this);
    }
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

    public class Idle : LWState
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
            switch(condition)
            {
                case TransitionCondition.RightMove:
                case TransitionCondition.LeftMove:
                case TransitionCondition.StopMove:
                    actor.ChangeState(new Move());
                    return true;
                    break;
            }

            return false;
        }
    }

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
        }

        public override void Exit(LightWarriorActor actor)
        {
        }

        public override void Process(LightWarriorActor actor)
        {
            _horizontalSpeed = actor.unit.GetSpeed().x;
            if (_horizontalSpeed > 0)
                innerState = InnerState.right;
            else if (_horizontalSpeed < 0)
                innerState = InnerState.left;
            else
                innerState = InnerState.stop;
        }

        public override bool Transition(LightWarriorActor actor, TransitionCondition condition)
        {
            switch(condition)
            {
                case TransitionCondition.RightMove:
                    if (innerState == InnerState.left)
                        actor.unit.Idle();
                    actor.unit.Move(1);
                    break;
                case TransitionCondition.LeftMove:
                    if (innerState == InnerState.right)
                        actor.unit.Idle();
                    actor.unit.Move(-1);
                    break;
                case TransitionCondition.StopMove:
                    actor.unit.Idle();
                    break;
            }
            return false;
        }
    }

}