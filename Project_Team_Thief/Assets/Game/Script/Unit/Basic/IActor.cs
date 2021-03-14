using System;
using System.Collections;
using System.Collections.Generic;

public enum TransitionCondition
{
    Idle = 0,
    Move = 1,
    LeftMove = 2,
    RightMove = 3,
}

public interface IActor
{
    bool Transition(TransitionCondition condition);
}
