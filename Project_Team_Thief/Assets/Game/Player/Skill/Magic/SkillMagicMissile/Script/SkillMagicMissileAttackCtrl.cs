using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMagicMissileAttackCtrl : AttackBase
{
    [SerializeField]
    private Rigidbody2D _rigidbody2D;
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    
    private float _movePositionX;
    private float _moveTime;
    private float _moveSpeed;
    private float _dir;
    private float _multiStageHit;
    private float _multiStageHitInterval;

    public override void Init(Damage damage, SkillDataBase skillData)
    {
        base.Init(damage, skillData);
        
        SkillMagicMissileData skillMagicMissileData = skillData as SkillMagicMissileData;
        _movePositionX = skillMagicMissileData.ProjectileMoveX;
        _moveTime = skillMagicMissileData.ProjectileMoveTime;
        _signalSourceAsset = skillMagicMissileData.CinemachineSignalSource;
        _multiStageHit = skillMagicMissileData.HitNumberOfTimes[0];
        _multiStageHitInterval = skillMagicMissileData.HitIntervals[0];
        
        if (_cinemachineImpulseSource != null)
        {
            _cinemachineImpulseSource.m_ImpulseDefinition.m_RawSignal = _signalSourceAsset;
        }

        _moveSpeed = (1 / _moveTime) * _movePositionX;
    }
    
    public void StartMagicMissileProgress(float dir)
    {
        _dir = dir;
        
        StartCoroutine(MoveCoroutine());
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(MultiStageHitCoroutuine(FindEnemyObj()));
        }
    }

    public override void AttackEnd()
    {
        base.AttackEnd();
        _spriteRenderer.enabled = false;
        StopAllCoroutines();
        OnEnemyHitEvent = null;
        Destroy(this.gameObject);
    }

    IEnumerator MultiStageHitCoroutuine(Collider2D collider2D)
    {

        if (collider2D == null)
        {
            Debug.Log("Break");
            yield break;
        }
        
        Unit target = collider2D.GetComponentInParent<Unit>();
        float _timer = _multiStageHitInterval;
        float _counter = 0;

        while (_counter < _multiStageHit)
        {
            _timer += GameManager.instance.TimeMng.FixedDeltaTime;

            if(_timer >= _multiStageHitInterval)
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

        AttackEnd();
    }
    
    IEnumerator MoveCoroutine()
    {
        float _timer = 0.0f;
        while (_timer < _moveTime)
        {
            _rigidbody2D.velocity = Vector2.zero;
            _rigidbody2D.AddForce(new Vector2(_moveSpeed * _dir, 0), ForceMode2D.Impulse);
            
            _timer += GameManager.instance.TimeMng.FixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        
        AttackEnd();
        _rigidbody2D.velocity = Vector2.zero;

    }
}
