using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Dandelion;

public class DandelionActor : MonoBehaviour, IActor
{
    public DandelionUnit unit;
    public AnimationCtrl animCtrl;
    public WwiseSoundCtrl wwiseSoundCtrl;
    private DDState _curState;

    public Idle idle = new Idle();
    public Attack attack = new Attack();
    public Hit hit = new Hit();
    public Die die = new Die();
    public NULL nullState = new NULL();

    private void Awake()
    {
        unit = GetComponentInParent<DandelionUnit>();
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
    private void Update()
    {
        _curState.Process(this);
    }

    public bool Transition(TransitionCondition condition, object param = null)
    {
        bool returnValue = _curState.Transition(this, condition);
        if (!returnValue)
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
        }
        return returnValue;
    }
    public Unit GetUnit()
    {
        return unit;
    }
    public void ChangeState(DDState newState)
    {
        if (_curState == newState && _curState != hit)
            return;
        _curState.Exit(this);
        _curState = newState;
        _curState.Enter(this);
    }
    private void HitTransition() { Transition(TransitionCondition.Hit); }
    private void DieTransition() { Transition(TransitionCondition.Die); }
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
}

namespace Dandelion
{
    public abstract class DDState
    {
        public abstract void Enter(DandelionActor actor);
        public abstract void Process(DandelionActor actor);
        public abstract void Exit(DandelionActor actor);
        public abstract bool Transition(DandelionActor actor, TransitionCondition condition);
    }
    //=====================================================================
    public class NULL : DDState
    {
        public override void Enter(DandelionActor actor) {}
        public override void Exit(DandelionActor actor) {}
        public override void Process(DandelionActor actor) {}
        public override bool Transition(DandelionActor actor, TransitionCondition condition)
        {
            return false;
        }
    }
    //=====================================================================
    public class Idle : DDState
    {
        public override void Enter(DandelionActor actor)
        {
            actor.animCtrl.PlayAni(AniState.Idle);
        }

        public override void Exit(DandelionActor actor)
        {
        }

        public override void Process(DandelionActor actor)
        {
        }

        public override bool Transition(DandelionActor actor, TransitionCondition condition)
        {
            switch (condition)
            {
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
    public class Hit : DDState
    {
        private float timeCheck;
        private bool firstHitAnim = true;

        public override void Enter(DandelionActor actor)
        {
            if (firstHitAnim)
                actor.animCtrl.PlayAni(AniState.Hit);
            else
                actor.animCtrl.PlayAni(AniState.Hit2);
            firstHitAnim = !firstHitAnim;

            timeCheck = 1.0f;
        }

        public override void Exit(DandelionActor actor)
        {
        }

        public override void Process(DandelionActor actor)
        {
            timeCheck -= GameManager.instance.TimeMng.DeltaTime;

            if (timeCheck < 0 && actor.unit.IsOnGround)
            {
                actor.ChangeState(actor.idle);
            }
        }

        public override bool Transition(DandelionActor actor, TransitionCondition condition)
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
    public class Die : DDState
    {
        private float timeCheck;

        public override void Enter(DandelionActor actor)
        {
            actor.animCtrl.PlayAni(AniState.Die);
            timeCheck = 0.5f;
            actor.unit.TurnOnInvincibility();
        }

        public override void Exit(DandelionActor actor)
        {
            actor.unit.HandleDeath();

            actor.UnregistEventForGameManager();
        }

        public override void Process(DandelionActor actor)
        {
            timeCheck -= GameManager.instance.TimeMng.DeltaTime;

            if (timeCheck < 0)
            {
                actor.ChangeState(actor.nullState);
            }
        }

        public override bool Transition(DandelionActor actor, TransitionCondition condition)
        {
            return true;
        }
    }
    //=====================================================================
    public class Attack : DDState
    {
        private float timeCheck;
        private bool isAttacked;
        public override void Enter(DandelionActor actor)
        {
            actor.animCtrl.PlayAni(AniState.AttackReady);
            actor.unit.Idle();
            timeCheck = actor.unit.AttackEnterDelay;
            isAttacked = false;
        }

        public override void Exit(DandelionActor actor)
        {
        }

        public override void Process(DandelionActor actor)
        {
            timeCheck -= GameManager.instance.TimeMng.DeltaTime;

            if (timeCheck <= 0)
            {
                if (!isAttacked)
                {
                    actor.animCtrl.PlayAni(AniState.Attack);
                    actor.unit.Attack();
                    Assert.IsNotNull(WwiseSoundManager.instance);
                    actor.wwiseSoundCtrl.PlayEventSound("Dandelion_FA");
                    isAttacked = true;
                    timeCheck = actor.unit.AttackEndDelay;
                }
                else
                {
                    actor.ChangeState(actor.idle);
                }
            }
        }

        public override bool Transition(DandelionActor actor, TransitionCondition condition)
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

}
