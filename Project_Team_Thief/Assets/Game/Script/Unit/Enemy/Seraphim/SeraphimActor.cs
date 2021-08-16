using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using PS.Enemy.Seraphim;

public class SeraphimActor : MonoBehaviour, IActor
{
    public SeraphimUnit unit;
    public AnimationCtrl animCtrl;
    public WwiseSoundCtrl wwiseSoundCtrl;
    private SPState _curState;

    public Idle idle = new Idle();
    public Move move = new Move();
    public Hit hit = new Hit();
    public Die die = new Die();
    public Attack attack = new Attack();
    public BackStep backStep = new BackStep();
    public static Null nullState = new Null();

    public SeraphimFireFXCtrl fireFX;
    [HideInInspector] public bool flipValue = false;

    public Vector2 BackStepPower = new Vector2();

    public Unit GetUnit()
    {
        return unit;
    }

    public bool Transition(TransitionCondition condition, object param = null)
    {
        bool result = _curState.Transition(this, condition);
        if(!result)
        {
            switch (condition)
            {
                case TransitionCondition.Hit:
                    ChangeState(hit);
                    result = true;
                    break;
                case TransitionCondition.Die:
                    ChangeState(die);
                    result = true;
                    break;
                case TransitionCondition.ForceKill:
                    ChangeState(die);
                    result = true;
                    break;
                case TransitionCondition.LookRight:
                    flipValue = true;
                    animCtrl.SetFlip(flipValue);
                    unit.SetAttackBoxDir(true);
                    result = true;
                    break;
                case TransitionCondition.LookLeft:
                    flipValue = false;
                    animCtrl.SetFlip(flipValue);
                    unit.SetAttackBoxDir(false);
                    result = true;
                    break;
            }
        }
        return result;
    }

    public void ChangeState(SPState newState)
    {
        if (_curState == newState)
            return;
        _curState.Exit(this);
        _curState = newState;
        _curState.Enter(this);
    }

    void Awake()
    {
        unit = GetComponent<SeraphimUnit>();
        Assert.IsNotNull(unit);
        unit.hitEvent.AddListener(HitTransition);
        unit.dieEvent.AddListener(DieTransition);
    }

    // Start is called before the first frame update
    void Start()
    {
        _curState = idle;
        _curState.Enter(this);

        RegistEventForGameManager();
    }

    // Update is called once per frame
    void Update()
    {
        //if (unit.transform.position.y < -1000)
        //    DieTransition();
        if (GameManager.instance.isPlayerDead)
            idle.Process(this);
        else
            _curState.Process(this);
    }
    private void TimeScaleChangeEnterCallback(float customTimeScale)
    {
        animCtrl.SetSpeed(customTimeScale);
        unit.TimeScaleChangeEnter(customTimeScale);
    }
    private void TimeScaleChangeExitCallback(float customTimeScale)
    {
        animCtrl.SetSpeed(1.0f);
        unit.TimeScaleChangeExit();
    }
    public void RegistEventForGameManager()
    {
        var v = GameManager.instance.TimeMng;
        if (v)
        {
            v.startBulletTimeEvent += TimeScaleChangeEnterCallback;
            v.endBulletTimeEvent += TimeScaleChangeExitCallback;
            v.startHitstopEvent += TimeScaleChangeEnterCallback;
            v.endHitstopEvent += TimeScaleChangeExitCallback;
        }
    }

    public void UnregistEventForGameManager()
    {
        var v = GameManager.instance.TimeMng;
        if (v)
        {
            v.startBulletTimeEvent -= TimeScaleChangeEnterCallback;
            v.endBulletTimeEvent -= TimeScaleChangeExitCallback;
            v.startHitstopEvent -= TimeScaleChangeEnterCallback;
            v.endHitstopEvent -= TimeScaleChangeExitCallback;
        }
    }

    // 피격, 사망 이벤트는 간단하게 트랜지션을 발동시켜주는 것으로 구현하였다.
    private void HitTransition() { Transition(TransitionCondition.Hit); }
    private void DieTransition() { Transition(TransitionCondition.Die); }
    public UnityEvent ActionEnd;
    [HideInInspector] public bool attackAnimEnd = false;
    public void AttackAnimEnd() { attackAnimEnd = true; }
}

namespace PS.Enemy.Seraphim
{
    public abstract class SPState
    {
        public abstract void Enter(SeraphimActor actor);
        public abstract void Process(SeraphimActor actor);
        public abstract void Exit(SeraphimActor actor);
        public abstract bool Transition(SeraphimActor actor, TransitionCondition condition);
    }
    //=====================================================================
    public class Idle : SPState
    {
        public override void Enter(SeraphimActor actor)
        {
            actor.animCtrl.PlayAni(AniState.Idle);
        }

