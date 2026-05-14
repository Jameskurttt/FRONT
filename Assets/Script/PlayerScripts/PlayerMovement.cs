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

    [Header("Combat")]
    public PlayerWeaponPickup weaponPickup;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 currentMove;
    private bool isGrounded;

    private bool isAttacking;
    private bool queuedCombo2;
    private bool queuedCombo3;
    private int comboStep;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (playerStats == null)
            playerStats = GetComponent<PlayerHealth>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (weaponPickup == null)
            weaponPickup = GetComponent<PlayerWeaponPickup>();

        if (animator != null)
        {
            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
            animator.ResetTrigger("Attack3");
        }

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
        HandleCombat();

        Vector3 finalMove = currentMove + velocity;
        controller.Move(finalMove * Time.deltaTime);
    }

    Vector3 GetCameraForward()
    {
        if (mainCamera == null)
            return transform.forward;

        Vector3 dir = mainCamera.transform.forward;
        dir.y = 0f;

        return dir.sqrMagnitude > 0.001f
            ? dir.normalized
            : transform.forward;
    }

    void HandleMovement(Vector3 forwardDir)
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 input = new Vector3(h, 0f, v).normalized;

        Vector3 rightDir = Vector3.Cross(Vector3.up, forwardDir);

        Vector3 targetMove =
            (forwardDir * input.z + rightDir * input.x) * GetMoveSpeed();

        currentMove = Vector3.Lerp(
            currentMove,
            targetMove,
            acceleration * Time.deltaTime
        );
    }

    void HandleRotation(Vector3 forwardDir)
    {
        if (forwardDir.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRot = Quaternion.LookRotation(forwardDir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            rotationSpeed * Time.deltaTime
        );
    }

    void HandleJump()
    {
        if (isGrounded && velocity.y < 0f)
            velocity.y = -2f;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null)
                animator.SetTrigger("Jump");
        }
    }

    void HandleGravity()
    {
        if (velocity.y < 0f)
            velocity.y += gravity * fallMultiplier * Time.deltaTime;
        else
            velocity.y += gravity * Time.deltaTime;
    }

    void HandleAnimations()
    {
        if (animator == null)
            return;

        float speed = currentMove.magnitude;

        if (speed < 0.2f)
            speed = 0f;

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsGrounded", controller.isGrounded);
        animator.SetFloat("Velocity", velocity.y);

        UpdateWeaponAnimationState();
    }

    void UpdateWeaponAnimationState()
    {
        if (weaponPickup == null)
        {
            animator.SetInteger("WeaponType", 0);
            return;
        }

        Weapon currentWeapon = weaponPickup.GetCurrentWeapon();

        if (currentWeapon == null)
        {
            animator.SetInteger("WeaponType", 0);
            return;
        }

        switch (currentWeapon.weaponType)
        {
            case WeaponType.Sword:
                animator.SetInteger("WeaponType", 1);
                break;

            case WeaponType.Bow:
                animator.SetInteger("WeaponType", 2);
                break;

            default:
                animator.SetInteger("WeaponType", 0);
                break;
        }
    }

    void HandleCombat()
    {
        if (weaponPickup == null)
            return;

        Weapon currentWeapon = weaponPickup.GetCurrentWeapon();

        if (currentWeapon == null)
            return;

        if (currentWeapon.weaponType == WeaponType.Sword)
            HandleSwordCombat();
        else if (currentWeapon.weaponType == WeaponType.Bow)
            HandleBowCombat();
    }

    void HandleSwordCombat()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        bool inCombo1 = stateInfo.IsName("SWORD_ATTACK1");
        bool inCombo2 = stateInfo.IsName("SWORD_ATTACK2");
        bool inCombo3 = stateInfo.IsName("SWORD_ATTACK3");

        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking)
            {
                isAttacking = true;
                comboStep = 1;

                queuedCombo2 = false;
                queuedCombo3 = false;

                animator.ResetTrigger("Attack2");
                animator.ResetTrigger("Attack3");
                animator.SetTrigger("Attack1");
            }
            else if (inCombo1 && comboStep == 1)
            {
                queuedCombo2 = true;
            }
            else if (inCombo2 && comboStep == 2)
            {
                queuedCombo3 = true;
            }
        }

        if (inCombo1)
        {
            comboStep = 1;

            if (queuedCombo2 && stateInfo.normalizedTime >= 0.45f)
            {
                queuedCombo2 = false;
                comboStep = 2;

                animator.ResetTrigger("Attack1");
                animator.SetTrigger("Attack2");
            }

            if (stateInfo.normalizedTime >= 0.95f && !queuedCombo2)
                ResetCombo();
        }

        if (inCombo2)
        {
            comboStep = 2;

            if (queuedCombo3 && stateInfo.normalizedTime >= 0.45f)
            {
                queuedCombo3 = false;
                comboStep = 3;

                animator.ResetTrigger("Attack2");
                animator.SetTrigger("Attack3");
            }

            if (stateInfo.normalizedTime >= 0.95f && !queuedCombo3)
                ResetCombo();
        }

        if (inCombo3)
        {
            comboStep = 3;

            if (stateInfo.normalizedTime >= 0.95f)
                ResetCombo();
        }
    }

    void HandleBowCombat()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Shoot");
        }
    }

    void ResetCombo()
    {
        isAttacking = false;
        queuedCombo2 = false;
        queuedCombo3 = false;
        comboStep = 0;
    }

    float GetMoveSpeed()
    {
        if (playerStats != null)
            return playerStats.GetMovementSpeed();

        return defaultMoveSpeed;
    }
}