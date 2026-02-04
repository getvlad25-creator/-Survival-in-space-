using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 3.5f;
    public int damage = 1;
    
    [Header("Стрельба")]
    public bool canShoot = true;
    public GameObject bulletPrefab;
    
    [Header("Настройки интервалов")]
    [SerializeField] private float _shootIntervalPC = 1.8f;
    [SerializeField] private float _shootIntervalMobile = 3.0f;
    
    private float shootInterval;
    private Transform player;
    private float shootTimer;
    private Rigidbody2D rb;
    private float searchTimer = 0f;
    private float searchInterval = 0.5f; // Ищем игрока каждые 0.5 секунды

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Настройка стрельбы
        shootInterval = Application.isMobilePlatform ? _shootIntervalMobile : _shootIntervalPC;
        shootTimer = shootInterval;
        
        // Первый поиск игрока
        FindPlayer();
    }

    void FindPlayer()
    {
        // СПОСОБ 1: Через PlayerHealth (самый надежный)
        if (PlayerHealth.Instance != null)
        {
            player = PlayerHealth.Instance.transform;
            Debug.Log("Враг нашел игрока через PlayerHealth.Instance: " + player.name);
            return;
        }
        
        // СПОСОБ 2: По тегу
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            Debug.Log("Враг нашел игрока по тегу: " + player.name);
            return;
        }
        
        // СПОСОБ 3: По компоненту PlayerHealth
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>(true); // true = ищет даже неактивные
        if (playerHealth != null)
        {
            player = playerHealth.transform;
            Debug.Log("Враг нашел игрока через FindObjectOfType: " + player.name);
            return;
        }
        
        // СПОСОБ 4: По имени
        playerObject = GameObject.Find("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            Debug.Log("Враг нашел игрока по имени 'Player': " + player.name);
            return;
        }
        
        Debug.LogWarning("Игрок не найден. Будет продолжен поиск...");
        player = null;
    }

    void Update()
    {
        // Поиск игрока с интервалом
        searchTimer += Time.deltaTime;
        if (searchTimer >= searchInterval)
        {
            // Если игрок не найден или уничтожен
            if (player == null || player.gameObject == null)
            {
                FindPlayer();
            }
            searchTimer = 0f;
        }
        
        // Если игрок все еще не найден, стоим на месте
        if (player == null)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        
        // Проверяем, активен ли игрок
        if (!player.gameObject.activeInHierarchy)
        {
            rb.velocity = Vector2.zero;
            player = null; 
            return;
        }
        
        // Движение к игроку
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * speed;
        
        // Поворот к игроку
        LookAtPlayer();
        
        // Стрельба (если игрок найден и в зоне видимости)
        if (canShoot && bulletPrefab != null && player != null)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0)
            {
                Shoot();
                shootTimer = shootInterval;
            }
        }
    }
    
    void LookAtPlayer()
    {
        if (player == null) return;
        
        Vector2 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }
    
    void Shoot()
    {
        if (bulletPrefab == null || player == null) return;
        
        try
        {
            // Создаем пулю
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            
            // Направляем пулю в сторону игрока
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                Vector2 direction = (player.position - transform.position).normalized;
                bulletRb.velocity = direction * 10f;
                
                // Устанавливаем тег и слой для вражеской пули
                bullet.tag = "EnemyBullet";
                
                // Проверяем существует ли слой, если нет - создаем
                int enemyBulletLayer = LayerMask.NameToLayer("EnemyBullet");
                if (enemyBulletLayer != -1)
                {
                    bullet.layer = enemyBulletLayer;
                }
                else
                {
                    Debug.LogWarning("Слой 'EnemyBullet' не существует. Создайте его в настройках проекта.");
                }
            }
            
            Debug.Log("Враг выстрелил!");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Ошибка при стрельбе врага: " + e.Message);
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
    
    void OnDestroy()
    {
        if (rb != null)
            rb.velocity = Vector2.zero;
    }
    
    void OnDrawGizmos()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
}