using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Transform downAttackPoint;
    [SerializeField] private Animator animator;
    [SerializeField] private float attackRange = 0.8f;
    [SerializeField] private int normalAttackDamage = 1;
    [SerializeField] private int downAttackDamage = 2;
    [SerializeField] private LayerMask enemyLayer;

    void Update()
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

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemyHealth = hit.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(downAttackDamage);
            }
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