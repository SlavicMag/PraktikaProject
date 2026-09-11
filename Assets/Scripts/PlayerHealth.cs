using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public bool IsFullHealth
    {
        get
        {
            return currentHealth >= maxHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log("Здоровье игрока: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        Debug.Log("Игрок восстановил здоровье: " + currentHealth);
    }

    void Die()
    {
        Debug.Log("Игрок умер");
    }
}