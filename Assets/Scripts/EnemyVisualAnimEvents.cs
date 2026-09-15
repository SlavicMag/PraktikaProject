using UnityEngine;

public class EnemyVisualAnimEvents : MonoBehaviour
{
    private EnemyMelee melee;

    private void Awake()
    {
        melee = GetComponentInParent<EnemyMelee>();
    }

    public void Attack_Hit()
    {
        if (melee != null)
        {
            melee.DealAttackDamage();
        }
    }
}