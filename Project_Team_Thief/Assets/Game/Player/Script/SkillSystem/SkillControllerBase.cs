using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public abstract class SkillControllerBase
{
    private GameSkillObject _skillObject = null;
    public GameSkillObject SkillObject => _skillObject;

    private SkillDataBase _skillData = null;
    public SkillDataBase SkillData => _skillData;

    private CinemachineImpulseSource _skillObjectCinemachineImpulseSource;
    
    private Unit _unit;

    public Unit Unit => _unit;

    protected UnityAction OnEndSkillAction = null;   // todo Event vs evnet UnityAction?
    
    public event UnityAction OnEndSkillEvent
    {
        add => OnEndSkillAction += value;
        remove => OnEndSkillAction -= value;
    }
    
    protected UnityAction OnBulletTimeAction = null;
    public event UnityAction OnBulletTimeEvet
    {
        add => OnBulletTimeAction += value;
        remove => OnBulletTimeAction -= value;
    }

    protected UnityAction OnHitStopAction = null;
    
    public event UnityAction OnHitStopEvent
    {
        add => OnHitStopAction += value;
        remove => OnHitStopAction -= value;
    }

    protected UnityAction OnZoomInAction = null;
    
    public event UnityAction OnZoomInEvent
    {
        add => OnZoomInAction += value;
        remove => OnZoomInAction -= value;
    }

    protected UnityAction OnZoomOutAction = null;
    
    public event UnityAction OnZoomOutEvent
    {
        add => OnZoomOutAction += value;
        remove => OnZoomOutAction -= value;
    }

    public SkillControllerBase(GameSkillObject skillObject, SkillDataBase data, Unit unit)
    {
        _skillObject = skillObject;
        _skillData = data;
        _unit = unit;
    }

    public virtual void Invoke()
    {
        if(_skillData.SOActionValue == null)
            return;
        
        var playerUnit = GameManager.instance.PlayerActor.GetUnit() as PlayerUnit;
        
        if(playerUnit == null)
            return;

        playerUnit.ZoomInEvent += ZoomInEventCall;
        playerUnit.ZoomOutEvent += ZoomOutEventCall;
        playerUnit.OnBulletTimeEvent += BulletTimeEventCall;
        playerUnit.OnHitStopEvent += HitStopEventCall;
        playerUnit.OnFlashEvent += FlashEventCall;
        playerUnit.OnCameraShakeEvent += CameraShakeEventCall;
    }
    

    public virtual void Release()
    {
        if(_skillData.SOActionValue == null)
            return;
        
        var playerUnit = GameManager.instance.PlayerActor.GetUnit() as PlayerUnit;
        
        if(playerUnit == null)
            return;

        playerUnit.ZoomInEvent -= ZoomInEventCall;
        playerUnit.ZoomOutEvent -= ZoomOutEventCall;
        playerUnit.OnBulletTimeEvent -= BulletTimeEventCall;
        playerUnit.OnHitStopEvent -= HitStopEventCall;
        playerUnit.OnFlashEvent -= FlashEventCall;
        playerUnit.OnCameraShakeEvent -= CameraShakeEventCall;
        
        _skillData = null;
        _skillObject = null;
        OnEndSkillAction = null;
    }

    public virtual void ZoomInEventCall()
    {
        GameManager.instance.CameraMng.OnZoomInEndEvent += ZoomInBackEventCall;

        GameManager.instance.CameraMng.ZoomIn(_skillData.SOActionValue.ZoomInOutData.GetZoomInData(),
            _skillData.SOActionValue.ZoomInOutData.ZoomInSize);
    }

    public virtual void ZoomInBackEventCall()
    {
        GameManager.instance.CameraMng.OnZoomInEndEvent -= ZoomInBackEventCall;

        GameManager.instance.CameraMng.ZoomOut(_skillData.SOActionValue.ZoomInOutData.GetZoomOutData());
    }

    public virtual void ZoomOutEventCall()
    {
        GameManager.instance.CameraMng.OnZoomInEndEvent += ZoomOutBackEventCall;

        GameManager.instance.CameraMng.ZoomIn(_skillData.SOActionValue.ZoomOutInData.GetZoomInData(),
            _skillData.SOActionValue.ZoomOutInData.ZoomInSize);
    }

    public virtual void ZoomOutBackEventCall()
    {
        GameManager.instance.CameraMng.OnZoomInEndEvent -= ZoomOutBackEventCall;

        GameManager.instance.CameraMng.ZoomOut(_skillData.SOActionValue.ZoomOutInData.GetZoomOutData());
    }

    public virtual void BulletTimeEventCall()
    {
        GameManager.instance.TimeMng.BulletTime(_skillData.SOActionValue.BulletTimeAmount,
            _skillData.SOActionValue.BulletTimeScale);
    }

    public virtual void HitStopEventCall()
    {
        GameManager.instance.TimeMng.HitStop(_skillData.SOActionValue.HitstopTime);
    }

    public virtual void CameraShakeEventCall()
    {
        _skillObjectCinemachineImpulseSource = _skillObject.AddCinemachineImpulseComponent();
        _skillObjectCinemachineImpulseSource.m_ImpulseDefinition.m_RawSignal =
            _skillData.SOActionValue.CinemachineSignalSource;
        _skillObjectCinemachineImpulseSource.GenerateImpulse();
        _skillObject.DestroyComponent(_skillObjectCinemachineImpulseSource);
    }

    public virtual void FlashEventCall()
    {
        //todo 플래쉬 구현해야함
    }
}
