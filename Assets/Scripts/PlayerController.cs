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

    [Header("Wall Checks")]


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
        Move();
        GetIsGrounded();
    }

    /*void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = groundCollider.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(groundCheckSize.x, groundCheckSize.y, 0.1f));
    }*/
    private void GetIsGrounded()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapBox(groundCollider.position, groundCheckSize, 0, groundDef);
        if (isGrounded && !wasGrounded)
        {
            numJumps = maxJumps;
        }
    }


    public void MoveInput(InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<Vector2>());
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
            if (numJumps > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                numJumps--;
            }
        }
        else if (context.canceled)
        {
            if (rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            }

        }
    }   
}
