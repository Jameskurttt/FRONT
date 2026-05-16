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

    [Header("Equipped Weapon")]
    [SerializeField] private int equippedWeaponDamage = 0;

    [Header("Defense")]
    [SerializeField] private float armor = 5f;
    [SerializeField] private float physicalDefense = 10f;
    [SerializeField] private float magicDefense = 8f;

    [Header("Mobility Bonus")]
    [SerializeField] private float bonusMovementSpeed = 0f;

    [Header("UI")]
    [SerializeField] private HealthBar healthBar;

    private bool isDead = false;

    void Start()
    {
        currentHP = maxHP;

        if (healthBar != null)
        {
            healthBar.SetSliderMax(maxHP);
            healthBar.SetSlider(currentHP);
        }

        ClampStats();
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
        float totalDefense = Mathf.Max(0f, armor);
        float finalDamage = CalculateReducedDamage(amount, totalDefense);

        ApplyFinalDamage(finalDamage);
    }

    public void TakePhysicalDamage(float amount)
    {
        float totalDefense = Mathf.Max(0f, armor + physicalDefense);
        float finalDamage = CalculateReducedDamage(amount, totalDefense);

        ApplyFinalDamage(finalDamage);
    }

    public void TakeMagicDamage(float amount)
    {
        float totalDefense = Mathf.Max(0f, armor + magicDefense);
        float finalDamage = CalculateReducedDamage(amount, totalDefense);

        ApplyFinalDamage(finalDamage);
    }

    private float CalculateReducedDamage(float incomingDamage, float totalDefense)
    {
        float finalDamage = incomingDamage * (100f / (100f + totalDefense));
        finalDamage = Mathf.Max(1f, finalDamage);
        return finalDamage;
    }

    private void ApplyFinalDamage(float finalDamage)
    {
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
        maxHP = Mathf.Max(1f, maxHP);

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
        hpRegen = Mathf.Max(0f, hpRegen);
    }

    public void IncreaseArmor(float amount)
    {
        armor += amount;
        armor = Mathf.Max(0f, armor);
    }

    public void IncreasePhysicalAttack(float amount)
    {
        physicalAttackDamage += amount;
        physicalAttackDamage = Mathf.Max(0f, physicalAttackDamage);
    }

    public void IncreaseMagicAttack(float amount)
    {
        magicAttackDamage += amount;
        magicAttackDamage = Mathf.Max(0f, magicAttackDamage);
    }

    public void IncreaseAttackSpeed(float amount)
    {
        attackSpeed += amount;
        attackSpeed = Mathf.Max(0.05f, attackSpeed);
    }

    public void IncreaseMovementSpeed(float amount)
    {
        bonusMovementSpeed += amount;
        bonusMovementSpeed = Mathf.Max(0f, bonusMovementSpeed);

        Debug.Log("Bonus Movement Speed: " + bonusMovementSpeed);
    }

    public void IncreasePhysicalDefense(float amount)
    {
        physicalDefense += amount;
        physicalDefense = Mathf.Max(0f, physicalDefense);
    }

    public void IncreaseMagicDefense(float amount)
    {
        magicDefense += amount;
        magicDefense = Mathf.Max(0f, magicDefense);
    }

    public void SetEquippedWeaponDamage(int amount)
    {
        equippedWeaponDamage = Mathf.Max(0, amount);
    }

    public void ClearEquippedWeaponDamage()
    {
        equippedWeaponDamage = 0;
    }

    public int GetEquippedWeaponDamage()
    {
        return equippedWeaponDamage;
    }

    public float GetTotalPhysicalAttack()
    {
        return physicalAttackDamage + equippedWeaponDamage;
    }

    public float GetCurrentHP() => currentHP;
    public float GetMaxHP() => maxHP;
    public float GetHPRegen() => hpRegen;
    public float GetArmor() => armor;
    public float GetPhysicalAttack() => physicalAttackDamage;
    public float GetMagicAttack() => magicAttackDamage;
    public float GetAttackSpeed() => attackSpeed;

    public float GetMovementSpeed()
    {
        return bonusMovementSpeed;
    }

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

    void ClampStats()
    {
        maxHP = Mathf.Max(1f, maxHP);
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);
        hpRegen = Mathf.Max(0f, hpRegen);
        armor = Mathf.Max(0f, armor);
        physicalAttackDamage = Mathf.Max(0f, physicalAttackDamage);
        magicAttackDamage = Mathf.Max(0f, magicAttackDamage);
        attackSpeed = Mathf.Max(0.05f, attackSpeed);
        bonusMovementSpeed = Mathf.Max(0f, bonusMovementSpeed);
        physicalDefense = Mathf.Max(0f, physicalDefense);
        magicDefense = Mathf.Max(0f, magicDefense);
    }
}