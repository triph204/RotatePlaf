using UnityEngine;
using UnityEngine.InputSystem;

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

    [Header("Gravity")]
    [SerializeField] private float gravityStrength = 25f;
    [SerializeField] private float maxFallSpeed = 20f;

    // Hướng gravity: 0=xuống 1=trái 2=lên 3=phải
    private static readonly Vector2[] GravVectors = { Vector2.down, Vector2.left, Vector2.up, Vector2.right };
    private static readonly float[] GravAngles = { 0f, -90f, 180f, 90f };

    private int gravDir = 0;
    private bool canRotate = true;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sr;

    private Vector2 moveInput;
    private bool isGrounded;

    [HideInInspector] public bool IsDead;

    public int GravDir => gravDir;
    private Vector2 RightAxis => new Vector2(-GravVectors[gravDir].y, GravVectors[gravDir].x);

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0;
    }

    private void Update()
    {
        if (IsDead) return;

        HandleRotateInput();
    }

    private void FixedUpdate()
    {
        if (IsDead) return;

        ApplyGravity();
        ApplyMovement();
        CheckGrounded();
        UpdateAnimator();
    }

    // ── GRAVITY ──────────────────────────────────────────────────────

    private void ApplyGravity()
    {
        Vector2 gAxis = GravVectors[gravDir];
        rb.AddForce(gAxis * gravityStrength, ForceMode2D.Force);

        float falling = Vector2.Dot(rb.linearVelocity, gAxis);
        if (falling > maxFallSpeed)
            rb.linearVelocity -= gAxis * (falling - maxFallSpeed);
    }

    // ── MOVEMENT ─────────────────────────────────────────────────────

    private void ApplyMovement()
    {
        Vector2 gravAxis = GravVectors[gravDir];
        float gravVel = Vector2.Dot(rb.linearVelocity, gravAxis);
        rb.linearVelocity = RightAxis * moveInput.x * moveSpeed + gravAxis * gravVel;
    }

    // ── JUMP ─────────────────────────────────────────────────────────

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    // ── ROTATE ───────────────────────────────────────────────────────

    private float rotateCooldown = 0f;

    private void HandleRotateInput()
    {
        if (Keyboard.current == null) return;

        rotateCooldown -= Time.deltaTime;
        if (rotateCooldown > 0f) return;

        if (Keyboard.current.eKey.wasPressedThisFrame) { Rotate(-1); rotateCooldown = 1f; }
        else if (Keyboard.current.qKey.wasPressedThisFrame) { Rotate(+1); rotateCooldown = 1f; }
    }

    private void Rotate(int dir)
    {
        gravDir = ((gravDir + dir) % 4 + 4) % 4;
        transform.rotation = Quaternion.Euler(0, 0, GravAngles[gravDir]);
        rb.linearVelocity = Vector2.zero;
    }

    // ── INPUT CALLBACKS (PlayerInput component) ──────────────────────

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (IsDead) return;
        moveInput = ctx.ReadValue<Vector2>();
        animator.SetFloat("Run", Mathf.Abs(moveInput.x));
        if (moveInput.x > 0.05f) sr.flipX = false;
        else if (moveInput.x < -0.05f) sr.flipX = true;
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (IsDead || !ctx.performed || !isGrounded) return;
        Vector2 jumpDir = -GravVectors[gravDir];
        float hVel = Vector2.Dot(rb.linearVelocity, RightAxis);
        rb.linearVelocity = RightAxis * hVel + jumpDir * jumpForce;
    }

    // ── ANIMATOR ─────────────────────────────────────────────────────

    private void UpdateAnimator()
    {
        float gravVel = Vector2.Dot(rb.linearVelocity, GravVectors[gravDir]);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsJumping", !isGrounded && gravVel < 0);
        animator.SetBool("IsFalling", !isGrounded && gravVel > 0.1f);
    }

    // ── DIE / RESPAWN ─────────────────────────────────────────────────

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

    public void Die()
    {
        if (IsDead) return;
        IsDead = true;
        moveInput = Vector2.zero;
        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("Die");
        StartCoroutine(RespawnRoutine());
    }

    private System.Collections.IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(1f);
        Respawn();
    }

    private void Respawn()
    {
        gravDir = 0;
        transform.position = respawnPoint.position;
        transform.rotation = Quaternion.identity;
        rb.linearVelocity = Vector2.zero;
        moveInput = Vector2.zero;
        canRotate = true;
        rotateCooldown = 0f;
        IsDead = false;
        GetComponent<Collider2D>().enabled = true;
        animator.ResetTrigger("Die");
        animator.SetFloat("Run", 0f);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsFalling", false);
        animator.SetBool("IsGrounded", true);
        animator.Play("Idle");
    }

  
}