using UnityEngine;

public class Enemy : MonoBehaviour
{

    private Rigidbody2D rb;
    public string enemyName;
    public int maxHP;
    public int currentHP;
    public int atk;
    public int def;
    public float x;
    public float y;

    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float jumpForce = 20f;
    private float horizontalMovement = 1f;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCollider;
    private Vector2 groundCheckSize = new Vector2(0.9f, 0.3f);
    private bool isGrounded;



    public void Initialize(string name, int maxHP, int atk, int def, float x, float y)
    {
        this.enemyName = name;
        this.maxHP = maxHP;
        this.atk = atk;
        this.def = def;
        this.currentHP = maxHP;
        this.x = x;
        this.y = y;

        
    }

    public void DrawEnemy()
    {
        rb.position = new Vector2(x, y);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {

        Move();

    }
    private void Move()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x)
        {
            horizontalMovement = 1f;
        }
        else if (player.position.x < transform.position.x)
        {
            horizontalMovement = -1f;
        }
        else
        {
            horizontalMovement = 0f;
        }

        rb.linearVelocity = new Vector2(
            moveSpeed * horizontalMovement,
            rb.linearVelocity.y
        );
    }
    //private void Jump()
    //{
    //    rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    //}

    //private void GetIsGrounded()
    //{
    //    isGrounded = Physics2D.OverlapBox(
    //        groundCollider.position,
    //        groundCheckSize,
    //        0,
    //        groundLayer
    //    );
    //}

}
