using UnityEngine;

public class PlayerRifle : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform riflePoint;
    [SerializeField] private GameObject bulletPrefab;

    [SerializeField] private float bulletSpeed = 12f;
    [SerializeField] private float rifleCooldown = 5f;

    private float cooldownTimer = 0f;

    void Update()
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

        animator.SetTrigger("Mosin");
        Debug.Log("Mosin triggered!");
    }

    public void FireBullet()
    {
        float direction = transform.localScale.x > 0f ? 1f : -1f;

        GameObject bullet = Instantiate(
            bulletPrefab,
            riflePoint.position,
            Quaternion.identity
        );

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

        bulletRb.velocity = new Vector2(
            direction * bulletSpeed,
            0f
        );
    }
}