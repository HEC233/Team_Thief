using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public abstract class AttackBase : MonoBehaviour
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
    protected SFXClip _sfxClip;
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
    
    protected CinemachineBlendDefinition _cinemachineBlendDefinition;
    
    // [SerializeField]
    // protected float _camerShakeTime;
    // [SerializeField]
    // protected float _cameraShakeAmplitudeGain;
    // [SerializeField] 
    // protected float _cameraShakeFrequencyGain;
    [SerializeField]
    protected LayerMask _hitLayerMask;
    // 재생 될 SFX에 대한 Enum 값을 선택하도록 하는게 좋을 듯.

    protected Damage _damage;
    protected float _minAttackDamage;
    protected float _maxAttackDamage;

    [Header("Objects")]
    [SerializeField]
    protected GameObject _fxGO;
    [SerializeField]
    protected FlashCtrl _flashGO;

    public UnityAction<string> OnEnemyHitEvent;
    

    public abstract void Flash();

    public abstract void HitStop();

    public abstract void BulltTime();

    public abstract void PlayFx();

    public abstract void PlaySfx();

    public abstract void AttackDamage();

    public abstract void SetDamage(in Damage damage);

    public abstract void CameraShake();

    public abstract void ZoomIn();

    public abstract CinemachineBlendDefinition ZoomOutBlendDefinition();

    public abstract void UnBind();
}
