using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _speed = 9f;
    [SerializeField] private float _mobileSpeed = 12f;
    [SerializeField] private float _rotationSmoothness = 10f;

    [Header("References")]
    [SerializeField] private Joystick _mobileJoystick;
    [SerializeField] private bool _useKeyboard = true;

    private Rigidbody2D _rigidbody;
    private Camera _mainCam;
    private float _minX, _maxX, _minY, _maxY;
    private float _currentSpeed;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _mainCam = Camera.main;
        
        // Настройка позиции и параметров
        transform.position = Vector3.zero;
        CalculateBounds();
        
        // Выбор скорости в зависимости от платформы
        _currentSpeed = Application.isMobilePlatform ? _mobileSpeed : _speed;

        if (_mobileJoystick == null) FindJoystick();

        Debug.Log("Player initialized. Speed: " + _currentSpeed + ", Platform: " + Application.platform);
    }

    private void FixedUpdate()
    {
        Vector2 moveInput = GetInput();

        if (moveInput.magnitude > 0.1f)
        {
            // Вращение
            float targetAngle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _rotationSmoothness * Time.fixedDeltaTime);
            
            // Движение (нормализуем, чтобы по диагонали не бегал быстрее)
            if (moveInput.magnitude > 1f) moveInput.Normalize();

            float multiplier = (!Application.isMobilePlatform && Input.GetKey(KeyCode.LeftShift)) ? 1.5f : 1f;
            Vector2 newPos = _rigidbody.position + moveInput * (_currentSpeed * multiplier) * Time.fixedDeltaTime;

            // Ограничение зоной видимости
            newPos.x = Mathf.Clamp(newPos.x, _minX, _maxX);
            newPos.y = Mathf.Clamp(newPos.y, _minY, _maxY);

            _rigidbody.MovePosition(newPos);
        }
    }

    private Vector2 GetInput()
    {
        Vector2 input = Vector2.zero;

        if (_useKeyboard)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
        }

        if (input == Vector2.zero && _mobileJoystick != null)
        {
            input.x = _mobileJoystick.Horizontal;
            input.y = _mobileJoystick.Vertical;
        }

        return input;
    }

    private void CalculateBounds()
    {
        Vector2 min = _mainCam.ViewportToWorldPoint(new Vector2(0, 0));
        Vector2 max = _mainCam.ViewportToWorldPoint(new Vector2(1, 1));
        var extents = GetComponent<Collider2D>().bounds.extents;

        _minX = min.x + extents.x;
        _maxX = max.x - extents.x;
        _minY = min.y + extents.y;
        _maxY = max.y - extents.y;
    }

    private void FindJoystick()
    {
        Joystick js = FindObjectOfType<Joystick>();
        if (js != null) _mobileJoystick = js;
    }

    // Публичные методы для взаимодействия
    public void BoostSpeed(float mult, float time) => StartCoroutine(BoostCoroutine(mult, time));

    private IEnumerator BoostCoroutine(float mult, float time)
    {
        _currentSpeed *= mult;
        yield return new WaitForSeconds(time);
        _currentSpeed /= mult;
    }
}