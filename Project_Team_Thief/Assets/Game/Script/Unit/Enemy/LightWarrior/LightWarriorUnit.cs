using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PS.Shadow;

using PS.FX;

public class LightWarriorUnit : Unit
{
    [SerializeField]
    private bool isOnGround = false;
    private bool skipGroundCheck = false;

    public float accelation = 100;
    private bool _isVerticalMoving = false;
    private Vector2 _nextAddingForce = Vector2.zero;

    public Transform footPosition;

    // 유닛이 피해를 입으면 애니메이션을 관장하는 액터또한 알아야 한다. 사이의 중계를 위한 이벤트
    [HideInInspector]
    public UnityEvent hitEvent;
    [HideInInspector]
    public UnityEvent dieEvent;

    [SerializeField]
    private float _hp;

    // 공격에 관한 값들, 계속 힙에 할당되는걸 막고자 클래스 변수로 만들어 놓았다.
    public List<BoxCollider2D> attackBox = new List<BoxCollider2D>();
    private int _curAttackBox = 0;
    public LayerMask hitBoxLayer;
    ContactFilter2D contactFilter = new ContactFilter2D();
    List<Collider2D> result = new List<Collider2D>();
    private bool isLookRight = false;
    private bool IsHorizontalMoving
    {
        get { return !Mathf.Approximately(GetSpeed().x, 0.0f); }
    }

    private float _originalGravityScale = 1.0f;
    private float _maxSpeed;
    private float _minSpeed;
    private float _jumpPower;
    private Vector2 _originalVelocity;
    public override void TimeScaleChangeEnter(float customTimeScale)
    {
        _customTimeScale = customTimeScale;
        _originalVelocity = _rigid.velocity;
        _rigid.velocity *= _customTimeScale;
        _originalGravityScale = _rigid.gravityScale;
        _rigid.gravityScale *= _customTimeScale * _customTimeScale;
        _maxSpeed = _unitData.maxSpeed * _customTimeScale;
        _minSpeed = _unitData.minSpeed * _customTimeScale;
        _jumpPower = _unitData.minJumpPower * _customTimeScale;
    }
    public override void TimeScaleChangeExit()
    {
        if (_customTimeScale != 0) _rigid.velocity /= _customTimeScale; else _rigid.velocity = _originalVelocity;
        _rigid.gravityScale = _originalGravityScale;
        _customTimeScale = 1.0f;
        _maxSpeed = _unitData.maxSpeed;
        _minSpeed = _unitData.minSpeed;
        _jumpPower = _unitData.minJumpPower;
    }

    private void Start()
    {
        _isVerticalMoving = false;

        _hp = _unitData.hp;

        contactFilter.useTriggers = true;
        contactFilter.useLayerMask = true;
        contactFilter.SetLayerMask(hitBoxLayer);

        _maxSpeed = _unitData.maxSpeed;
        _minSpeed = _unitData.minSpeed;
        _jumpPower = _unitData.minJumpPower;


        GameManager.instance.shadow.RegistCollider(_rigid.GetComponent<CapsuleCollider2D>());

        //---------- for test ----------------
        SetDamagePower(_unitData.skillDamage)/*.SetDamageKnockBack(new Vector2(200, 200))*/;
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        _rigid.AddForce(_nextAddingForce * _customTimeScale);
        _nextAddingForce = Vector2.zero;

        if (!skipGroundCheck)
            isOnGround = Physics2D.Raycast(footPosition.position, Vector2.down, Mathf.Epsilon, _groundLayer);
        else
            skipGroundCheck = false;

        if (isOnGround)
        {
            Vector2 velocity = _rigid.velocity;
            if (velocity.sqrMagnitude > _maxSpeed * _maxSpeed)
                _rigid.velocity = velocity.normalized * _maxSpeed;
            _isVerticalMoving = false;
        }
    }

    public override void Idle()
    {
        _rigid.velocity = new Vector2(0.0f, _rigid.velocity.y);
    }

    // 서서히 멈추게 하기 위해서 추가한 함수
    // 그런데 그라운드 피직스 매터리얼에 마찰값을 줘도 갑자기 작동을 안함 왜지?
    // 그래서 일단 정지시키도록 해놨음
    public void MoveStop()
    {
        if (IsHorizontalMoving)
        {
            //_rigid.velocity = _rigid.velocity.normalized;
            _rigid.velocity = Vector2.zero;
        }
        //_rigid.velocity = Vector2.zero;
    }

    /*
     * 변경안 : minSpeed가 적용되도록 해야 한다.
     * 지금은 피직스 매터리얼의 Friction(마찰)값으로 속도를 줄여주고 있는데
     * 이것을 제거하고 스크립트 내에서 자체적으로 속도를 줄여주도록 해야할까?
     * 이동여부 불리언을 만들고 이동시작시 minSpeed로 속도를 바꿔주고
     * accelation만큼 가속해주다 maxSpeed로 클램핑을 해주고
     * 주기적으로 내부에서 속도값을 줄여주는 방식으로
     * 흠... 이러면 지금이랑 다를게 뭐지?
     * 점프와 관련된 부분을 이렇게 하면 될까?
     */ 
    public override void Move(float delta)
    {
        if (!IsHorizontalMoving)
        {
            _rigid.velocity = new Vector2(delta * _minSpeed, _rigid.velocity.y);
        }
        else
            _nextAddingForce = new Vector2(delta * accelation, _nextAddingForce.y);

        SetAttackBoxDir(GetSpeed().x > 0);
    }

    public override void MoveTo(Vector3 position)
    {
    }

    public override void Jump(float jumpForce)
    {
        if (!_isVerticalMoving)
        {
            _rigid.velocity = new Vector2(_rigid.velocity.x, _jumpPower);
            _isVerticalMoving = true;
        }
        else
            _nextAddingForce = new Vector2(_nextAddingForce.x, jumpForce);
    }

    public void SetAttackBox(int index)
    {
        if (index > -1 && index < attackBox.Count)
            _curAttackBox = index;
    }

    public void SetAttackBoxDir(bool isDirectionRight)
    {
        if (isLookRight != isDirectionRight)
        {
            foreach (var box in attackBox)
            {
                box.offset = new Vector2(-box.offset.x, box.offset.y);
            }
            isLookRight = isDirectionRight;
            // 보는 방향이 달라졌을때의 처리를 위한 공간
        }
    }

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

        if (GameManager.instance.FX)
        {
            string fxName = string.Empty;
            switch(inputDamage.additionalInfo)
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
            GameManager.instance.FX.Play(fxName, inputDamage.hitPosition);
        }

        if(GameManager.instance.shadow)
        {
            GameManager.instance.shadow.Burst(inputDamage.hitPosition, 50, 10, 5, true);
        }

        if (_hp <= 0)
        {
            dieEvent.Invoke();
        }
        else
        {
            hitEvent.Invoke();
        }
    }

    public override void HandleDeath()
    {
        GameManager.instance.shadow.UnregistCollider(_rigid.GetComponent<CapsuleCollider2D>());

        DestroyImmediate(transform.parent.gameObject);
    }
}
