using UnityEngine;

public class HealZone : MonoBehaviour
{
    [SerializeField] private int healAmount = 1;

    private bool used = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used)
        {
            return;
        }

        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            if (!playerHealth.IsFullHealth)
            {
                playerHealth.Heal(healAmount);

                used = true;
            }
        }
    }
}