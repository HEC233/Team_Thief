using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class SkillAxeAttackCtrl : AttackBase
{
    public event UnityAction OnEndSkillEvent;
    
    [SerializeField] 
    private BoxCollider2D _basicAttackCollider2D;
    [SerializeField]
    private Rigidbody2D _rigidbody2D;
    [SerializeField]
    private CinemachineImpulseSource _cinemachineImpulseSource;
    
    private SignalSourceAsset _cinemachineSignalSource;
    
    private ContactFilter2D _contactFilter2D;
    private List<Collider2D> result = new List<Collider2D>();
    private bool _isInit = false;
    private bool _isEnter = false;

    private float _movePositionX;
    private float _moveTime;
    private float _moveSpeed;
    private float _dir;
    
    public void Init(float movePosX, float moveTime, SignalSourceAsset cinemachineSignalSource, float dir)
    {
        _isInit = true;

        _movePositionX = movePosX;
        _moveTime = moveTime;
        _cinemachineSignalSource = cinemachineSignalSource;
        _dir = dir;

        _cinemachineImpulseSource.m_ImpulseDefinition.m_RawSignal = _cinemachineSignalSource;
        _moveSpeed = (1 / _moveTime) * _movePositionX;
        _damage.knockBack = new Vector2(_damage.knockBack.x * _dir, _damage.knockBack.y);
        
        _contactFilter2D.useTriggers = true;
        _contactFilter2D.useLayerMask = true;
        _contactFilter2D.layerMask = _hitLayerMask;

        StartCoroutine(AxeMoveCoroutine());
    }

    public void Progress()
    {
        if(_isInit == false)
            return;
        
        PlaySfx();
        PlayFx();

        AttackDamage();

        if (_isEnter == true)
        {
            HitStop();
            CameraShake();
        }
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
    }

    public override void PlaySfx()
    {
        if (_isPlaySFX == false)
            return;
        
        GameManager.instance.soundMng.PlaySFX(_sfxClip);
    }
    
    public override void CameraShake()
    {
        if (_isAbleCameraShake == false)
            return;
        _cinemachineImpulseSource.GenerateImpulse();
        //GameManager.instance.cameraMng.Shake(_cameraShakeAmplitudeGain, _cameraShakeFrequencyGain, _camerShakeTime);
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
                }
            }
        }
    }

    public override void SetDamage(in Damage damage)
    {
        _damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Progress();
        }
    }

    IEnumerator AxeMoveCoroutine()
    {
        float _timer = 0.0f;
        while (_timer < _moveTime)
        {
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.AddForce(new Vector2(_moveSpeed * _dir, 0), ForceMode2D.Impulse);
            
            _timer += GameManager.instance.timeMng.FixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        OnEndSkillEvent?.Invoke();
        Destroy(this.gameObject);
    }
}
