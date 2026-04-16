using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Cameras")]
    public Camera playerCamera;
    public Camera deathCamera;

    [Header("Health")]
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float currentHP;
    [SerializeField] private float hpRegen = 1f;

    [Header("Offense")]
    [SerializeField] private float physicalAttackDamage = 20f;
    [SerializeField] private float magicAttackDamage = 10f;
    [SerializeField] private float attackSpeed = 1f;

    [Header("Defense")]
    [SerializeField] private float armor = 5f;
    [SerializeField] private float physicalDefense = 10f;
    [SerializeField] private float magicDefense = 8f;

    [Header("Mobility")]
    [SerializeField] private float movementSpeed = 7f;

    [Header("UI")]
    [SerializeField] private HealthBar healthBar;

    private bool isDead = false;
    private PlayerMovement movement;

    void Start()
    {
        currentHP = maxHP;
        movement = GetComponent<PlayerMovement>();

        if (healthBar != null)
        {
            healthBar.SetSliderMax(maxHP);
            healthBar.SetSlider(currentHP);
        }

        ApplyMovementSpeed();
    }

    void Update()
    {
        if (isDead) return;
        RegenerateHP();
    }

    void RegenerateHP()
    {
        if (currentHP < maxHP)
        {
            currentHP += hpRegen * Time.deltaTime;
            currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

            if (healthBar != null)
                healthBar.SetSlider(currentHP);
        }
    }

    public void TakeDamage(float amount)
    {
        float finalDamage = amount * (100f / (100f + armor));

        currentHP -= finalDamage;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        if (healthBar != null)
            healthBar.SetSlider(currentHP);

        Debug.Log("Player HP: " + currentHP);

        if (currentHP <= 0f)
        {
            currentHP = 0f;
            Die();
        }
    }

    public void TakePhysicalDamage(float amount)
    {
        float finalDamage = amount * (100f / (100f + physicalDefense + armor));

        currentHP -= finalDamage;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        if (healthBar != null)
            healthBar.SetSlider(currentHP);

        Debug.Log("Player HP: " + currentHP);

        if (currentHP <= 0f)
        {
            currentHP = 0f;
            Die();
        }
    }

    public void TakeMagicDamage(float amount)
    {
        float finalDamage = amount * (100f / (100f + magicDefense + armor));

        currentHP -= finalDamage;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        if (healthBar != null)
            healthBar.SetSlider(currentHP);

        Debug.Log("Player HP: " + currentHP);

        if (currentHP <= 0f)
        {
            currentHP = 0f;
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        if (healthBar != null)
            healthBar.SetSlider(currentHP);
    }

    public void IncreaseMaxHP(float amount)
    {
        maxHP += amount;
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        if (healthBar != null)
        {
            healthBar.SetSliderMax(maxHP);
            healthBar.SetSlider(currentHP);
        }
    }

    public void IncreaseHPRegen(float amount)
    {
        hpRegen += amount;
    }

    public void IncreaseArmor(float amount)
    {
        armor += amount;
    }

    public void IncreasePhysicalAttack(float amount)
    {
        physicalAttackDamage += amount;
    }

    public void IncreaseMagicAttack(float amount)
    {
        magicAttackDamage += amount;
    }

    public void IncreaseAttackSpeed(float amount)
    {
        attackSpeed += amount;
    }

    public void IncreaseMovementSpeed(float amount)
    {
        movementSpeed += amount;
        ApplyMovementSpeed();
    }

    public void IncreasePhysicalDefense(float amount)
    {
        physicalDefense += amount;
    }

    public void IncreaseMagicDefense(float amount)
    {
        magicDefense += amount;
    }

    public float GetCurrentHP() => currentHP;
    public float GetMaxHP() => maxHP;
    public float GetHPRegen() => hpRegen;
    public float GetArmor() => armor;
    public float GetPhysicalAttack() => physicalAttackDamage;
    public float GetMagicAttack() => magicAttackDamage;
    public float GetAttackSpeed() => attackSpeed;
    public float GetMovementSpeed() => movementSpeed;
    public float GetPhysicalDefense() => physicalDefense;
    public float GetMagicDefense() => magicDefense;

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (playerCamera != null)
            playerCamera.enabled = false;

        if (deathCamera != null)
            deathCamera.enabled = true;

        if (GameManager.instance != null)
            GameManager.instance.GameOver();

        gameObject.SetActive(false);
    }

    void ApplyMovementSpeed()
    {
        if (movement != null)
        {
            movement.moveSpeed = movementSpeed;
        }
    }
}