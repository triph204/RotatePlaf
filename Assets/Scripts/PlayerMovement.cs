using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isDead;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        Flip();
        animator.SetBool("IsJumping", !isGrounded);
        animator.SetBool("IsFalling", rb.velocity.y < -0.1f);
        animator.SetBool("IsGrounded", isGrounded);
    }

    void Flip()
    {
        if (moveInput.x > 0)
            sr.flipX = false;
        else if (moveInput.x < 0)
            sr.flipX = true;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isDead) return;
        moveInput = context.ReadValue<Vector2>();
        animator.SetFloat("Run", Mathf.Abs(moveInput.x));
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (isDead) return;
        if (context.performed && isGrounded)
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isDead)
            Die();
    }

    private void Die()
    {
        isDead = true;
        moveInput = Vector2.zero;
        rb.velocity = Vector2.zero;

        animator.SetTrigger("Die");
        StartCoroutine(RespawnAfterDie());
    }

    private IEnumerator RespawnAfterDie()
    {
        yield return null;
        float dieLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(1f);
        Respawn();
    }

    private void Respawn()
    {
        transform.position = respawnPoint.position;
        isDead = false;
        GetComponent<Collider2D>().enabled = true;
        rb.velocity = Vector2.zero;
        

        // Reset tất cả params về mặc định
        animator.ResetTrigger("Die");
        animator.SetFloat("Run", 0f);
        animator.SetBool("IsJumping", false);
        animator.SetFloat("VelocityY", 0f);

        animator.Play("Idle");
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}