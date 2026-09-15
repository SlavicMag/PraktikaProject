using UnityEngine;

public class PlayerVisualAnimEvents : MonoBehaviour
{
    private PlayerRifle rifle;
    private PlayerCombat combat;


    private void Awake()
    {
        rifle = GetComponentInParent<PlayerRifle>();
        combat = GetComponentInParent<PlayerCombat>();
    }

    public void SpawnBullet()
    {
        if (rifle != null)
        {
            rifle.Shoot();
        }
    }

    public void Attack_Normal()
    {
        if (combat != null)
        {
            combat.DealNormalAttackDamage();
        }
    }

    public void Attack_Down()
    {
        if (combat != null)
        {
            combat.DealDownAttackDamage();
        }
    }
}