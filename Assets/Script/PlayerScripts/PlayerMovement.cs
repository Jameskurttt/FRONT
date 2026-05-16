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

    [Header("Audio Sources")]
    public AudioSource footstepSource;
    public AudioSource sfxSource;

    [Header("Movement Sounds")]
    public AudioClip runSound;
    public AudioClip jumpSound;

    [Header("Attack Swing Sounds")]
    public AudioClip swordCombo1Sound;
    public AudioClip swordCombo2Sound;
    public AudioClip swordCombo3Sound;
    public AudioClip bowShootSound;

    [Header("Enemy Hit Sounds")]
    public AudioClip swordCombo1HitSound;
    public AudioClip swordCombo2HitSound;
    public AudioClip swordCombo3HitSound;
    public AudioClip bowHitEnemySound;

    [Header("Volumes")]
    [Range(0f, 1f)] public float runVolume = 0.5f;
    [Range(0f, 1f)] public float jumpVolume = 1f;
    [Range(0f, 1f)] public float attackVolume = 1f;
    [Range(0f, 1f)] public float hitVolume = 1f;

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
            animator.ResetTrigger("Shoot");
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

        return dir.sqrMagnitude > 0.001f ? dir.normalized : transform.forward;
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

        HandleRunningSound(input);
    }

    void HandleRunningSound(Vector3 input)
    {
        bool isMoving = input.magnitude > 0.1f;

        if (footstepSource == null || runSound == null)
            return;

        if (isMoving && isGrounded)
        {
            if (!footstepSource.isPlaying)
            {
                footstepSource.clip = runSound;
                footstepSource.volume = runVolume;
                footstepSource.loop = true;
                footstepSource.Play();
            }
        }
        else
        {
            if (footstepSource.isPlaying)
                footstepSource.Stop();
        }
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

        if (Input.GetButtonDown("Jump") && isGrounded && !Input.GetMouseButton(0) && !isAttacking)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (animator != null)
                animator.SetTrigger("Jump");

            PlayJumpSound();
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
        if (weaponPickup == null || animator == null)
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

                animator.ResetTrigger("Jump");
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
            animator.SetTrigger("Shoot");
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
        float bonusSpeed = 0f;

        if (playerStats != null)
            bonusSpeed = playerStats.GetMovementSpeed();

        return defaultMoveSpeed + bonusSpeed;
    }

    public void PlayJumpSound()
    {
        if (sfxSource != null && jumpSound != null)
            sfxSource.PlayOneShot(jumpSound, jumpVolume);
    }

    public void PlaySwordCombo1Sound()
    {
        if (sfxSource != null && swordCombo1Sound != null)
            sfxSource.PlayOneShot(swordCombo1Sound, attackVolume);
    }

    public void PlaySwordCombo2Sound()
    {
        if (sfxSource != null && swordCombo2Sound != null)
            sfxSource.PlayOneShot(swordCombo2Sound, attackVolume);
    }

    public void PlaySwordCombo3Sound()
    {
        if (sfxSource != null && swordCombo3Sound != null)
            sfxSource.PlayOneShot(swordCombo3Sound, attackVolume);
    }

    public void PlayBowShootSound()
    {
        if (sfxSource != null && bowShootSound != null)
            sfxSource.PlayOneShot(bowShootSound, attackVolume);
    }

    public void PlaySwordCombo1HitSound()
    {
        if (sfxSource != null && swordCombo1HitSound != null)
            sfxSource.PlayOneShot(swordCombo1HitSound, hitVolume);
    }

    public void PlaySwordCombo2HitSound()
    {
        if (sfxSource != null && swordCombo2HitSound != null)
            sfxSource.PlayOneShot(swordCombo2HitSound, hitVolume);
    }

    public void PlaySwordCombo3HitSound()
    {
        if (sfxSource != null && swordCombo3HitSound != null)
            sfxSource.PlayOneShot(swordCombo3HitSound, hitVolume);
    }

    public void PlayBowHitEnemySound()
    {
        if (sfxSource != null && bowHitEnemySound != null)
            sfxSource.PlayOneShot(bowHitEnemySound, hitVolume);
    }
}