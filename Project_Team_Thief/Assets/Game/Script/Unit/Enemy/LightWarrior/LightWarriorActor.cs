using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using LightWarrior;

public class LightWarriorActor : MonoBehaviour, IActor
{
    public LightWarriorUnit unit;
    public AnimationCtrl animCtrl;
    LWState _curState;

    public Idle idle = new Idle();
    public Move move = new Move();
    public Hit hit = new Hit();
    public Die die = new Die();
    public Attack attack = new Attack();
    public static Null nullState = new Null();

    private void Awake()
    {
        unit = GetComponentInParent<LightWarriorUnit>();
        Assert.IsNotNull(unit);
        unit.hitEvent.AddListener(HitTransition);
        unit.dieEvent.AddListener(DieTransition);
    }

    private void Start()
    {
        _curState = idle;
        _curState.Enter(this);

        RegistEventForGameManager();
    }

    public void RegistEventForGameManager()
    {
        var v = GameManager.instance.timeMng;
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
        var v = GameManager.instance.timeMng;
        if (v)
        {
            v.startBulletTimeEvent -= TimeScaleChangeEnterCallback;
            v.endBulletTimeEvent -= TimeScaleChangeExitCallback;
            v.startHitstopEvent -= TimeScaleChangeEnterCallback;
            v.endHitstopEvent -= TimeScaleChangeExitCallback;
        }
    }

    private void Update()
    {
        _curState.Process(this);
    }

    public bool Transition(TransitionCondition condition, object param = null)
    {
        // common change, this transition can occur in any state
        switch (condition)
        {
            case TransitionCondition.Die:
                ChangeState(die);
                return true;
            case TransitionCondition.ForceKill:
                ChangeState(die);
                return true;
        }
            
        return _curState.Transition(this, condition);
    }

    public void ChangeState(LWState newState)
    {
        if (_curState == newState && _curState != hit)
            return;
        _curState.Exit(this);
        _curState = newState;
        _curState.Enter(this);
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
    
    // 피격, 사망 이벤트는 간단하게 트랜지션을 발동시켜주는 것으로 구현하였다.
    private void HitTransition() { Transition(TransitionCondition.Hit); }
    private void DieTransition() { Transition(TransitionCondition.Die); }
}

namespace LightWarrior
{
    public abstract class LWState
    {
        public abstract void Enter(LightWarriorActor actor);
        public abstract void Process(LightWarriorActor actor);
        public abstract void Exit(LightWarriorActor actor);
        public abstract bool Transition(LightWarriorActor actor, TransitionCondition condition);
    }

    //=====================================================================
    public class Null : LWState
    {
        public override void Enter(LightWarriorActor actor)
        {
        }

        public override void Exit(LightWarriorActor actor)
        {
        }

        public override void Process(LightWarriorActor actor)
        {
        }

        public override bool Transition(LightWarriorActor actor, TransitionCondition condition)
        {
            return false;
        }
    }
    //=====================================================================
    public class Idle : LWState
    {
        public override void Enter(LightWarriorActor actor)
        {
            actor.animCtrl.PlayAni(AniState.Idle);
        }

        public override void Exit(LightWarriorActor actor)
        {
        }

        public override void Process(LightWarriorActor actor)
        {
        }

        public override bool Transition(LightWarriorActor actor, TransitionCondition condition)
        {
            switch(condition)
            {
                case TransitionCondition.RightMove:
                case TransitionCondition.LeftMove:
                    actor.ChangeState(actor.move);
                    return actor.Transition(condition);
                case TransitionCondition.Hit:
                    actor.ChangeState(actor.hit);
                    return true;
                case TransitionCondition.Attack:
                    actor.ChangeState(actor.attack);
                    return true;
            }

            return false;
        }
    }

    //=====================================================================
    public class Move : LWState
    {
        private enum InnerState
        {
            right,left,stop
        }
        private float _horizontalSpeed;
        private InnerState innerState;

