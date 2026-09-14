using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    [Header("Floating Text")]
    [SerializeField] private GameObject floatingCombatTextPrefab;

    private int currentHealth;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;

        Debug.Log("Здоровье игрока: " + currentHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        ShowDamageText(damage);

        Debug.Log("Игрок получил урон: " + damage);
        Debug.Log("Здоровье игрока: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead)
        {
            return;
        }

        int oldHealth = currentHealth;

        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        int actualHeal = currentHealth - oldHealth;

        if (actualHeal > 0)
        {
            ShowHealText(actualHeal);
        }

        Debug.Log("Игрок восстановил здоровье: " + actualHeal);
        Debug.Log("Здоровье игрока: " + currentHealth);
    }

    private void ShowDamageText(int damage)
    {
        if (floatingCombatTextPrefab == null)
        {
            return;
        }

        Vector3 spawnPosition = transform.position + Vector3.up * 1.2f;

        GameObject textObject = Instantiate(
            floatingCombatTextPrefab,
            spawnPosition,
            Quaternion.identity
        );

        FloatingCombatText floatingText =
            textObject.GetComponent<FloatingCombatText>();

        if (floatingText != null)
        {
            floatingText.SetText("-" + damage + " HP");
        }
    }

    private void ShowHealText(int amount)
    {
        if (floatingCombatTextPrefab == null)
        {
            return;
        }

        Vector3 spawnPosition = transform.position + Vector3.up * 1.2f;

        GameObject textObject = Instantiate(
            floatingCombatTextPrefab,
            spawnPosition,
            Quaternion.identity
        );

        FloatingCombatText floatingText =
            textObject.GetComponent<FloatingCombatText>();

        if (floatingText != null)
        {
            floatingText.SetText("+" + amount + " HP");
        }
    }

    public bool IsFullHealth
    {
        get
        {
            return currentHealth >= maxHealth;
        }
    }

    public bool IsDead
    {
        get
        {
            return isDead;
        }
    }

    private void Die()
    {
        isDead = true;

        Debug.Log("Игрок умер");

        DisablePlayer();
    }

    private void DisablePlayer()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
            {
                script.enabled = false;
            }
        }

        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.enabled = false;
        }

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }
}