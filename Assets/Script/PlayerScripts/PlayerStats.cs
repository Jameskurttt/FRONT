using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public float currentHP = 100f;
    public float maxHP = 100f;
    public float hpRegen = 1f;

    [Header("Offense")]
    public float physicalAttackDamage = 20f;
    public float magicAttackDamage = 10f;
    public float attackSpeed = 1f;

    [Header("Defense")]
    public float armor = 5f;
    public float physicalDefense = 10f;
    public float magicDefense = 8f;

    [Header("Mobility")]
    public float movementSpeed = 5f;

    void Update()
    {
        RegenerateHP();
    }

    void RegenerateHP()
    {
        if (currentHP < maxHP)
        {
            currentHP += hpRegen * Time.deltaTime;
            currentHP = Mathf.Clamp(currentHP, 0f, maxHP);
        }
    }

    public void TakePhysicalDamage(float damage)
    {
        float finalDamage = damage - physicalDefense;
        finalDamage = Mathf.Max(finalDamage, 1f);

        currentHP -= finalDamage;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void TakeMagicDamage(float damage)
    {
        float finalDamage = damage - magicDefense;
        finalDamage = Mathf.Max(finalDamage, 1f);

        currentHP -= finalDamage;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);
    }

    public void IncreaseMaxHP(float amount)
    {
        maxHP += amount;
        currentHP += amount; // optional para sumabay current HP
        currentHP = Mathf.Clamp(currentHP, 0f, maxHP);
    }

    void Die()
    {
        Debug.Log("Player Died");
    }
}