        public override void Exit(SeraphimActor actor)
        {
        }

        public override void Process(SeraphimActor actor)
        {
        }

        public override bool Transition(SeraphimActor actor, TransitionCondition condition)
        {
            switch (condition)
            {
                case TransitionCondition.RightMove:
                case TransitionCondition.LeftMove:
                case TransitionCondition.StopMove:
                    actor.ChangeState(actor.move);
                    return actor.Transition(condition);
                case TransitionCondition.Hit:
                    actor.ChangeState(actor.hit);
                    return true;
                case TransitionCondition.Attack:
                    actor.ChangeState(actor.attack);
                    return true;
                case TransitionCondition.BackStepLeft:
                case TransitionCondition.BackStepRight:
                    actor.ChangeState(actor.backStep);
                    return actor.Transition(condition);
            }

            return false;
        }
    }
    //=====================================================================
    public class Move : SPState
    {
        uint soundID;
        bool bSoundPlaying = false;

        private enum InnerState
        {
            right, left, stop
        }
        private float _horizontalSpeed;
        private InnerState innerState;

        public override void Enter(SeraphimActor actor)
        {
            innerState = InnerState.stop;
        }

        public override void Exit(SeraphimActor actor)
        {
            if(bSoundPlaying)
            {
                actor.wwiseSoundCtrl.StopEventSoundFromId(soundID);
            }
        }

        public override void Process(SeraphimActor actor)
        {
            _horizontalSpeed = actor.unit.GetSpeed().x;
            if (Mathf.Approximately(_horizontalSpeed, 0))
            {
                innerState = InnerState.stop;
                if(bSoundPlaying)
                {
                    actor.wwiseSoundCtrl.StopEventSoundFromId(soundID);
                    bSoundPlaying = false;
                }
            }
            else
            {
                if (!bSoundPlaying)
                {
                    //Assert.IsNotNull(WwiseSoundManager.instance);
                    soundID = actor.wwiseSoundCtrl.PlayEventSound("Seraphim_FS");
                    bSoundPlaying = true;
                }
                if (_horizontalSpeed > 0)
                    innerState = InnerState.right;
                else if (_horizontalSpeed < 0)
                    innerState = InnerState.left;
            }
        }
        public override bool Transition(SeraphimActor actor, TransitionCondition condition)
        {
            switch (condition)
            {
                case TransitionCondition.RightMove:
                    if (innerState == InnerState.left)
                        actor.unit.Idle();
                    actor.flipValue = true;
                    actor.animCtrl.SetFlip(actor.flipValue);
                    actor.animCtrl.PlayAni(AniState.Move);
                    actor.unit.Move(1);
                    return true;
                case TransitionCondition.LeftMove:
                    if (innerState == InnerState.right)
                        actor.unit.Idle();
                    actor.flipValue = false;
                    actor.animCtrl.SetFlip(actor.flipValue);
                    actor.animCtrl.PlayAni(AniState.Move);
                    actor.unit.Move(-1);
                    return true;
                case TransitionCondition.StopMove:
                    actor.animCtrl.PlayAni(AniState.Idle);
                    actor.unit.MoveStop();
                    return true;
                case TransitionCondition.Idle:
                    actor.unit.Idle();
                    actor.ChangeState(actor.idle);
                    return true;
                case TransitionCondition.Hit:
                    actor.ChangeState(actor.hit);
                    return true;
                case TransitionCondition.Attack:
                    actor.ChangeState(actor.attack);
                    return true;
                case TransitionCondition.Jump:
                    actor.unit.Jump(10);
                    return true;
                case TransitionCondition.BackStepLeft:
                case TransitionCondition.BackStepRight:
                    actor.ChangeState(actor.backStep);
                    return actor.Transition(condition);
            }
            return false;
        }
    }
    //=====================================================================
    public class BackStep : SPState
    {
        private bool processed = false;

        public override void Enter(SeraphimActor actor)
        {
            actor.animCtrl.PlayAni(AniState.Backstep);
            processed = false;
        }

        public override void Exit(SeraphimActor actor)
        {
            actor.ActionEnd.Invoke();
        }

        public override void Process(SeraphimActor actor)
        {
            if (actor.unit.IsOnGround)
                actor.ChangeState(actor.idle);
        }

