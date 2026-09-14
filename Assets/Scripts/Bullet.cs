using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private int damage = 2;
    [SerializeField] private LayerMask enemyLayer;

    private float direction;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        Vector2 currentPosition = transform.position;

        Vector2 movement = new Vector2(
            direction * speed * Time.deltaTime,
            0f
        );

        Vector2 nextPosition = currentPosition + movement;

        RaycastHit2D hit = Physics2D.Raycast(
            currentPosition,
            movement.normalized,
            movement.magnitude,
            enemyLayer
        );

        if (hit.collider != null)
        {
            EnemyHealth enemyHealth =
                hit.collider.GetComponentInParent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            Destroy(gameObject);
            return;
        }

        transform.position = nextPosition;
    }

    public void SetDirection(float newDirection)
    {
        direction = newDirection;
    }
}