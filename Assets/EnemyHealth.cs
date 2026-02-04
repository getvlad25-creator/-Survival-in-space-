using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 3;
    [SerializeField] private ParticleSystem _particle;
    private Spawner spawner;
    private bool isDead = false;

    public void SetSpawner(Spawner spawnerRef)
    {
        spawner = spawnerRef;
    }

    // МЕТОД ДЛЯ Bullet - принимает параметр урона
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        
        health -= damage;
        Debug.Log("Враг получил " + damage + " урона. Осталось здоровья: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    public void TakeDamage()
    {
        TakeDamage(1); // По умолчанию 1 урон
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Дополнительный способ получения урона
        if (other.CompareTag("Bullet") && !isDead)
        {
            // Уничтожаем пулю
            if (other.gameObject != null)
                Destroy(other.gameObject);
            
            // Наносим урон
            TakeDamage(1);
        }
    }

    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log("Враг убит!");
        
        if (_particle != null)
        {
            _particle.Play();
        }
        
        // Счетчики
        if (KilledEnemyDisplay.Instance != null)
        {
            KilledEnemyDisplay.Instance.AddKill();
        }
        
        if (RecordManager.Instance != null)
        {
            RecordManager.Instance.AddKill();
        }
        
        // Уведомляем спавнер
        if (spawner != null)
        {
            spawner.EnemyDefeated(gameObject);
        }

        // Уничтожаем врага
        Destroy(gameObject);
    }
}