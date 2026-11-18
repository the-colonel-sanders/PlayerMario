using System.ComponentModel.Design.Serialization;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings")]
    private Rigidbody2D rb;
    [SerializeField] private float moveSpeed = 5;
    private Vector2 moveInput;
    private float horizontalMovement;

    [Header("Ground Raycasting")]
    [SerializeField] private float jumpForce = 45;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Transform groundCollider;
    private Vector2 groundCheckSize = new Vector2(0.9f, 0.3f);

    [Header("Jumping")]
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private int numJumps = 2;

    [SerializeField] private bool isGrounded;

    [Header("Direction")]
    [SerializeField] private bool isFacingRight = true;

    [Header("Wall Checks")]
    [SerializeField] private float wallSlideSpeed = 2;
    [SerializeField] private bool isWallSliding;
    [SerializeField] private Transform wallCollider;
    private Vector2 wallCheckSize = new Vector2(0.2f, 0.9f);
    [SerializeField] private LayerMask wallLayer;

    [Header("Wall Jumps")]
    [SerializeField] private bool isWallJumping;
    private float wallJumpDirection;
    [SerializeField] private float wallJumpTime = 0.5f;
    private float wallJumpTimer;
    [SerializeField] private Vector2 wallJumpPower = new Vector2(5f, 10f);
    // How long input is ignored after a wall jump (separate from the wallJumpTime grace window)
    [SerializeField] private float wallJumpLockTime = 0.15f;

    [Header("Player Collisions")]
    private BoxCollider2D bc;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

        GetIsGrounded();
        ProcessWallSlide();
        ProcessWallJump();

        if (!isWallJumping)
        {
            Move();
            Flip();
        }

    }

    /*void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = groundCollider.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(groundCheckSize.x, groundCheckSize.y, 0.1f));
    }*/

    //Environmental Collision Checks
    private void GetIsGrounded()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapBox(groundCollider.position, groundCheckSize, 0, groundLayer);
        if (isGrounded && !wasGrounded)
        {
            numJumps = maxJumps;

            // Landing ends any wall jump immediately so Move() takes over this frame (no floor drift)
            if (isWallJumping)
            {
                CancelInvoke(nameof(CancelWallJump));
                isWallJumping = false;
            }
        }
    }

    private bool GetIsTouchingWall()
    {
        return Physics2D.OverlapBox(wallCollider.position, wallCheckSize, 0, wallLayer);
    }
    private void ProcessWallSlide()
    {
        if (!isGrounded && GetIsTouchingWall() && horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void ProcessWallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }


    //Movement Input Feedback
    public void MoveInput(InputAction.CallbackContext context)
    {
        //Debug.Log(context.ReadValue<Vector2>());
        horizontalMovement = context.ReadValue<Vector2>().x;
        
    }

    private void Move()
    {
        rb.linearVelocity = new Vector2(moveSpeed * horizontalMovement, rb.linearVelocity.y);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Wall Jump
            if (wallJumpTimer > 0f)
            {
                WallJump();
            }
            //Normal Jump
            else if (numJumps > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                numJumps--;
            }
        }
        else if (context.canceled)
        {
            //Short Jump (Jump input released)
            if (rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }

        }
    }   

    private void WallJump()
    {
        isWallJumping = true;
        rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
        wallJumpTimer = 0;

        //Force flip
        if(transform.localScale.x != wallJumpDirection)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1;
            transform.localScale = ls;
        }

        Invoke(nameof(CancelWallJump), wallJumpLockTime);
    }

    private void Flip()
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1;
            transform.localScale = ls;
        }
    }
}
