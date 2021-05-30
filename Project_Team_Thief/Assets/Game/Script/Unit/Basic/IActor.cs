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
    Dash,
    WallClimbing,
    Wallslideing,
    WallJump,
    StopMove,
    Skill1,
    Skill2,
    Hit,
    Die,
    Attack,
    LookRight,
    LookLeft,
    ForceKill,
    JumpAttack,
    BackStepRight,
    BackStepLeft,
    None,
    SkillAxe,
    SkillSpear,
    SkillHammer,

    // Shadow
    Default,
    Off,

    MouseMove,
    ArrowInput,
    DialogueNext,
}

// transition에 추가적인 데이터를 넘겨주고 싶은 경우가 생길수 있으니 Object를 넘기는 법을 상의하자
public interface IActor
{
    /// <summary>
    /// 전이조건을 성공적으로 처리하면 True를 반환하도록 한다.
    /// </summary>
    bool Transition(TransitionCondition condition, object param = null);

    Unit GetUnit();
}


