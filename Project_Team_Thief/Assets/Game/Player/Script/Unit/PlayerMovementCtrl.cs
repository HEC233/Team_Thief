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
    [SerializeField]
    private float _jumpPower = 5.0f;

    [SerializeField] 
    private float _addAllJumpPpower = 8.0f;

    [SerializeField] 
    private float _curJumpPower = 0.0f;
    
    [SerializeField]
    private float _addJumpPower = 4f;
    
    private float _maxJumpVelocityY = 7.4f;
    private bool _isMaxJumpVelocityY = false;
    public bool isFristJumpFuncCall = false;
    
    // 시간 관련 변수
    private float _scale = 1;
    
    void Init()
    {
        
    }

    int index = 0;
    float[] timeArray = new float[3] {0.3f, 0.2f, 0.1f};
    float[] powerArray = new float[3] {1, 1, 1};
    private bool[] checkArray = new bool[3] {false, false, false};

    [SerializeField] private bool isForce = true;
    
    public void AddJumpTestToArray(float time)
    {
        // if(index >= 3)
        //     return;
        //
        // if (checkArray[index] == false)
        // {
        //     if (time <= timeArray[index])
        //     {
        //         Debug.Log("'Time : " + time);
        //         checkArray[index] = true;
        //         //_rigidbody2D.AddForce(new Vector2(0, powerArray[index++]), ForceMode2D.Impulse);
        //         Debug.Log("호출");
        //
        //     }
        // }
        
        // impulse = a * t = force = a
        // > fixedUpdate에서 t를 한번 더 곱해주기 때문에 t^2이 아닌 t
        // impulse = a
        // force = a * t
        // 이 코드가 하는 말은
        //impluse로 한번에 줄 값을 Time.fixedDeltaTime을 곱함으로써 1초에 나눈 값을 천천히 addforce해준 것이다.
        
        if(isForce)
        _rigidbody2D.AddForce((new Vector2(0, _addJumpPower) * _addAllJumpPpower), ForceMode2D.Force);
        else
        {
            _rigidbody2D.AddForce((new Vector2(0, _addJumpPower) * _addAllJumpPpower) * Time.fixedDeltaTime, ForceMode2D.Impulse);
        }

    }

    public void asd()
    {
        if(_curJumpPower >= _addAllJumpPpower)
            return;

        float addPower = _addJumpPower * Time.deltaTime;
        float leftPower = _addAllJumpPpower - _curJumpPower;

        if (addPower >= leftPower)
        {
            addPower = leftPower;
        }
        
        _curJumpPower += addPower;
        
        _rigidbody2D.AddForce(new Vector2(0, addPower), ForceMode2D.Impulse);
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

    public void AddJumpForce()
    {
        var power = new Vector3(_rigidbody2D.velocity.x, _addJumpPower * _scale, 0.0f);
        _rigidbody2D.AddForce(power, ForceMode2D.Impulse);
        
        //Debug.Log("호출 벨로시티 : " + _rigidbody2D.velocity);
    }
    

    public void ResetJumpVal()
    {
        isFristJumpFuncCall = true;
        _isMaxJumpVelocityY = false;
        _curJumpPower = 0.0f;
        
        
        index = 0;
        for (int i = 0; i < checkArray.Length; i++)
        {
            checkArray[i] = false;
        }
    }

    public Vector3 GetVelocity()
    {
        return _rigidbody2D.velocity;
    }
    
}
