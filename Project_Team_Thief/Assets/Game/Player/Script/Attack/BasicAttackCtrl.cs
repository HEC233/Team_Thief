using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Assert = UnityEngine.Assertions.Assert;

public class BasicAttackCtrl : AttackBase
{
    [SerializeField] 
    private BoxCollider2D _basicAttackCollider2D;
    private ContactFilter2D _contactFilter2D;
    private List<Collider2D> result = new List<Collider2D>();
    private bool _isInit = false;

    private void OnEnable()
    {
        if(_basicAttackCollider2D == null)
            Assert.IsNotNull("_basicAttackCollider Noll");
        
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
        
        //this.gameObject.SetActive(false);
    }

    // private void Update()
    // {
    //     Debug.Log("isTouching : " + _basicAttackCollider2D.IsTouchingLayers(_hitLayerMask));
    // }

    public void Progress()
    {
        PlayFx();
        PlaySfx();
        AttackDamage();
        HitStop();
        CameraShake();
        
        //this.gameObject.SetActive(false);
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
    }
    
    public override void CameraShake()
    {
        if (_isAbleCameraShake == false)
            return;
        
    }

    public override void AttackDamage()
    {
        // 다음 프레임에 활성화가 되기 때문에 바로 끄면 체크 X
        if (_basicAttackCollider2D.IsTouchingLayers(_hitLayerMask))
        {
            _basicAttackCollider2D.OverlapCollider(_contactFilter2D, result);
            foreach (var item in result)
            {
                if (item.gameObject.CompareTag("Player"))
                    continue;

                item.GetComponentInParent<Unit>().HandleHit(_damage);
            }
        }
    }

    public override void SetDamage(in Damage damage)
    {

    }
}
