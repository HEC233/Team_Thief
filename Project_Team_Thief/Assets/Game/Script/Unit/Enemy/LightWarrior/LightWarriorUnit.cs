using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightWarriorUnit : MonsterUnit
{
    IEnumerator AttackMove()
    {
        //_rigid.MovePosition(new )
        // 여기 어떻게 구현해야 되지?
        // 단순 더하기는 안되고
        // 리지드바디에 addposition은 고정주기로 안해도 문제가 없는걸까?

        yield return null;
    }

    public override void Attack()
    {
        // 앞뒤 딜레이는 여기서 구현하는게 나을까 애니메이션을 관장하는 액터에서 처리하는게 나을까
        // 개인적으로 액터가 낫다고 생각은 한다.

        // 히트박스 레이어와 접촉해 있는지 판단
        if(attackBox[_curAttackBox].IsTouchingLayers(hitBoxLayer))
        {
            // 접촉해 있는 히트박스레이어인 콜라이더들 가져오기
            attackBox[_curAttackBox].OverlapCollider(contactFilter, result);
            foreach(var c in result)
            {
                // 콜라이더로 부터 Unit 추출
                var u = c.GetComponentInParent<Unit>();
                // 유닛이 자기자신이거나 없으면 예외처리
                if (u == null || u == this)
                    continue;
                if (u.tag == "Player")
                    u.HandleHit(_damage);
            }
            /*
            GameManager.instance.cameraMng.Shake(data.cameraShakeIntensity, data.cameraShakeCount);
            GameManager.instance.timeMng.BulletTime(data.bulletTimeLength);
            GameManager.instance.timeMng.HitStop(data.hitstopLength);
             */
        }

        StopAllCoroutines();
        StartCoroutine(AttackMove());
    }

    public override void HandleHit(in Damage inputDamage)
    {
        // 대미지 상정방식 기획서에 맞게 변경 필요
        _hp -= inputDamage.power * _unitData.reduceHit;  
        _rigid.AddForce(inputDamage.knockBack, ForceMode2D.Impulse);
        isOnGround = false;
        skipGroundCheck = true;
        skipGroundCheckTime = 0.1f;

        if (_hp <= 0)
        {
            dieEvent.Invoke();
        }
        else
        {
            if (GameManager.instance.FX)
            {
                string fxName = string.Empty;
                switch (inputDamage.additionalInfo)
                {
                    case 0:
                        fxName = "Hit1";
                        break;
                    case 1:
                        fxName = "Hit2";
                        break;
                    case 2:
                        fxName = "Hit3";
                        break;
                }
                var effect = GameManager.instance?.FX.Play(fxName, inputDamage.hitPosition);
                //GameManager.instance?.timeMng.hitStopReadyCheckList.Add(effect.IsPlaying);
            }

            if (GameManager.instance.shadow)
            {
                GameManager.instance.shadow.Burst(inputDamage.hitPosition, 10, 10, 5, true);
            }
            hitEvent.Invoke();
        }
    }
}
