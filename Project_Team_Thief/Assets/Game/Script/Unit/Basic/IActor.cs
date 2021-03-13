using System;
using System.Collections;
using System.Collections.Generic;

public enum TransitionCondition
{
    Idle = 0,
    Move = 1,
}

public interface IActor
{
    bool Transition(TransitionCondition condition);
}
