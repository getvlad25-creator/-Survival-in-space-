using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }
    
    public int maxHealth = 15;
    private Text healthText;
    private int currentHealth;
    private bool isInvincible;
    private float invincibilityTimer;
    [SerializeField] private GameObject _lozeMenu;
    [SerializeField] private AudioClip _deathSound;
    
    public int CurrentHealth
    {
        get { return currentHealth; }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        maxHealth = 15;
        currentHealth = maxHealth;
        
        FindHealthText();
        UpdateHealthUI();
        
        Debug.Log("PlayerHealth Здоровье: " + currentHealth.ToString());
    }

    void FindHealthText()
    {
        GameObject textObject = GameObject.Find("HealthText");
        
        if (textObject != null)
        {
            healthText = textObject.GetComponent<Text>();
            if (healthText != null)
            {
                healthText.text = currentHealth.ToString();
                return;
            }
        }
        
        CreateHealthText();
    }

    void CreateHealthText()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        GameObject textObject = new GameObject("HealthText");
        textObject.transform.SetParent(canvas.transform);
        
        healthText = textObject.AddComponent<Text>();
        healthText.text = currentHealth.ToString();
        healthText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        healthText.fontSize = 100;
        healthText.color = Color.red;
        healthText.alignment = TextAnchor.MiddleCenter;
        healthText.fontStyle = FontStyle.Bold;
        
        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 0);
        rect.anchorMax = new Vector2(1, 0);
        rect.pivot = new Vector2(1, 0);
        rect.anchoredPosition = new Vector2(-50, 50);
        rect.sizeDelta = new Vector2(120, 120);
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0) isInvincible = false;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || currentHealth <= 0) return;
        
        currentHealth -= damage;
        isInvincible = true;
        invincibilityTimer = 0.5f;
        
        UpdateHealthUI();
        
        if (currentHealth <= 0)
            Die();
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = currentHealth.ToString();
        }
        else
        {
            FindHealthText();
        }
    }

    void Die()
    {
        Debug.Log("Player died!");
        
        // СОХРАНЯЕМ РЕКОРД
        if (RecordManager.Instance != null)
        {
            RecordManager.Instance.Save();
        }
        
        // Обновляем UI
        if (healthText != null)
            healthText.text = "0";
        
        // Показываем меню поражения
        if (_lozeMenu != null)
        {
            _lozeMenu.SetActive(true);
        }
        
        // Проигрываем звук смерти
        PlayDeathSound();
        
        // Деактивируем игрока
        gameObject.SetActive(false);
    }

    void PlayDeathSound()
    {
        if (_deathSound != null)
        {
            AudioSource.PlayClipAtPoint(_deathSound, transform.position, 1f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Урон при столкновении с врагом
        if (other.CompareTag("Enemy") && !isInvincible)
        {
            Debug.Log("Игрок столкнулся с врагом: " + other.name);
            TakeDamage(1);
        }
        
        // Урон от вражеских пуль
        if (other.CompareTag("EnemyBullet") && !isInvincible)
        {
            Debug.Log("Игрок получил урон от вражеской пули!");
            TakeDamage(1);
            
            // Уничтожаем пулю
            if (other.gameObject != null)
                Destroy(other.gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isInvincible)
        {
            Debug.Log("Игрок столкнулся с врагом (физика): " + collision.gameObject.name);
            TakeDamage(1);
        }
    }
    
    void OnCollisionStay2D(Collision2D collision)
    {
        // Урон каждые 0.5 секунды при постоянном контакте
        if (collision.gameObject.CompareTag("Enemy") && !isInvincible && invincibilityTimer <= 0)
        {
            TakeDamage(1);
        }
    }
    
    public void ForceUpdateUI()
    {
        UpdateHealthUI();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }
    
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}