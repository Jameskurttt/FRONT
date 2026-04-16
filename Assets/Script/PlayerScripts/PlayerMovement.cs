using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float acceleration = 12f;
    public float rotationSpeed = 15f;
    public float jumpForce = 7f;

    [Header("Jump Fix")]
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    public float fallGravityMultiplier = 2.5f;
    public float lowJumpGravityMultiplier = 2f;
    public float maxFallSpeed = 20f;

    [Header("References")]
    public Camera mainCamera;
    public Transform groundCheck;

    [Header("Ground Check")]
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private Vector3 moveInput;
    private bool isGrounded;

    private float coyoteCounter;
    private float jumpBufferCounter;
    private bool jumpConsumed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Prevent unwanted rotations
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Smooth physics
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (mainCamera == null)
            mainCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        moveInput = new Vector3(h, 0f, v).normalized;

        // Check if grounded
        if (groundCheck != null)
            isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        HandleJumpTimers();

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }

        TryJump();
    }

    void FixedUpdate()
    {
        Vector3 cursorDirection = GetCursorDirection();
        Move(cursorDirection);
        Rotate(cursorDirection);
        ApplyBetterGravity();
    }

    // Get direction from player to mouse
    Vector3 GetCursorDirection()
    {
        if (mainCamera == null)
            return transform.forward;

        Vector3 dir = mainCamera.transform.forward;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.001f)
            return dir.normalized;

        return transform.forward;
    }

    // Movement
    void Move(Vector3 forwardDir)
    {
        Vector3 rightDir = Vector3.Cross(Vector3.up, forwardDir);
        Vector3 targetMove = (forwardDir * moveInput.z + rightDir * moveInput.x) * moveSpeed;

        Vector3 velocity = rb.linearVelocity;

        Vector3 newVelocity = Vector3.Lerp(
            new Vector3(velocity.x, 0f, velocity.z),
            targetMove,
            acceleration * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector3(
            newVelocity.x,
            velocity.y,
            newVelocity.z
        );
    }

    // Rotation
    void Rotate(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.001f) return;

        direction.y = 0f;

        Quaternion targetRot = Quaternion.LookRotation(direction);
        Quaternion smoothRot = Quaternion.Slerp(
            rb.rotation,
            targetRot,
            rotationSpeed * Time.fixedDeltaTime
        );

        rb.MoveRotation(smoothRot);
    }

    void HandleJumpTimers()
    {
        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
            jumpConsumed = false;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }

        jumpBufferCounter -= Time.deltaTime;
    }

    void TryJump()
    {
        if (jumpConsumed) return;

        if (jumpBufferCounter > 0f && coyoteCounter > 0f)
        {
            Jump();
            jumpBufferCounter = 0f;
            jumpConsumed = true;
            coyoteCounter = 0f;
            isGrounded = false;
        }
    }

    // Jump function
    void Jump()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void ApplyBetterGravity()
    {
        Vector3 velocity = rb.linearVelocity;

        if (velocity.y < 0f)
        {
            velocity.y += Physics.gravity.y * (fallGravityMultiplier - 1f) * Time.fixedDeltaTime;
        }
        else if (velocity.y > 0f && !Input.GetButton("Jump"))
        {
            velocity.y += Physics.gravity.y * (lowJumpGravityMultiplier - 1f) * Time.fixedDeltaTime;
        }

        if (velocity.y < -maxFallSpeed)
        {
            velocity.y = -maxFallSpeed;
        }

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, velocity.y, rb.linearVelocity.z);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}