        public override void Enter(LightWarriorActor actor)
        {
            innerState = InnerState.stop;
        }

        public override void Exit(LightWarriorActor actor)
        {
        }

        public override void Process(LightWarriorActor actor)
        {
            _horizontalSpeed = actor.unit.GetSpeed().x;
            if (Mathf.Approximately(_horizontalSpeed, 0))
                innerState = InnerState.stop;
            else if (_horizontalSpeed > 0)
                innerState = InnerState.right;
            else if (_horizontalSpeed < 0)
                innerState = InnerState.left;
        }

        public override bool Transition(LightWarriorActor actor, TransitionCondition condition)
        {
            switch (condition)
            {
                case TransitionCondition.RightMove:
                    if (innerState == InnerState.left)
                        actor.unit.Idle();
                    actor.animCtrl.SetFlip(true);
                    actor.animCtrl.PlayAni(AniState.Move);
                    actor.unit.Move(1);
                    return true;
                case TransitionCondition.LeftMove:
                    if (innerState == InnerState.right)
                        actor.unit.Idle();
                    actor.animCtrl.SetFlip(false);
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
            }
            return false;
        }
    }

    //=====================================================================
    public class Hit : LWState
    {
        private float timeCheck;

        public override void Enter(LightWarriorActor actor)
        {
            actor.animCtrl.PlayAni(AniState.Hit);
            timeCheck = 1.0f;
        }

        public override void Exit(LightWarriorActor actor)
        {
        }

        public override void Process(LightWarriorActor actor)
        {
            //timeCheck += GameManager.instance.timeMng.customTimeWhere???
            timeCheck -= Time.deltaTime;

            if(timeCheck < 0)
            {
                actor.ChangeState(actor.idle);
            }
        }

        public override bool Transition(LightWarriorActor actor, TransitionCondition condition)
        {
            switch (condition)
            {
                case TransitionCondition.Hit:
                    timeCheck = 1.0f;
                    return true;
            }
            return false;
        }
    }

    //=====================================================================
    public class Die : LWState
    {
        private float timeCheck;

        public override void Enter(LightWarriorActor actor)
        {
            actor.animCtrl.PlayAni(AniState.Die);
            timeCheck = 0.5f;
        }

        public override void Exit(LightWarriorActor actor)
        {
            actor.unit.HandleDeath();

            actor.UnregistEventForGameManager();
        }

        public override void Process(LightWarriorActor actor)
        {
            //timeCheck += GameManager.instance.timeMng.customTimeWhere???
            timeCheck -= Time.deltaTime;

            if (timeCheck < 0)
            {
                actor.ChangeState(LightWarriorActor.nullState);
            }
        }

        public override bool Transition(LightWarriorActor actor, TransitionCondition condition)
        {
            return false;
        }
    }

    //=====================================================================s
    public class Attack : LWState
    {
        private float timeCheck;
        private bool isAttacked;
        public override void Enter(LightWarriorActor actor)
        {
            actor.animCtrl.PlayAni(AniState.AttackReady);
            actor.unit.Idle();
            timeCheck = 0.5f;
            isAttacked = false;
        }

        public override void Exit(LightWarriorActor actor)
        {

        }

        public override void Process(LightWarriorActor actor)
        {
            //timeCheck += GameManager.instance.timeMng.customTimeWhere???
            timeCheck -= Time.deltaTime;

            if (timeCheck < 0)
            {
                if (!isAttacked)
                {
                    actor.animCtrl.PlayAni(AniState.Attack);
                    actor.unit.Attack();
                    isAttacked = true;
                    timeCheck = 0.5f;
                }
                else
                {
                    actor.ChangeState(actor.idle);
                }
            }
        }

        public override bool Transition(LightWarriorActor actor, TransitionCondition condition)
        {
            switch(condition)
            {
                case TransitionCondition.Hit:
                    actor.ChangeState(actor.hit);
                    return true;
            }

            return false;
        }
    }
}