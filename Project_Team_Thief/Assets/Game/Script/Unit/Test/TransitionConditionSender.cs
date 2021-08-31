using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionConditionSender : MonoBehaviour
{
    public LightWarriorActor actor;

    private void Start()
    {
        GameManager.instance.ControlActor = actor;
    }

    public void SendCondition(TransitionCondition condition)
    {
        actor.Transition(condition);
    }
    public void SendConditionRight()
    {
        actor.Transition(TransitionCondition.RightMove);
    }
    public void SendConditionLeft()
    {
        actor.Transition(TransitionCondition.LeftMove);
    }
    public void SendConditionStop()
    {
        actor.Transition(TransitionCondition.StopMove);
    }
}
