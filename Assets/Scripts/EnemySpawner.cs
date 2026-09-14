using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private int maxEnemies = 5;
    [SerializeField] private float spawnInterval = 3f;

    [Header("Spawn Area")]
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(5f, 3f);

    private float spawnTimer = 0f;

    private void Update()
    {
        if (enemyPrefab == null)
        {
            return;
        }

        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            TrySpawnEnemy();

            spawnTimer = spawnInterval;
        }
    }

    private void TrySpawnEnemy()
    {
        int currentEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;

        if (currentEnemies >= maxEnemies)
        {
            return;
        }

        Vector2 randomPosition = new Vector2(
            Random.Range(
                transform.position.x - spawnAreaSize.x / 2f,
                transform.position.x + spawnAreaSize.x / 2f
            ),
            Random.Range(
                transform.position.y - spawnAreaSize.y / 2f,
                transform.position.y + spawnAreaSize.y / 2f
            )
        );

        Instantiate(
            enemyPrefab,
            randomPosition,
            Quaternion.identity
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(
            transform.position,
            spawnAreaSize
        );
    }
}