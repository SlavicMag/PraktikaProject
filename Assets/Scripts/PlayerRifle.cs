using UnityEngine;

public class PlayerRifle : MonoBehaviour
{
    [SerializeField] private Transform riflePoint;
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private float rifleCooldown = 5f;

    private float cooldownTimer = 0f;

    private void Update()
    {
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.R) && cooldownTimer <= 0f)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        cooldownTimer = rifleCooldown;

        float direction = transform.localScale.x > 0f ? 1f : -1f;

        GameObject bullet = Instantiate(
            bulletPrefab,
            riflePoint.position,
            Quaternion.identity
        );

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.SetDirection(direction);
        }
    }
}