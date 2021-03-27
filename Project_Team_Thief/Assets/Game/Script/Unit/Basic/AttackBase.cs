using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackBase : MonoBehaviour
{
    [Header ("Attack Option")]
    [SerializeField, Tooltip("크리티컬이 발생하는가?")]
    protected bool _isAbleCritical;

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

    [Header ("Values")]
    [SerializeField]
    protected float _hitStopTime;
    [SerializeField]
    protected float _bulltTime;
    [SerializeField]
    protected float _bulletTimeScale;
    [SerializeField]
    protected float _shakeTime;
    [SerializeField]
    protected float _flashTime;
    [SerializeField]
    protected float _criticalPercentage;
    [SerializeField]
    protected float _camerShakeTime;
    [SerializeField]
    protected float _cameraShakePower;
    [SerializeField]
    protected LayerMask _hitLayerMask;

    protected Damage _damage;
    protected float _minAttackDamage;
    protected float _maxAttackDamage;

    [Header("Objects")]
    [SerializeField]
    protected GameObject _fxGO;
    [SerializeField]
    protected FlashCtrl _flashGO;
    

    public abstract void Flash();

    public abstract void HitStop();

    public abstract void BulltTime();

    public abstract void PlayFx();

    public abstract void PlaySfx();

    public abstract void AttackDamage();

    public abstract void SetDamage(float min, float max);

    public abstract void CameraShake();
}
