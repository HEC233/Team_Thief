using System;
using System.Collections;
using System.Collections.Generic;

public enum TransitionCondition
{
    Idle = 0,
    Move = 1,
    LeftMove = 2,
    RightMove = 3,
    RunningInertia = 4,
    Jump = 5,
    DoubleJump = 7,
    Falling = 8,
    Roll = 9,
}

public interface IActor
{
    bool Transition(TransitionCondition condition);
}
