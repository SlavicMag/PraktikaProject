using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject gameOverMenu;

    [Header("Player")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform respawnPoint;

    private bool isShown = false;

    private void Start()
    {
        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerHealth == null) return;

        if (playerHealth.IsDead && !isShown)
        {
            ShowGameOver();
        }
    }

    private void ShowGameOver()
    {
        isShown = true;

        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(true);
        }

        Debug.Log("Game Over");
    }

    public void OnRestartButton()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }

        PrefabSpawner[] spawners = FindObjectsOfType<PrefabSpawner>();

        foreach (PrefabSpawner spawner in spawners)
        {
            spawner.ResetSpawner();
        }

        if (playerTransform != null && respawnPoint != null)
        {
            playerTransform.position = respawnPoint.position;

            Rigidbody2D rb = playerTransform.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.velocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        if (playerHealth != null)
        {
            playerHealth.ResetHealth();
        }

        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(false);
        }

        isShown = false;
    }

    public void OnExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}