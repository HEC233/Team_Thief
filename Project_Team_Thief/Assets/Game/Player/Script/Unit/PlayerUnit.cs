using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Unit?€ ?¸ë??? ë³´ì´?? ?¸í„°?˜ì´??.
public class PlayerUnit : Unit
{
    [SerializeField] 
    // ë°©í–¥ ê´€?? ë³€??
    
    // Ground Check
    private bool _isGround = true;
    public bool IsGround => _isGround;

    [Header("Ground Check")]
    public Transform groundCheck;    // Groundì²´í¬ë¥? ?„í•¨
    public Vector2 groundCheckBoxSize;
    public LayerMask groundLayer;

    // ?í”„ ê´€?? ë³€??
    private int _jumpCount = 0;
    private float _coyoteTime = 0.2f;
    private float _maxCoyoteTime = 0.2f; 
    private int _maxJumpCount = 2;
    private float _maxJumpTime = 0.1f;


    void Start()
    {
        Init();
    }

    void Init()
    {
    }
    
    // ?¥í›„?ëŠ” ?°ì´?? ?¼í„° ?´ë˜?¤ë¼?˜ê? ?°ì´?°ë? ê°€ì§€ê³? ?ˆëŠ” ?¨ìˆ˜?ì„œ ì§ì ‘ ?¸ì¶œë¡? ë°›ì•„ ?? ?? ?ˆë„ë¡?
    // ?˜ì • ?? ?ˆì •
    public void SetVariable(float coyoteTime, int maxJumpCount, float maxJumpTime)
    {
        _maxCoyoteTime = coyoteTime;
        _maxJumpCount = maxJumpCount;
        _maxJumpTime = maxJumpTime;

        _jumpCount = _maxJumpCount;
        _coyoteTime = _maxCoyoteTime;
    }
    

    public override void Idle()
    {
        base.Idle();
    }

    public void Progress()
    {
        CheckGround();
    }
    
    private void OnDrawGizmos()
    {
        if (groundCheck != null && groundCheckBoxSize != null)
            Gizmos.DrawWireCube(groundCheck.position, groundCheckBoxSize);
    }

    public override void Move(float delta)
    {
        // Vector2 dir = moveUtil.moveforce(5);
        // Rigidbody2D.addfoce(dir);
    }

    public void MoveStop()
    {
    }

    public bool IsRunningInertia()
    {
    }
    public override void Jump(float jumpForce)
    {
        _jumpCount--;
        
    }

    public bool CheckIsJumpAble()
    {
        if (_coyoteTime >= 0.0f)
        {
        }

        return false;
    }

    public void ResetJumpVal()
    {
        _jumpCount = _maxJumpCount;
        _coyoteTime = _maxCoyoteTime;
    }

    public override void Attack()
    {
        base.Attack();
    }

    {
    }
    
    public void CheckMovementDir(float inputDir)
    {
        if (_isFacingRight && inputDir < 0)
        {
            Flip();
        }
        else if (!_isFacingRight && inputDir > 0)
        {
            Flip();
        }
    }
    
    void Flip()
    {
        _facingDir *= -1;
        _isFacingRight = !_isFacingRight;
        transform.parent.transform.Rotate(0.0f, 180.0f, 0.0f);   // yê°? ?Œë¦¬?? ?«ìë¥? curANi ë¨? ? í˜•ë³´ê°„ ?? ? ë‹ˆë©”ì´?˜ì„ ì£¼ë©´ ë¹™ê??„ëŠ” ?´ìœ ? ë‹ˆë©”ì´??
    }

    public void CheckGround()
    {
        _isGround = Physics2D.OverlapBox(groundCheck.position, groundCheckBoxSize, 0, groundLayer);

        if(_isGround == false)
        {
            _coyoteTime -= Time.deltaTime;
        }
        else if (_isGround == true)
        {
            ResetJumpVal();
        }
    }
    
}
