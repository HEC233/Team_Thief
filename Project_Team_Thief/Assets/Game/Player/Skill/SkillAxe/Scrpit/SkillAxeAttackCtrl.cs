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


    public override void Init(Damage damage, SkillDataBase skillData)
    {
        base.Init(damage, skillData);
        
        SkillAxeData skillAxeData = (SkillAxeData) skillData;
        _movePositionX = skillAxeData.ProjectileMoveX;
        _moveTime = skillAxeData.ProjectileMoveTime;
        _signalSourceAsset = skillAxeData.CinemachineSignalSource;
        _axeMultiStageHit = skillAxeData.HitNumberOfTimes[0];
        _axeMultiStageHitInterval = skillAxeData.HitIntervals[0];
        _axeMultiStageHitCoroutuineCounter = 0;
        _curAxeMultiStageHitCoroutuineCounter = 0;

        if (_cinemachineImpulseSource != null)
        {
            _cinemachineImpulseSource.m_ImpulseDefinition.m_RawSignal = _signalSourceAsset;
        }

        _moveSpeed = (1 / _moveTime) * _movePositionX;
    }

    public void StartAxeProgress(float dir)
    {
        _dir = dir;
        
        StartCoroutine(AxeMoveCoroutine());
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

        Unit target = collider2D.GetComponentInParent<Unit>();
        float _timer = _axeMultiStageHitInterval;
        float _counter = 0;

        while (_counter < _axeMultiStageHit)
        {
            _timer += GameManager.instance.TimeMng.FixedDeltaTime;

            if(_timer >= _axeMultiStageHitInterval)
            {
                if (collider2D == null)
                {
                    yield break;
                }
                ProgressTargetSelection(collider2D);
                //AttackDamage(collider2D);

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
        AttackEnd();
    }

    IEnumerator AxeMoveCoroutine()
    {
        float _timer = 0.0f;
        while (_timer < _moveTime)
        {
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.AddForce(new Vector2(_moveSpeed * _dir, 0), ForceMode2D.Impulse);
            
            _timer += GameManager.instance.TimeMng.FixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _spriteRenderer.enabled = false;
        _rigidbody2D.velocity = Vector2.zero;
        StartCoroutine(WaitAxeMultiStageHitCoroutuine());
    }
}
