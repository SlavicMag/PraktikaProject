using UnityEngine;

public class PlayerHeal : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private int healAmount = 1;
    [SerializeField] private float healCooldown = 30f;

    private float cooldownTimer = 0f;

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Q) && cooldownTimer <= 0f)
        {
            Heal();
        }
    }

    private void Heal()
    {
        if (playerHealth.IsFullHealth)
        {
            Debug.Log("Здоровье уже полное");
            return;
        }

        playerHealth.Heal(healAmount);

        cooldownTimer = healCooldown;
    }
}