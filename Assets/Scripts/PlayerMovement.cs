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

    [Header("Respawn")]
    [SerializeField] private Transform respawnPoint;

    private static readonly Vector2[] GravVectors = { Vector2.down, Vector2.left, Vector2.up, Vector2.right };
    private static readonly float[] GravAngles = { 0f, -90f, 180f, 90f };

    private int _gravDir = 0;
    public int GravDir => _gravDir;
    private Vector2 RightAxis => new Vector2(-GravVectors[_gravDir].y, GravVectors[_gravDir].x);

    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _sr;

    private Vector2 _moveInput;
    private bool _isGrounded;
    private bool _wasGrounded;
    private bool _isRunning;
    private float _rotateCooldown;

    public bool IsDead { get; private set; }

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
        _rb.gravityScale = 0;
    }

    // ── Update ────────────────────────────────────────────────────────────────

    private void Update()
    {
        if (IsDead) return;
        HandleRotateInput();
        HandleRunSound();
    }

    private void FixedUpdate()
    {
        if (IsDead) return;
        ApplyGravity();
        ApplyMovement();
        CheckGrounded();
        UpdateAnimator();
    }

    // ── Physics ───────────────────────────────────────────────────────────────

    private void ApplyGravity()
    {
        Vector2 gravAxis = GravVectors[_gravDir];
        _rb.AddForce(gravAxis * gravityStrength, ForceMode2D.Force);
        float falling = Vector2.Dot(_rb.linearVelocity, gravAxis);
        if (falling > maxFallSpeed)
            _rb.linearVelocity -= gravAxis * (falling - maxFallSpeed);
    }

    private void ApplyMovement()
    {
        float gravVel = Vector2.Dot(_rb.linearVelocity, GravVectors[_gravDir]);
        _rb.linearVelocity = RightAxis * _moveInput.x * moveSpeed + GravVectors[_gravDir] * gravVel;
    }

    private void CheckGrounded()
    {
        _wasGrounded = _isGrounded;
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (!_wasGrounded && _isGrounded)
            Audio.instance.PlaySound(Audio.instance.fall);
    }

    // ── Sound ─────────────────────────────────────────────────────────────────

    private void HandleRunSound()
    {
        bool shouldRun = Mathf.Abs(_moveInput.x) > 0.05f && _isGrounded;

        if (shouldRun && !_isRunning)
        {
            _isRunning = true;
            Audio.instance.PlaySound(Audio.instance.run);
        }
        else if (!shouldRun && _isRunning)
        {
            _isRunning = false;
        }
    }

    // ── Rotate ────────────────────────────────────────────────────────────────

    private void HandleRotateInput()
    {
        if (Keyboard.current == null) return;
        _rotateCooldown -= Time.deltaTime;
        if (_rotateCooldown > 0f) return;

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            Rotate(-1);
            _rotateCooldown = 1f;
            Audio.instance.PlaySound(Audio.instance.rotate);
        }
        else if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Rotate(+1);
            _rotateCooldown = 1f;
            Audio.instance.PlaySound(Audio.instance.rotate);
        }
    }

    private void Rotate(int dir)
    {
        _gravDir = ((_gravDir + dir) % 4 + 4) % 4;
        transform.rotation = Quaternion.Euler(0, 0, GravAngles[_gravDir]);
        _rb.linearVelocity = Vector2.zero;
    }

    // ── Input callbacks ───────────────────────────────────────────────────────

    public void OnMove(InputAction.CallbackContext ctx)
    {
        if (IsDead) return;
        _moveInput = ctx.ReadValue<Vector2>();
        _animator.SetFloat("Run", Mathf.Abs(_moveInput.x));
        if (_moveInput.x > 0.05f) _sr.flipX = false;
        else if (_moveInput.x < -0.05f) _sr.flipX = true;
    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (IsDead || !ctx.performed || !_isGrounded) return;
        float hVel = Vector2.Dot(_rb.linearVelocity, RightAxis);
        Audio.instance.PlaySound(Audio.instance.jump);
        _rb.linearVelocity = RightAxis * hVel + (-GravVectors[_gravDir]) * jumpForce;
    }

    // ── Animator ──────────────────────────────────────────────────────────────

    private void UpdateAnimator()
    {
        float gravVel = Vector2.Dot(_rb.linearVelocity, GravVectors[_gravDir]);
        _animator.SetBool("IsGrounded", _isGrounded);
        _animator.SetBool("IsJumping", !_isGrounded && gravVel < 0);
        _animator.SetBool("IsFalling", !_isGrounded && gravVel > 0.1f);
    }

    // ── Die / Respawn ─────────────────────────────────────────────────────────

    public void Die()
    {
        if (IsDead) return;
        IsDead = true;
        _moveInput = Vector2.zero;
        _rb.linearVelocity = Vector2.zero;
        _animator.SetTrigger("Die");
        Audio.instance.PlaySound(Audio.instance.die);
        StartCoroutine(RespawnRoutine());
    }

    private System.Collections.IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(1f);
        Respawn();
    }

    private void Respawn()
    {
        _gravDir = 0;
        IsDead = false;
        _moveInput = Vector2.zero;
        _rotateCooldown = 0f;

        transform.position = respawnPoint.position;
        transform.rotation = Quaternion.identity;
        _rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = true;

        _animator.ResetTrigger("Die");
        _animator.Play("Idle");
    }
}