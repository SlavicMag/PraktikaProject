using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;

    [Header("Invulnerability")]
    [SerializeField] private float invulnerabilityDuration = 0.75f;

    [Header("Floating Text")]
    [SerializeField] private GameObject floatingCombatTextPrefab;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI healthText;

    private int currentHealth;
    private bool isDead = false;
    private bool isInvulnerable = false;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();

        Debug.Log("Здоровье игрока: " + currentHealth);
    }

    private void UpdateHealthUI()
    {
        if (healthText == null) return;

        healthText.text = "HP: " + currentHealth + " / " + maxHealth;
    }

    public void TakeDamage(int damage)
    {
        // Игрок уже мёртв
        if (isDead)
        {
            return;
        }

        // Игрок временно неуязвим
        if (isInvulnerable)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();

        ShowDamageText(damage);

        Debug.Log("Игрок получил урон: " + damage);
        Debug.Log("Здоровье игрока: " + currentHealth);

        // Если здоровье закончилось
        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // Запускаем неуязвимость
        StartCoroutine(InvulnerabilityCoroutine());
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

        UpdateHealthUI();

        int actualHeal = currentHealth - oldHealth;

        if (actualHeal > 0)
        {
            ShowHealText(actualHeal);
        }

        Debug.Log("Игрок восстановил здоровье: " + actualHeal);
        Debug.Log("Здоровье игрока: " + currentHealth);
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;

        // Временно отключаем столкновение
        // между Player и Enemy
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Enemy"),
            true
        );

        Debug.Log("Игрок получил неуязвимость");

        yield return new WaitForSeconds(
            invulnerabilityDuration
        );

        // Возвращаем столкновение
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Enemy"),
            false
        );

        isInvulnerable = false;

        Debug.Log("Неуязвимость игрока закончилась");
    }

    private void ShowDamageText(int damage)
    {
        if (floatingCombatTextPrefab == null)
        {
            return;
        }

        Vector3 spawnPosition =
            transform.position + Vector3.up * 1.2f;

        GameObject textObject = Instantiate(
            floatingCombatTextPrefab,
            spawnPosition,
            Quaternion.identity
        );

        FloatingCombatText floatingText =
            textObject.GetComponent<FloatingCombatText>();

        if (floatingText != null)
        {
            floatingText.SetText(
                "-" + damage + " HP"
            );
        }
    }

    private void ShowHealText(int amount)
    {
        if (floatingCombatTextPrefab == null)
        {
            return;
        }

        Vector3 spawnPosition =
            transform.position + Vector3.up * 1.2f;

        GameObject textObject = Instantiate(
            floatingCombatTextPrefab,
            spawnPosition,
            Quaternion.identity
        );

        FloatingCombatText floatingText =
            textObject.GetComponent<FloatingCombatText>();

        if (floatingText != null)
        {
            floatingText.SetText(
                "+" + amount + " HP"
            );
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

    public bool IsInvulnerable
    {
        get
        {
            return isInvulnerable;
        }
    }

    private void Die()
    {
        isDead = true;

        // На случай, если игрок умер во время i-frames
        StopAllCoroutines();

        // Возвращаем столкновение с врагами
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Enemy"),
            false
        );

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

        MonoBehaviour[] scripts =
            GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
            {
                script.enabled = false;
            }
        }

        SpriteRenderer[] sprites =
            GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.enabled = false;
        }

        Collider2D[] colliders =
            GetComponentsInChildren<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }

    public void ResetHealth()
    {
        isDead = false;
        isInvulnerable = false;
        currentHealth = maxHealth;

        UpdateHealthUI();

        StopAllCoroutines();

        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        if (playerLayer != -1 && enemyLayer != -1)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, false);
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.simulated = true;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
            {
                script.enabled = true;
            }
        }

        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.enabled = true;
        }

        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            collider.enabled = true;
        }

        Debug.Log("Игрок возрождён. Здоровье: " + currentHealth);
    }
}