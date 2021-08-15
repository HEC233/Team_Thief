using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class AttackBase : MonoBehaviour
{
    [Header ("Attack Option")]
    [SerializeField, Tooltip("크리티컬이 발생하는가?")]
    protected bool _isAbleCritical;

    [SerializeField, Tooltip("방향 전환이 일어나는가?")]
    protected bool _isChangeDir;

    [SerializeField, Tooltip("플래쉬 효과가 발생하는가?")]
    protected bool _isAbleFlash;

    [SerializeField, Tooltip("히트 스탑이 발생하는가?")]
    protected bool _isAbleHitStop;

    [SerializeField, Tooltip("불릿 타임이 발생하는가?")]
    protected bool _isAbleBulltTime;

    [SerializeField, Tooltip("카메라 쉐이크가 발생하는가?")]
    protected bool _isAbleCameraShake;

    [SerializeField, Tooltip("이펙트가 표시되는가?")]
    protected bool _isDisplyFx;

    [SerializeField, Tooltip("효과음이 재생되는가?")]
    protected bool _isPlaySFX;
    
    [SerializeField, Tooltip("효과음이 항상 재생되는가?")]
    protected bool _isAlwaysPlaySFX;

    [SerializeField, Tooltip("줌 인이 일어나는가?")]
    protected bool _isZoomIn;
    
    [Header ("Values")]
    [SerializeField]
    protected float _hitStopTime;
    [SerializeField]
    protected float _bulltTime;
    [SerializeField]
    protected float _bulletTimeScale;
    [SerializeField]
    protected float _flashTime;
    [SerializeField]
    protected float _criticalPercentage;
    [SerializeField]
    protected string _sfxSoundName;
    protected uint _sfxSoundId;
    [SerializeField] 
    protected AnimationCurve _zoomInCurve;
    [SerializeField] 
    protected float _zoomInTime;
    [SerializeField] 
    protected float _zoomInSize;
    [SerializeField] 
    protected AnimationCurve _zoomOutCurve;
    [SerializeField]
    protected float _zoomOutTime;
    [SerializeField] 
    protected string _fxName;

    protected ContactFilter2D _contactFilter2D;
    protected List<Collider2D> result = new List<Collider2D>();
    
    protected CinemachineBlendDefinition _cinemachineBlendDefinition;
    protected SignalSourceAsset _signalSourceAsset;

    protected bool _isEnter = false;
    protected bool _isInit = false;

    [SerializeField]
    protected LayerMask _hitLayerMask;
    // 재생 될 SFX에 대한 Enum 값을 선택하도록 하는게 좋을 듯.

    protected Damage _damage;
    protected float _minAttackDamage;
    protected float _maxAttackDamage;

    [Header("Objects")]
    [SerializeField]
    protected FlashCtrl _flashGO;
    [SerializeField] 
    protected BoxCollider2D _attackCollider2D;
    [SerializeField]
    protected CinemachineImpulseSource _cinemachineImpulseSource;

    
    public UnityAction OnEnemyHitEvent;

    public virtual void Progress()
    {
        if (_isInit == false)
        {
            Debug.LogError("Is Not Init Attack Ctrl");
            return;
        }

        AttackAreaDetection();

        if (_isEnter == true)
        {
            AttackDamage();
            PlayFx();
            PlaySfx();
            HitStop();
            BulltTime();
            CameraShake();
            ZoomIn();
        }
        else if (_isAlwaysPlaySFX)
        {
            PlaySfx();
        }
        
        AttackEnd();
    }

    public virtual void ProgressTargetSelection(Collider2D target)
    {
        if (_isInit == false)
        {
            Debug.LogError("Is Not Init Attack Ctrl");
            return;
        }

        AttackDamage(target);
        PlayFx();
        PlaySfx();
        HitStop();
        BulltTime();
        CameraShake();
        ZoomIn();

        if (_isAlwaysPlaySFX)
        {
            PlaySfx();
        }

        AttackEnd();
    }

    public virtual void Init(Damage damage)
    {
        if (_attackCollider2D == null)
        {
            Debug.LogError("Is Not AttackCollider2D");
        }

        _contactFilter2D.useTriggers = true;
        _contactFilter2D.useLayerMask = true;
        _contactFilter2D.layerMask = _hitLayerMask;
        
        _damage = damage;
        
        Bind();
        
        _isInit = true;
    }

    public virtual void Init(Damage damage, SignalSourceAsset signalSourceAsset)
    {
        Init(damage);

        if (_cinemachineImpulseSource == null)
        {
            Debug.LogError("Is CinemachineImpulseSource Null");
        }
        
        _signalSourceAsset = signalSourceAsset;
        _cinemachineImpulseSource.m_ImpulseDefinition.m_RawSignal = _signalSourceAsset;
    }

    public virtual void Init(Damage damage, SkillDataBase skillData)
    {
        Init(damage);
    }
    
    protected void Bind()
    {
        if (_isZoomIn == true)
            GameManager.instance.cameraMng.OnZoomInEndEvent += ZoomOut;
    }

    protected void UnBind()
    {
        if (_isZoomIn == true)
            GameManager.instance.cameraMng.OnZoomInEndEvent -= ZoomOut;
    }

    protected void Flash()
    {
        if (_isAbleFlash == false)
            return;
    }

    private void HitStop()
    {
        if (_isAbleHitStop == false)
            return;

        GameManager.instance.timeMng.HitStop(_hitStopTime);
    }

    private void BulltTime()
    {
        if (_isAbleBulltTime == false)
            return;
        
        GameManager.instance.timeMng.BulletTime(_bulletTimeScale, _bulltTime);
    }

    protected virtual void PlayFx()
    {
        if (_isDisplyFx == false)
            return;
        if (_fxName == String.Empty)
            return;

        GameManager.instance.FX.Play(_fxName, _damage.hitPosition);
    }

    protected virtual void PlaySfx()
    {
        if (_isPlaySFX == false)
            return;
        if (_sfxSoundName == String.Empty)
            return;
        
        _sfxSoundId = WwiseSoundManager.instance.PlayEventSound(_sfxSoundName);
    }

    protected void AttackAreaDetection()
    {
        _isEnter = false;
        
        // 다음 프레임에 활성화가 되기 때문에 바로 끄면 체크 X
        if (_attackCollider2D.IsTouchingLayers(_hitLayerMask))
        {
            _attackCollider2D.OverlapCollider(_contactFilter2D, result);
            foreach (var item in result)
            {
                if (item.gameObject.CompareTag("Player"))
                    continue;
         
                if (item.gameObject.CompareTag("Enemy"))
                {
                    _isEnter = true;
                    break;
                }
            }
        }
        
    }

    private void AttackDamage()
    {
        foreach (var item in result)
        {
            if (item.gameObject.CompareTag("Player"))
                continue;

            if (item.gameObject.CompareTag("Enemy"))
            {
                if (item.GetComponentInParent<Unit>().GetInvincibility() == true)
                {
                    continue;
                }

                //============== 고재협이 편집함 ======================
                _damage.hitPosition = item.ClosestPoint(_attackCollider2D.bounds.center);
                //=====================================================
                item.GetComponentInParent<Unit>().HandleHit(_damage);
                // 리팩토링 할 때 참고할 점
                // 이렇게 스트링으로 박지말고 각 공격을 관리하는 친구가 스킬에 관한 정보를 가지고 있자.
                OnEnemyHitEvent?.Invoke();
            }
        }
    }

    private void AttackDamage(Collider2D target)
    {
        //============== 고재협이 편집함 ======================
        _damage.hitPosition = target.ClosestPoint(_attackCollider2D.bounds.center);
        //=====================================================
        target.GetComponentInParent<Unit>().HandleHit(_damage);
        
        OnEnemyHitEvent?.Invoke();
    }

    private void CameraShake()
    {
        if (_isAbleCameraShake == false)
            return;

        if (_cinemachineImpulseSource.m_ImpulseDefinition.m_RawSignal == null)
        {
            Debug.LogError("_cinemachineImpulseSource RawSignal is Null!");
        }
        
        _cinemachineImpulseSource.GenerateImpulse();
    }

    private void ZoomIn()
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
    }
    
    private CinemachineBlendDefinition ZoomOutBlendDefinition()
    {
        _cinemachineBlendDefinition.m_Style = CinemachineBlendDefinition.Style.Custom;
        _cinemachineBlendDefinition.m_CustomCurve = _zoomOutCurve;
        _cinemachineBlendDefinition.m_Time = _zoomOutTime;

        return _cinemachineBlendDefinition;
    }

    public virtual void AttackEnd()
    {
        ZoomOut();
        
        UnBind();
    }
    
}