        public override bool Transition(SeraphimActor actor, TransitionCondition condition)
        {
            if (processed)
                return false;
            switch (condition)
            {
                case TransitionCondition.BackStepLeft:

                    actor.unit.BackStep(new Vector2(-actor.BackStepPower.x, actor.BackStepPower.y));

                    processed = true;
                    return true;
                case TransitionCondition.BackStepRight:

                    actor.unit.BackStep(actor.BackStepPower);

                    processed = true;
                    return true;
            }

            return false;
        }
    }
    //=====================================================================
    public class Hit : SPState
    {
        private float timeCheck;
        private bool firstHitAnim = true;

        public override void Enter(SeraphimActor actor)
        {
            if (firstHitAnim)
                actor.animCtrl.PlayAni(AniState.Hit);
            else
                actor.animCtrl.PlayAni(AniState.Hit2);
            firstHitAnim = !firstHitAnim;

            timeCheck = 1.0f;
        }

        public override void Exit(SeraphimActor actor)
        {
        }

        public override void Process(SeraphimActor actor)
        {
            //timeCheck += GameManager.instance.timeMng.customTimeWhere???
            timeCheck -= GameManager.instance.TimeMng.DeltaTime;

            if (timeCheck < 0 && actor.unit.IsOnGround)
            {
                actor.ChangeState(actor.idle);
            }
        }

        public override bool Transition(SeraphimActor actor, TransitionCondition condition)
        {
            switch (condition)
            {
                case TransitionCondition.Hit:
                    if (firstHitAnim)
                        actor.animCtrl.PlayAni(AniState.Hit);
                    else
                        actor.animCtrl.PlayAni(AniState.Hit2);
                    firstHitAnim = !firstHitAnim;
                    timeCheck = 1.0f;
                    return true;
            }
            return false;
        }
    }
    //=====================================================================
    public class Die : SPState
    {
        private float timeCheck;

        public override void Enter(SeraphimActor actor)
        {
            actor.animCtrl.PlayAni(AniState.Die);
            timeCheck = 0.5f;
            actor.unit.TurnOnInvincibility();
        }

        public override void Exit(SeraphimActor actor)
        {
            actor.unit.HandleDeath();

            actor.UnregistEventForGameManager();
        }

        public override void Process(SeraphimActor actor)
        {
            //timeCheck += GameManager.instance.timeMng.customTimeWhere???
            timeCheck -= GameManager.instance.TimeMng.DeltaTime;

            if (timeCheck < 0)
            {
                actor.ChangeState(SeraphimActor.nullState);
            }
        }

        public override bool Transition(SeraphimActor actor, TransitionCondition condition)
        {
            return false;
        }
    }
    //=====================================================================
    public class Attack : SPState
    {
        private float timeCheck;
        private bool isAttacked;
        public override void Enter(SeraphimActor actor)
        {
            actor.animCtrl.PlayAni(AniState.AttackReady);
            actor.unit.Idle();
            Assert.IsNotNull(WwiseSoundManager.instance);
            WwiseSoundManager.instance.PlayEventSound("Seraphim_AR");
            timeCheck = actor.unit.AttackEnterDelay;
            isAttacked = false;
            actor.attackAnimEnd = false;
        }

        public override void Exit(SeraphimActor actor)
        {
            actor.ActionEnd.Invoke();
        }

        public override void Process(SeraphimActor actor)
        {
            //timeCheck += GameManager.instance.timeMng.customTimeWhere???
            timeCheck -= GameManager.instance.TimeMng.DeltaTime;

            if(actor.attackAnimEnd)
            {
                actor.animCtrl.PlayAni(AniState.Idle);
                actor.attackAnimEnd = false;
            }

            if (timeCheck < 0)
            {
                if (!isAttacked)
                {
                    actor.animCtrl.PlayAni(AniState.Attack);
                    actor.unit.Attack();
                    actor.fireFX.gameObject.SetActive(true);
                    actor.fireFX.SetFlip(actor.flipValue);
                    Assert.IsNotNull(WwiseSoundManager.instance);
                    actor.wwiseSoundCtrl.PlayEventSound("Seraphim_Shot");
                    isAttacked = true;
                    timeCheck = actor.unit.AttackEndDelay;
                }
                else
                {
                    actor.ChangeState(actor.idle);
                }
            }
        }

        public override bool Transition(SeraphimActor actor, TransitionCondition condition)
        {
            switch (condition)
            {
                case TransitionCondition.Hit:
                    actor.ChangeState(actor.hit);
                    return true;
            }

            return false;
        }
    }
    //=====================================================================
    public class Null : SPState
    {
        public override void Enter(SeraphimActor actor)
        {
        }

        public override void Exit(SeraphimActor actor)
        {
        }

        public override void Process(SeraphimActor actor)
        {
        }

        public override bool Transition(SeraphimActor actor, TransitionCondition condition)
        {
            return false;
        }
    }
    //=====================================================================

}
