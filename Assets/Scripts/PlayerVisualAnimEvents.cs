using UnityEngine;

public class PlayerVisualAnimEvents : MonoBehaviour
{
    private PlayerRifle rifle;

    private void Awake()
    {
        rifle = GetComponentInParent<PlayerRifle>();
    }

    public void SpawnBullet()
    {
        if (rifle != null)
        {
            rifle.Shoot();
        }
    }
}