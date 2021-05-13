using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NullActor : IActor
{
    public Unit GetUnit()
    {
        return null;
    }

    public bool Transition(TransitionCondition condition, object param = null)
    {
        return false;
    }
}
