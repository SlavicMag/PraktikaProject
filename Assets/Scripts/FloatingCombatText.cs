using TMPro;
using UnityEngine;

public class FloatingCombatText : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;

    [Header("Animation")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float lifeTime = 0.8f;

    private float timer;

    private void Awake()
    {
        if (textMesh == null)
        {
            textMesh = GetComponent<TextMeshPro>();
        }
    }

    private void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        timer += Time.deltaTime;

        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    public void SetText(string message)
    {
        textMesh.text = message;
    }
}