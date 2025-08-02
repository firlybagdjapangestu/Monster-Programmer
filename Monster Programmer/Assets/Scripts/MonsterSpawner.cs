using UnityEngine;

[AddComponentMenu("Custom/Monster Spawner 2D")]
public class MonsterSpawner2D : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Prefab monster yang akan di‑spawn.")]
    [SerializeField] private GameObject monsterPrefab;

    [Tooltip("Lebar area spawn (sumbu X).")]
    [Min(0f)] public float width = 10f;

    [Tooltip("Tinggi area spawn (sumbu Y).")]
    [Min(0f)] public float height = 5f;

    [Tooltip("Jumlah monster yang akan langsung di‑spawn saat Start.")]
    [SerializeField] private int initialCount = 5;

    [Tooltip("Apakah spawn otomatis di Start?")]
    [SerializeField] private bool spawnOnStart = true;

    private void Start()
    {
        if (spawnOnStart && monsterPrefab != null)
        {
            for (int i = 0; i < initialCount; i++)
            {
                SpawnOne();
            }
        }
    }

    /// <summary>
    /// Mem‑spawn satu monster di posisi acak dalam area persegi panjang (2D).
    /// </summary>
    public void SpawnOne()
    {
        if (monsterPrefab == null) return;

        Vector3 randomOffset = new Vector3(
            Random.Range(-width / 2f, width / 2f),
            Random.Range(-height / 2f, height / 2f),
            0f // Z tetap 0 di 2D
        );

        Instantiate(
            monsterPrefab,
            transform.position + randomOffset,
            Quaternion.identity
        );
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position;
        Vector3 size = new Vector3(width, height, 0f); // 2D: gunakan X dan Y saja
        Gizmos.DrawWireCube(center, size);
    }
#endif
}
