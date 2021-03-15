using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementCtrl : MonoBehaviour
{
    [SerializeField] 
    private Rigidbody2D _rigidbody2D;
    
    // 이동 관련 변수
    private float _curSpeed = 0.0f;
    private float _minSpeed = 0.8f;
    private float _maxSpeed = 6.5f;
    
    // 점프 관련 변수
    private float _jumpPower = 1.0f;
    
    // 시간 관련 변수
    private float _scale = 1;
    
    void Init()
    {
        
    }
    
    // 향후에는 데이터 센터 클래스라던가 데이터를 가지고 있는 함수에서 직접 호출로 받아 올 수 있도록
    // 수정 할 예정
    public void SetVariable(float minSpeed, float maxSpeed, float jumpPower)
    {
        _minSpeed = minSpeed;
        _maxSpeed = maxSpeed;
        _jumpPower = jumpPower;
    }

    public void Move(float dir)
    {
        _rigidbody2D.AddForce(new Vector2(_minSpeed * dir, 0), ForceMode2D.Impulse);

        if (Mathf.Abs(_rigidbody2D.velocity.x) >= _maxSpeed)
            _rigidbody2D.velocity = new Vector2(_maxSpeed * dir, _rigidbody2D.velocity.y);
    }

    public void MoveStop()
    {
        _rigidbody2D.velocity = Vector2.zero;
    }

    public bool IsRunningInertia()
    {
        return Mathf.Abs(_rigidbody2D.velocity.x) >= _maxSpeed ? true : false;
    }

    public void Jump(float jumpForce)
    {
        var power = new Vector3(_rigidbody2D.velocity.x, _jumpPower * _scale, 0.0f);
        _rigidbody2D.AddForce(power, ForceMode2D.Impulse);
    }
}
