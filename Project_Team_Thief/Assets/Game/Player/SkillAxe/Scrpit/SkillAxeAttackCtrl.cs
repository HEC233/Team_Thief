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
    private Rigidbody2D _rigidbody2D;
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

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
        _movePositionX = movePosX;
        _moveTime = moveTime;
        _signalSourceAsset = cinemachineSignalSource;
        _dir = dir;
        _axeMultiStageHit = axeMultiStageHit;
        _axeMultiStageHitInterval = axeMultiStageHitInterval;
        _axeMultiStageHitCoroutuineCounter = 0;

        _cinemachineImpulseSource.m_ImpulseDefinition.m_RawSignal = _signalSourceAsset;
        _moveSpeed = (1 / _moveTime) * _movePositionX;
        _damage.knockBack = new Vector2(_damage.knockBack.x * _dir, _damage.knockBack.y);

        StartCoroutine(AxeMoveCoroutine());
        
    }
    
    public void AttackDamage(Collider2D item)
    {
        //============== 고재협이 편집함 ======================
        _damage.hitPosition = item.ClosestPoint(_attackCollider2D.bounds.center);
        //=====================================================
        item.GetComponentInParent<Unit>().HandleHit(_damage);
        
        OnEnemyHitEvent?.Invoke();
    }

    private Collider2D FindEnemyObj()
    {
        _isEnter = false;

        // 다음 프레임에 활성화가 되기 때문에 바로 끄면 체크 X
        if (_attackCollider2D.IsTouchingLayers(_hitLayerMask))
        {
            _attackCollider2D.OverlapCollider(_contactFilter2D, result);
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

        
        UnBind();
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
