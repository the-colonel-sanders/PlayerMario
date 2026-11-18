using System.ComponentModel.Design.Serialization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings")]
    private Rigidbody2D rb;
    [SerializeField] private float walkSpeed = 5;
    private Vector2 moveInput;

    [Header("Ground Raycasting")]
    [SerializeField] private float jumpForce = 45;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.5f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask groundDef;

    [SerializeField] private bool isTouchingGround;

    [Header("Player Collisions")]
    [SerializeField] BoxCollider2D bc;

    private bool isGrounded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        isTouchingGround = GetIsGrounded();
        GetInputs();
        Move();
    }

    void GetInputs()
    {
        // Replaces Input.GetAxisRaw("Horizontal") and Input.GetAxisRaw("Vertical")
        float moveX = 0f;
        float moveY = 0f;

        //Ground RayCasting and Jumping
        float colliderDistance = bc.bounds.extents.y;
        Debug.DrawRay(groundCheckPoint.position, new Vector2(0, -1 - groundCheckY), Color.red);
        isGrounded = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, groundDef)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, groundDef)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, groundDef);

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX += 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveY -= 1f;
        if ((Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) && isTouchingGround) Jump();


        moveInput = new Vector2(moveX, moveY).normalized;
    }

    private bool GetIsGrounded()
    {
        float colliderDistance = bc.bounds.extents.y;
        //return Physics2D.Raycast(transform.position, Vector2.down, colliderDistance + 0.1f, LayerMask.GetMask("Ground"));
        return Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, groundDef)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, groundDef)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, groundDef);
    }

    private void Move()
    {
        rb.linearVelocity = new Vector2(walkSpeed * moveInput.x, rb.linearVelocity.y);
    }


    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        //rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}
