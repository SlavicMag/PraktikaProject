using System.Collections.Generic;
using UnityEngine;

public class PrefabSpawner : MonoBehaviour
{
    [Header("Prefab")]
    [SerializeField] private GameObject prefab;

    [Header("Spawn Settings")]
    [SerializeField] private int maxCount = 1;
    [SerializeField] private float respawnDelay = 5f;

    [Header("Spawn Point")]
    [SerializeField] private Transform spawnPoint;

    private float respawnTimer = 0f;
    private bool waitingToRespawn = false;
    private readonly List<GameObject> spawned = new List<GameObject>();

    private void Start()
    {
        Spawn();
    }

    private void Update()
    {
        if (prefab == null) return;

        bool wasDestroyed = false;
        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            if (spawned[i] == null)
            {
                spawned.RemoveAt(i);
                wasDestroyed = true;
            }
        }

        if (wasDestroyed && !waitingToRespawn)
        {
            waitingToRespawn = true;
            respawnTimer = respawnDelay;
        }

        if (waitingToRespawn)
        {
            respawnTimer -= Time.deltaTime;

            if (respawnTimer <= 0f)
            {
                waitingToRespawn = false;

                if (spawned.Count < maxCount)
                {
                    Spawn();
                }
            }
        }
    }

    public void Spawn()
    {
        if (prefab == null)
        {
            Debug.LogWarning("PrefabSpawner: префаб не назначен на " + gameObject.name);
            return;
        }

        Vector3 position = spawnPoint != null
            ? spawnPoint.position
            : transform.position;

        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        spawned.Add(instance);
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnPoint.position, 0.2f);
        }
    }

    public void ResetSpawner()
    {
        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            if (spawned[i] != null)
            {
                Destroy(spawned[i]);
            }
        }

        spawned.Clear();

        respawnTimer = 0f;
        waitingToRespawn = false;

        Spawn();
    }
}