using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kobold;

public class KoboldActor : MonoBehaviour, IActor
{
    Unit _kobold;
    KoboldState _curState;

    private void Update()
    {
        _curState.Process(this);
    }

    public bool Transition(TransitionCondition condition, object param = null)
    {
        _curState.Transition(this, condition);
        return false;
    }

    public void ChangeState(KoboldState newState)
    {
        _curState.Exit(this);
        _curState = newState;
        _curState.Enter(this);
    }
}

namespace Kobold
{
    public abstract class KoboldState
    {
        public abstract void Enter(KoboldActor actor);
        public abstract void Process(KoboldActor actor);
        public abstract void Exit(KoboldActor actor);
        public abstract bool Transition(KoboldActor actor, TransitionCondition condition);
    }


}