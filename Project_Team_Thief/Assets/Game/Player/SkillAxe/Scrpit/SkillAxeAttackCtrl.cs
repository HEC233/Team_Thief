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
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    
    private SignalSourceAsset _cinemachineSignalSource;
    
    private ContactFilter2D _contactFilter2D;
    private List<Collider2D> result = new List<Collider2D>();
    private bool _isInit = false;
    private bool _isEnter = false;

    private float _movePositionX;
    private float _moveTime;
    private float _moveSpeed;
    private float _dir;
    private float _axeMultiStageHit;
    private float _axeMultiStageHitInterval;
    private int _axeMultiStageHitCoroutuineCounter;
    private int _curAxeMultiStageHitCoroutuineCounter;

    private List<int> _enterEnemyHashList = new List<int>();    // Hash를 key로 사용하려 한다. 이에 관해서 상의 필요할 듯.

    public void Init(float movePosX, float moveTime, SignalSourceAsset cinemachineSignalSource, float dir, float axeMultiStageHit, float axeMultiStageHitInterval)
    {
        _isInit = true;

        _movePositionX = movePosX;
        _moveTime = moveTime;
        _cinemachineSignalSource = cinemachineSignalSource;
        _dir = dir;
        _axeMultiStageHit = axeMultiStageHit;
        _axeMultiStageHitInterval = axeMultiStageHitInterval;
        _axeMultiStageHitCoroutuineCounter = 0;

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
    }

    public void AttackDamage(Collider2D item)
    {
        //============== 고재협이 편집함 ======================
        _damage.hitPosition = item.ClosestPoint(_basicAttackCollider2D.bounds.center);
        //=====================================================
        item.GetComponentInParent<Unit>().HandleHit(_damage);
        
        OnEnemyHitEvent?.Invoke("Skill1Axe");
    }

    private Collider2D FindEnemyObj()
    {
        _isEnter = false;

        // 다음 프레임에 활성화가 되기 때문에 바로 끄면 체크 X
        if (_basicAttackCollider2D.IsTouchingLayers(_hitLayerMask))
        {
            _basicAttackCollider2D.OverlapCollider(_contactFilter2D, result);
            foreach (var item in result)
            {
                if (_enterEnemyHashList.Contains(item.GetHashCode()) == true)
                    continue;

                if (item.gameObject.CompareTag("Player"))
                    continue;

                if (item.gameObject.CompareTag("Enemy"))
                {
                    _enterEnemyHashList.Add(item.GetHashCode());
                    _isEnter = true;
                    return item;
                }
            }
        }

        return null;
    }

    public override void SetDamage(in Damage damage)
    {
        _damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(AxeMultiStageHitCoroutuine(FindEnemyObj()));
        }
    }

    IEnumerator AxeMultiStageHitCoroutuine(Collider2D collider2D)
    {

        if (collider2D == null)
        {
            Debug.Log("Break");
            yield break;
        }

        _axeMultiStageHitCoroutuineCounter++;

        float _timer = _axeMultiStageHitInterval;
        float _counter = 0;

        while (_counter < _axeMultiStageHit)
        {
            _timer += GameManager.instance.timeMng.FixedDeltaTime;

            if(_timer >= _axeMultiStageHitInterval)
            {
                if (collider2D == null)
                {
                    yield break;
                }

                Progress();
                AttackDamage(collider2D);

                _counter++;
                _timer = 0.0f;
            }


            yield return new WaitForFixedUpdate();
        }

        _curAxeMultiStageHitCoroutuineCounter++;
    }

    IEnumerator WaitAxeMultiStageHitCoroutuine()
    {
        while (_curAxeMultiStageHitCoroutuineCounter < _axeMultiStageHitCoroutuineCounter)
        {
            yield return new WaitForFixedUpdate();
        }


        OnEndSkillEvent?.Invoke();
        Destroy(this.gameObject);
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

        _spriteRenderer.enabled = false;
        _rigidbody2D.velocity = Vector2.zero;
        StartCoroutine(WaitAxeMultiStageHitCoroutuine());
    }
}
