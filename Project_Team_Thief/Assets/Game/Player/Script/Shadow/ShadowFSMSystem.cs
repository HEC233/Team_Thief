using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ShadowFSMStateBase : IFSMStateBase
{
    private ShadowFSMSystem _systemMgr = null;

    public ShadowFSMSystem SystemMgr => _systemMgr;

    public ShadowFSMStateBase(ShadowFSMSystem system)
    {
        _systemMgr = system;
    }


    public abstract void StartState();

    public abstract void Update();

    public abstract void EndState();
    public abstract bool Transition(TransitionCondition condition);

}

public class ShadowFSMSystem : FSMSystem<TransitionCondition, ShadowFSMStateBase>, IActor
{
    private ShadowUnit _shadowUnit;
    public ShadowUnit ShadowUnit => _shadowUnit;
    
    [SerializeField] 
    private AnimationCtrl _animationCtrl;
    public AnimationCtrl AnimationCtrl => _animationCtrl;

    [SerializeField]
    private FxCtrl _fxCtrl;

    public AniState changeAniState;
    

    private void Start()
    {
        
    }
    
    private void Init()
    {
        changeAniState = AniState.Idle;
        
        Bind();
    }

    private void Bind()
    {
        
    }
    
    private class DefaultState : ShadowFSMStateBase
    {
        public DefaultState(ShadowFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            PlayDefaultAni(SystemMgr.changeAniState);
            SystemMgr.changeAniState = AniState.Idle;
        }

        public override void Update()
        {
        }

        public override void EndState()
        {
        }

        private void PlayDefaultAni(AniState aniState)
        {
            SystemMgr.AnimationCtrl.PlayAni(aniState);
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Idle)
                PlayDefaultAni(AniState.Idle);
            if (condition == TransitionCondition.Move)
                PlayDefaultAni(AniState.Move);
            if (condition == TransitionCondition.RunningInertia)
                PlayDefaultAni(AniState.RunningInertia);
            if (condition == TransitionCondition.Jump)
                PlayDefaultAni(AniState.Jump);
            if (condition == TransitionCondition.Falling)
                PlayDefaultAni(AniState.Fall);
            if (condition == TransitionCondition.Dash)
                PlayDefaultAni(AniState.Dash);
            if (condition == TransitionCondition.Wallslideing)
                PlayDefaultAni(AniState.Wallslideing);

            if (condition == TransitionCondition.Off)
                return true;
  
            return false;
        }
    }
    
    private class OffState : ShadowFSMStateBase
    {
        public OffState(ShadowFSMSystem system) : base(system)
        {
        }

        public override void StartState()
        {
            SystemMgr.AnimationCtrl.SetOnOffSpriteRenderer(false);
        }

        public override void Update()
        {
        }

        public override void EndState()
        {
            SystemMgr.AnimationCtrl.SetOnOffSpriteRenderer(true);
        }

        public override bool Transition(TransitionCondition condition)
        {
            if (condition == TransitionCondition.Default)
                return true;

            if (condition == TransitionCondition.Idle)
                SystemMgr.changeAniState = AniState.Idle;

            if (condition == TransitionCondition.Move)
                SystemMgr.changeAniState = AniState.Move;

            if (condition == TransitionCondition.RunningInertia)
                SystemMgr.changeAniState = AniState.RunningInertia;

            if (condition == TransitionCondition.Jump)
                SystemMgr.changeAniState = AniState.Jump;

            if (condition == TransitionCondition.Falling)
                SystemMgr.changeAniState = AniState.Fall;

            if (condition == TransitionCondition.Dash)
                SystemMgr.changeAniState = AniState.Dash;

            if (condition == TransitionCondition.Wallslideing)
                SystemMgr.changeAniState = AniState.Wallslideing;
            
         
            SystemMgr.Transition(TransitionCondition.Default);
            return false;
        }
    }
    
    protected override void RegisterState()
    {
        AddState(TransitionCondition.Default, new DefaultState(this));
        AddState(TransitionCondition.Off, new OffState(this));
    }

    public bool Transition(TransitionCondition condition, object param = null)
    {
        //if (GetContainsKey(condition) == false)
        //    return false;

        if (CheckStateChangeAbleCondition(condition) == false)
            return false;
        
        if (GameManager.instance.timeMng.IsHitStop == true)
            return false;
        
        if (CurrState == condition)
            return false;
        
        ChangeState(condition);
        return true;
    }
    
    public Unit GetUnit()
    {
        return ShadowUnit;
    }
}
