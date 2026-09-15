using UnityEngine;

public class HealZone : MonoBehaviour
{
    [SerializeField] private int healAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            if (!playerHealth.IsFullHealth)
            {
                playerHealth.Heal(healAmount);
                Destroy(gameObject);
            }
        }
    }
}