using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int health = 3;
    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Враг получил урон:" + damage);
        Debug.Log("Здоровье врага: " + health);
        if (health <= 0)
        {
            Die();
        }
      
    }
    private void Die()
    {
        Debug.Log("Враг умер");
        Destroy(gameObject);
    }
}
