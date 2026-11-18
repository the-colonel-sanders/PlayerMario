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
    private bool isJumpHeld;

    [Header("Direction")]
    [SerializeField] private bool isFacingRight = true;

    [Header("Wall Checks")]
    [SerializeField] private float wallSlideSpeed = 2;
    [SerializeField] private bool isWallSliding;
    [SerializeField] private Transform wallCollider;
    private Vector2 wallCheckSize = new Vector2(0.01f, 0.75f);
    [SerializeField] private LayerMask wallLayer;
    // How long the fall eases from wallSlideSpeed back to normal gravity after leaving a wall
    [SerializeField] private float wallSlideExitTime = 0.15f;
    private float wallSlideExitTimer;

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
    // Thin probe at the top (+1) or bottom (-1) edge of the wall check box
    private bool WallProbe(float side)
    {
        Vector2 probePos = new Vector2(wallCollider.position.x, wallCollider.position.y + side * wallCheckSize.y * 0.5f);
        return Physics2D.OverlapBox(probePos, new Vector2(wallCheckSize.x, 0.05f), 0, wallLayer);
    }

    // False when sliding off a wall's bottom edge
    private bool GetIsWallBelow() => WallProbe(-1f);

    // False when going over a wall's top edge, or when the wall is shorter than the player
    private bool GetIsWallAbove() => WallProbe(1f);

    private void ProcessWallSlide()
    {
        if (!isGrounded && GetIsTouchingWall() && horizontalMovement != 0)
        {
            isWallSliding = true;
            // Only ease the fall after leaving a wall if we left from its bottom, not when hopping over its top
            wallSlideExitTimer = GetIsWallAbove() ? wallSlideExitTime : 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;

            // Ease out of the slide: for a moment after leaving the wall, fall with half gravity
            // starting from wallSlideSpeed, instead of snapping straight to full gravity
            if (wallSlideExitTimer > 0f && !isGrounded && !isWallJumping)
            {
                wallSlideExitTimer -= Time.deltaTime;
                float elapsed = wallSlideExitTime - wallSlideExitTimer;
                float gravity = Mathf.Abs(Physics2D.gravity.y) * rb.gravityScale;
                float maxFallSpeed = wallSlideSpeed + 0.5f * gravity * elapsed;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
            }
            else
            {
                wallSlideExitTimer = 0f;
            }
        }
    }

    private void ProcessWallJump()
    {
        if (isWallSliding && !(GetIsWallBelow() && GetIsWallAbove()))
        {
            // The wall doesn't cover the whole wall check: we're at its bottom edge, its top edge,
            // or it's a small wall. No wall jump (and no forced flip); clear any leftover grace time too,
            // so hopping again after clearing a small wall does a normal jump.
            wallJumpTimer = 0f;
        }
        else if (isWallSliding)
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
            // Ignore extra "performed" events from other jump keys while one is already held,
            // otherwise a second key would fire a normal jump right after the wall jump and override it
            if (isJumpHeld) return;
            isJumpHeld = true;

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
            isJumpHeld = false;

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
