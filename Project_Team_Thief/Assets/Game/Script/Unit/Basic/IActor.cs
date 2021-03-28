using System;
using System.Collections;
using System.Collections.Generic;

public enum TransitionCondition
{
    Idle = 0,
    Move,
    LeftMove,
    RightMove,
    RunningInertia,
    Jump,
    DoubleJump,
    Falling,
    Roll,
    Wallslideing,
    WallJump,
    StopMove,
    Skill1,
    Skill2,
    Hit,
    Die,
    Attack,
    SetAttackBoxRight,
    SetAttackBoxLeft,
    ForceKill,
    JumpAttack,
    Dash = 99,
}

// transition에 추가적인 데이터를 넘겨주고 싶은 경우가 생길수 있으니 Object를 넘기는 법을 상의하자
public interface IActor
{
    // 전이조건을 성공적으로 처리하면 True를 반환하도록 한다.
    bool Transition(TransitionCondition condition, object param = null);
}
