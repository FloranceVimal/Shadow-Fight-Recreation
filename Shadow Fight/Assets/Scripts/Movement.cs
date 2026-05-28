using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Player Stats")]
    public float MovementSpeed = 5f;
    public float jumpPower = 10f;

    [Header("Player Controls")]
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightKey = KeyCode.D;
    public KeyCode jumpKey = KeyCode.W;
    public KeyCode attackKey = KeyCode.F;
    public KeyCode blockKey=KeyCode.Q;
    
    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    
    private float moveInput;
    private bool isFacingRight = true;
    public bool isBlocking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>(); 
        isFacingRight = transform.localScale.x > 0;
    }

    void Update()
    {
        moveInput = 0f; 
        isBlocking = Input.GetKey(blockKey);
        
        if (Input.GetKey(leftKey)) 
        {
            moveInput -= 1f; 
        }
        if (Input.GetKey(rightKey)) 
        {
            moveInput += 1f; 
        }
        //slide attack fix
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Attack")|| isBlocking)
        {
            moveInput = 0f;
        }

        
        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        anim.SetBool("IsGrounded", IsGrounded());
        anim.SetBool("IsBlocking", isBlocking);

        Flip();

        
        if (Input.GetKeyDown(attackKey) && IsGrounded())
        {
            anim.SetTrigger("Attack");
        }
        
        if (Input.GetKeyDown(jumpKey) && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        }
    }
    

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * MovementSpeed, rb.linearVelocity.y);
    }

    private void Flip()
    {
        if (isFacingRight && moveInput < 0f || !isFacingRight && moveInput > 0f)
        {
            isFacingRight = !isFacingRight; 
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}