using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health = 3;

    [Header("Floating Text")]
    [SerializeField] private GameObject floatingCombatTextPrefab;

    public void TakeDamage(int damage)
    {
        health -= damage;

        ShowDamageText(damage);

        Debug.Log("Враг получил урон:" + damage);
        Debug.Log("Здоровье врага: " + health);

        if (health <= 0)
        {
            Die();
        }
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

    private void Die()
    {
        Debug.Log("Враг умер");

        Destroy(gameObject);
    }
}