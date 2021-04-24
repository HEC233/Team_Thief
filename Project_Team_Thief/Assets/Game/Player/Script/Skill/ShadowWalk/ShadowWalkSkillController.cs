using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ShadowWalkSkillController : SkillControllerBase
{
    public ShadowWalkSkillController(GameSkillObject skillObject, SkillDataBase data, Unit unit) : base(skillObject, data, unit) { }

    private ShadowWalkSkillData _shadowWalkSkillData;

    private PlayerUnit _unit;

    public override void Invoke()
    {
        _shadowWalkSkillData = SkillData as ShadowWalkSkillData;
        _unit = Unit as PlayerUnit;
        var shadowTransform = _unit.shadowWalkShadow.transform;
        _unit.Rigidbody2D.MovePosition(shadowTransform.position);
        
        _unit.shadowWalkShadow.ChangeControlState(_shadowWalkSkillData.ControlTime);
        CameraShake();
        CrateShadowLump();
        GameManager.instance.FX.Play("UlimFx", shadowTransform.position);
        OnEndSkill?.Invoke();
    }

    private void CameraShake()
    {
        var cinemachineImpulseSource = SkillObject.gameObject.AddComponent<CinemachineImpulseSource>();
        cinemachineImpulseSource.m_ImpulseDefinition.m_RawSignal = _shadowWalkSkillData.CinemachineImpulseSource;
        cinemachineImpulseSource.GenerateImpulse();
        SkillObject.DestroyComponent(cinemachineImpulseSource);
    }

    private void CrateShadowLump()
    {
        for (int i = 0; i < _shadowWalkSkillData.ShadowLumpAmount; i++)
        {
            var shadowLump = 
                GameObject.Instantiate(_shadowWalkSkillData.ShadowLumpGameObject);
            
            shadowLump.transform.position = _unit.shadowWalkShadow.transform.position;
            shadowLump.GetComponent<ShadowLumpUnit>().Init(_shadowWalkSkillData.ControlTime);
        }
    }
}
