using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform downAttackPoint;
    [SerializeField] private Animator animator;

    [Header("Attack")]
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private int normalAttackDamage = 1;
    [SerializeField] private int downAttackDamage = 2;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 3f;
    [SerializeField] private float knockbackTime = 0.15f;

    [SerializeField] private LayerMask enemyLayer;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetKey(KeyCode.S))
            {
                DownAttack();
            }
            else
            {
                NormalAttack();
            }
        }
    }

    private void NormalAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void DealNormalAttackDamage()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            attackPoint.position,
            new Vector2(attackRange, attackRange),
            0f,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(normalAttackDamage);

                ApplyKnockback(hit);
            }
        }
    }

    private void DownAttack()
    {
        animator.SetTrigger("DownAttack");
    }

    public void DealDownAttackDamage()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            downAttackPoint.position,
            new Vector2(attackRange, attackRange),
            0f,
            enemyLayer
        );

        bool hitEnemy = false;

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(downAttackDamage);

                ApplyKnockback(hit);

                hitEnemy = true;
            }
        }

        if (hitEnemy)
        {
            PlayerMovement movement = GetComponent<PlayerMovement>();

            if (movement != null)
            {
                movement.BounceUp();
            }
        }

    }

    private void ApplyKnockback(Collider2D hit)
    {
        EnemyMelee enemyMelee = hit.GetComponent<EnemyMelee>();

        if (enemyMelee != null)
        {
            float direction = hit.transform.position.x > transform.position.x
                ? 1f
                : -1f;

            enemyMelee.ApplyKnockback(
                direction,
                knockbackForce,
                knockbackTime
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.DrawWireCube(
                attackPoint.position,
                new Vector3(attackRange, attackRange, 0f)
            );
        }

        if (downAttackPoint != null)
        {
            Gizmos.DrawWireCube(
                downAttackPoint.position,
                new Vector3(attackRange, attackRange, 0f)
            );
        }
    }
}