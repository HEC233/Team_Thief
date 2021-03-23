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
    Wallslideing = 10,
}

public interface IActor
{
    // 전이조건을 성공적으로 처리하면 True를 반환하도록 한다.
    bool Transition(TransitionCondition condition, object param = null);
}
