using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _timeToDestroy = 3f;
    [SerializeField] private int _damage = 1;
    
    private float _time;
    private bool _isEnemyBullet = false;
    private Transform _myTransform;
    private Vector3 _moveDirection;
    private bool _isDestroyed = false;

    void Awake()
    {
        // Кэшируем компоненты один раз
        _myTransform = transform;
    }

    void OnEnable()
    {
        // Сбрасываем состояние при повторном использовании
        _time = 0f;
        _isDestroyed = false;
        
        // Определяем, чья это пуля (вызывается каждый раз при активации)
        _isEnemyBullet = (gameObject.layer == LayerMask.NameToLayer("EnemyBullet"));
        
        // Определяем направление движения один раз
        _moveDirection = _myTransform.up;
    }

    void Update()
    {
        // Двигаем пулю с кэшированным направлением и трансформом
        _myTransform.position += _moveDirection * (_speed * Time.deltaTime);
        
        _time += Time.deltaTime;
        if (_time >= _timeToDestroy)
        {
            SafeDestroy();
        }
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (_isDestroyed) return;
        
        // кэшируем тег один раз
        string otherTag = other.tag;
        
        // ПУЛЯ ВРАГА
        if (_isEnemyBullet)
        {
            if (otherTag == "Player")
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(_damage);
                }
                SafeDestroy();
                return;
            }
        }
        // ПУЛЯ ИГРОКА
        else
        {
            if (otherTag == "Enemy")
            {
                EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    // Пробуем вызвать TakeDamage без параметра
                    enemyHealth.TakeDamage();
                }
                SafeDestroy();
                return;
            }
        }
        
        // уничтожать пулю при столкновении с препятствиями
        if (otherTag == "Obstacle" || otherTag == "Wall" || otherTag == "Ground")
        {
            SafeDestroy();
        }
    }

    void OnBecameInvisible()
    {
        // Быстро уничтожаем пулю, когда она выходит за пределы камеры
        if (!_isDestroyed)
        {
            SafeDestroy();
        }
    }

    void SafeDestroy()
    {
        if (_isDestroyed) return;
        
        _isDestroyed = true;
        Destroy(gameObject);
    }
}