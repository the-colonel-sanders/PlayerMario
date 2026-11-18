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

    [SerializeField] private Transform groundCollider;
    private Vector2 groundCheckSize = new Vector2(0.9f, 0.3f);

    [Header("Jumping")]
    [SerializeField] private int maxJumps = 2;
    [SerializeField] private int numJumps = 2;

    [SerializeField] private bool isGrounded;

    [Header("Player Collisions")]
    private BoxCollider2D bc;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
        GetInputs();
        Move();
        isGrounded = GetIsGrounded();
        //DrawDebug();
    }

    void GetInputs()
    {
        // Replaces Input.GetAxisRaw("Horizontal") and Input.GetAxisRaw("Vertical")
        float moveX = 0f;
        float moveY = 0f;

        //Ground RayCasting and Jumping
        //Debug.DrawRay(groundCheckPoint.position, new Vector2(0, -1 - groundCheckY), Color.red);
        //isGrounded = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, groundDef)
        //|| Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, groundDef)
        //|| Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, groundDef);
        Keyboard kb = Keyboard.current;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) moveX -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) moveX += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) moveY -= 1f;
        if ((kb.wKey.isPressed || kb.upArrowKey.isPressed) && numJumps > 0)
        {
            Jump();
        }


        moveInput = new Vector2(moveX, moveY).normalized;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = groundCollider.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(groundCheckSize.x, groundCheckSize.y, 0.1f));
    }
    private bool GetIsGrounded()
    {
        //return Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, groundDef)
            //|| Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, groundDef)
            //|| Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, groundDef);
        if (Physics2D.OverlapBox(groundCollider.position, groundCheckSize, 0, groundDef))
        {
            numJumps = maxJumps;
            return true;
        }
        return false;
    }

    private void Move()
    {
        rb.linearVelocity = new Vector2(walkSpeed * moveInput.x, rb.linearVelocity.y);
    }


    private void Jump()
    {
        numJumps--;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }
}
