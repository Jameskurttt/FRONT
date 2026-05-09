using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float defaultMoveSpeed = 7f;
    public float acceleration = 12f;
    public float rotationSpeed = 15f;

    [Header("Jump Physics")]
    public float jumpHeight = 2.5f;
    public float gravity = -30f;
    public float fallMultiplier = 2.5f;

    [Header("References")]
    public Camera mainCamera;
    public PlayerHealth playerStats;
    public Animator animator;

    private CharacterController controller;

    private Vector3 velocity;
    private Vector3 currentMove;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (playerStats == null)
            playerStats = GetComponent<PlayerHealth>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        Vector3 forwardDir = GetCameraForward();

        HandleMovement(forwardDir);
        HandleRotation(forwardDir);
        HandleJump();
        HandleGravity();
        HandleAnimations();

        Vector3 finalMove = currentMove + velocity;
        controller.Move(finalMove * Time.deltaTime);
    }

    Vector3 GetCameraForward()
    {
        if (mainCamera == null)
            return transform.forward;

        Vector3 dir = mainCamera.transform.forward;
        dir.y = 0f;

        return dir.sqrMagnitude > 0.001f ? dir.normalized : transform.forward;
    }

    void HandleMovement(Vector3 forwardDir)
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(h, 0f, v).normalized;

        Vector3 rightDir = Vector3.Cross(Vector3.up, forwardDir);
        Vector3 targetMove = (forwardDir * input.z + rightDir * input.x) * GetMoveSpeed();

        currentMove = Vector3.Lerp(currentMove, targetMove, acceleration * Time.deltaTime);
    }

    void HandleRotation(Vector3 forwardDir)
    {
        if (forwardDir.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(forwardDir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }

    void HandleJump()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }
    }

    void HandleGravity()
    {
        if (velocity.y < 0)
        {
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }
    }

    void HandleAnimations()
    {
        if (animator == null) return;

        float speed = currentMove.magnitude;

        if (speed < 0.2f)
            speed = 0f;

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", controller.isGrounded);
        animator.SetFloat("Velocity", velocity.y);
    }

    float GetMoveSpeed()
    {
        if (playerStats != null)
            return playerStats.GetMovementSpeed();

        return defaultMoveSpeed;
    }
}