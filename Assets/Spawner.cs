using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [Header("Настройки")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float minDistanceFromPlayer = 3f;
    public float minDistanceBetweenEnemies = 2f;
    public float spawnDelay = 2f;
    public int maxEnemies = 6;

    private List<GameObject> activeEnemies = new List<GameObject>();
    private float timeSinceLastSpawn = 0f;
    private bool isInitialized = false;
    private Transform playerTransform;

    void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Точки появления не назначены в инспекторе!");
            return;
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("Префаб врага не назначен!");
            return;
        }

        FindPlayer();

        if (playerTransform != null)
        {
            isInitialized = true;
            SpawnInitialEnemies();
        }
        else
        {
            Debug.LogWarning("Игрок не найден. Поиск в Update.");
        }
    }

    void Update()
    {
        // Eсли не нашли игрока на старте
        if (!isInitialized)
        {
            FindPlayer();
            if (playerTransform != null)
            {
                isInitialized = true;
                SpawnInitialEnemies();
            }
            return;
        }

        activeEnemies.RemoveAll(item => item == null);

        // Таймер и спавн
        timeSinceLastSpawn += Time.deltaTime;

        if (activeEnemies.Count < maxEnemies && timeSinceLastSpawn >= spawnDelay)
        {
            TryToSpawnEnemy();
            timeSinceLastSpawn = 0f;
        }
    }

    void FindPlayer()
    {
        if (PlayerHealth.Instance != null)
        {
            playerTransform = PlayerHealth.Instance.transform;
        }
        else
        {
            // Поиск по тегу "Player"
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player"); 
            if (playerObject == null) playerObject = GameObject.FindGameObjectWithTag("Игрок");

            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
        }
    }

    void SpawnInitialEnemies()
    {
        for (int i = 0; i < Mathf.Min(maxEnemies / 2, spawnPoints.Length); i++)
        {
            TryToSpawnEnemy();
        }
    }

    void TryToSpawnEnemy()
    {
        if (playerTransform == null) return;

        Transform spawnPoint = FindSafeSpawnPoint();
        
        if (spawnPoint != null)
        {
            SpawnEnemyAtPoint(spawnPoint);
        }
    }

    Transform FindSafeSpawnPoint()
    {
        List<Transform> availablePoints = new List<Transform>();
        
        // Пустые точки в массиве
        foreach(var p in spawnPoints) if(p != null) availablePoints.Add(p);

        ShuffleList(availablePoints);

        // Пытаемся найти хорошую точку
        foreach (Transform point in availablePoints)
        {
            if (IsPointSafe(point.position)) return point;
        }

        // Если нет, берем самую дальнюю от игрока
        Transform farthestPoint = null;
        float maxDistance = -1f;

        foreach (Transform point in availablePoints)
        {
            float dist = Vector3.Distance(point.position, playerTransform.position);
            if (dist > maxDistance)
            {
                maxDistance = dist;
                farthestPoint = point;
            }
        }

        return farthestPoint;
    }

    bool IsPointSafe(Vector3 pos)
    {
        if (Vector3.Distance(pos, playerTransform.position) < minDistanceFromPlayer)
            return false;

        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null && Vector3.Distance(pos, enemy.transform.position) < minDistanceBetweenEnemies)
                return false;
        }

        return true;
    }

    void SpawnEnemyAtPoint(Transform spawnPoint)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        
        // Попытка привязать спавнер к врагу
        EnemyHealth health = enemy.GetComponent<EnemyHealth>();
        if (health != null)
        {
            health.SetSpawner(this); 
            activeEnemies.Add(enemy);
        }
        else
        {
            activeEnemies.Add(enemy);
        }
    }

    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void EnemyDefeated(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.2f);
            Gizmos.DrawWireSphere(playerTransform.position, minDistanceFromPlayer);
        }

        Gizmos.color = Color.red;
        if (spawnPoints != null)
        {
            foreach (Transform point in spawnPoints)
            {
                if (point != null) Gizmos.DrawSphere(point.position, 0.3f);
            }
        }
    }
}