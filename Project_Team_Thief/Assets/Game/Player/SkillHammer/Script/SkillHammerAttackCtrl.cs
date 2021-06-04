using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Assert = UnityEngine.Assertions.Assert;

public class SkillHammerAttackCtrl : AttackBase
{
  [SerializeField] 
    private BoxCollider2D _basicAttackCollider2D;
    [SerializeField]
    private CinemachineImpulseSource _cinemachineImpulseSource;
    private ContactFilter2D _contactFilter2D;
    private List<Collider2D> result = new List<Collider2D>();
    private bool _isInit = false;
    private bool _isEnter = false;
    public bool alwaysEnter = false;
    public SignalSourceAsset signalSourceAsset;
    
    private void OnEnable()
    {
        if(_basicAttackCollider2D == null)
            Assert.IsNotNull("_basicAttackCollider Null");
        
        if(_isInit == false)
            Init();
            
        //Progress();
    }

    private void Init()
    {
        _isInit = true;
        
        _contactFilter2D.useTriggers = true;
        _contactFilter2D.useLayerMask = true;
        _contactFilter2D.layerMask = _hitLayerMask;

        _cinemachineImpulseSource.m_ImpulseDefinition.m_RawSignal = signalSourceAsset;
    }

    public void Progress()
    {
        if (_cinemachineImpulseSource.m_ImpulseDefinition.m_RawSignal == null)
        {
            _cinemachineImpulseSource.m_ImpulseDefinition.m_RawSignal = signalSourceAsset;
        }
        
        Bind();

        AttackDamage();

        if (_isEnter == true || alwaysEnter == true)
        {
            PlayFx();
            HitStop();
            CameraShake();
            PlaySfx();
            ZoomIn();
        }
    }

    private void EnemyHitFx()
    {
        WwiseSoundManager.instance.PlayEventSound("PC_hammaer_Smash");
        GameManager.instance.FX.Play("HitHammerFx", _damage.hitPosition);
    }

    public override void Flash()
    {
        if (_isAbleFlash == false)
            return;
    }

    public override void HitStop()
    {
        if (_isAbleHitStop == false)
            return;

        GameManager.instance.timeMng.HitStop(_hitStopTime);
    }

    public override void BulltTime()
    {
        if (_isAbleBulltTime == false)
            return;
        
        GameManager.instance.timeMng.BulletTime(_bulletTimeScale, _bulltTime);
    }

    public override void PlayFx()
    {
        if (_isDisplyFx == false)
            return;

        EnemyHitFx();
    }

    public override void PlaySfx()
    {
        if (_isPlaySFX == false)
            return;

        //WwiseSoundManager.instance.PlayEventSound("PC_HIT_hammer");
    }
    
    public override void CameraShake()
    {
        if (_isAbleCameraShake == false)
            return;
        _cinemachineImpulseSource.GenerateImpulse();
    }
    
    public override void ZoomIn()
    {
        if (_isZoomIn == false)
        {
            return;
        }
        
        _cinemachineBlendDefinition.m_Style = CinemachineBlendDefinition.Style.Custom;
        _cinemachineBlendDefinition.m_CustomCurve = _zoomInCurve;
        _cinemachineBlendDefinition.m_Time = _zoomInTime;

        GameManager.instance.cameraMng.ZoomIn(_cinemachineBlendDefinition, _zoomInSize);
    }

    private void ZoomOut()
    {
        GameManager.instance.cameraMng.ZoomOut(ZoomOutBlendDefinition());
        UnBind();
    }

    public override CinemachineBlendDefinition ZoomOutBlendDefinition()
    {
        _cinemachineBlendDefinition.m_Style = CinemachineBlendDefinition.Style.Custom;
        _cinemachineBlendDefinition.m_CustomCurve = _zoomOutCurve;
        _cinemachineBlendDefinition.m_Time = _zoomOutTime;

        return _cinemachineBlendDefinition;
    }

    public override void UnBind()
    {
        GameManager.instance.cameraMng.OnZoomInEndEvent -= ZoomOut;
    }

    private void Bind()
    {
        GameManager.instance.cameraMng.OnZoomInEndEvent += ZoomOut;
    }

    public override void AttackDamage()
    {
        _isEnter = false;
        
        // 다음 프레임에 활성화가 되기 때문에 바로 끄면 체크 X
        if (_basicAttackCollider2D.IsTouchingLayers(_hitLayerMask))
        {
            _basicAttackCollider2D.OverlapCollider(_contactFilter2D, result);
            foreach (var item in result)
            {
                if (item.gameObject.CompareTag("Player"))
                    continue;

                if (item.gameObject.CompareTag("Enemy"))
                {
                    //============== 고재협이 편집함 ======================
                    _damage.hitPosition = item.ClosestPoint(_basicAttackCollider2D.bounds.center);
                    //=====================================================
                    _isEnter = true;
                    item.GetComponentInParent<Unit>().HandleHit(_damage);
                    OnEnemyHitEvent?.Invoke("Skill3Hammer");
                }
            }
        }
    }

    public override void SetDamage(in Damage damage)
    {
        _damage = damage;
    }
}
