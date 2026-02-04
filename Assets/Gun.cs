using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Gun : MonoBehaviour
{
    [SerializeField] private GameObject _bullet;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private AudioSource _audio;
    
    [Header("Настройки стрельбы")]
    [SerializeField] private float _fireRate = 0.15f;
    
    [Header("Настройки управления")]
    [SerializeField] private GameObject _joystickObject;
    [SerializeField] private GameObject _exitButtonObject;
    
    private float _nextFireTime = 0f;
    private Camera _mainCamera;
    private List<int> _ignoreFingerIds = new List<int>(); 
    
    void Start()
    {
        _mainCamera = Camera.main;
        
        if (_joystickObject == null)
            _joystickObject = GameObject.Find("Fixed Joystick");
        
        if (_exitButtonObject == null)
            _exitButtonObject = GameObject.Find("Exitbutton");
    }
    
    void Update()
    {
        if (_bullet == null || _spawnPoint == null || _mainCamera == null)
            return;
            
        UpdateIgnoreFingers();
        
        // поворачиваем пушку к курсору/пальцу
        RotateGunToTarget();
        
        HandleShooting();
    }
    
    void UpdateIgnoreFingers()
    {
        _ignoreFingerIds.Clear();
        
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);
            
            if (IsTouchOverObject(touch, _joystickObject) || 
                IsTouchOverObject(touch, _exitButtonObject))
            {
                _ignoreFingerIds.Add(touch.fingerId);
            }
        }
    }
    
    void RotateGunToTarget()
    {
        // Получаем позицию цели (курсора или пальца)
        Vector3 targetPosition = GetTargetPosition();
        if (targetPosition == Vector3.zero) return;
        
        Vector3 worldTarget = _mainCamera.ScreenToWorldPoint(targetPosition);
        worldTarget.z = 0;
        
        // Вычисляем направление от пушки к цели
        Vector3 direction = worldTarget - transform.position;
        
        // Вычисляем угол поворота
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Поворачиваем пушку
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
        
    }
    
    Vector3 GetTargetPosition()
    {
        // Для телефона
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                
                if (_ignoreFingerIds.Contains(touch.fingerId))
                    continue;
                    
                return touch.position;
            }
        }
        
        // Для PC - мышь
        if (!IsMouseOverIgnoredUI())
        {
            return Input.mousePosition;
        }
        
        return Vector3.zero;
    }
    
    void HandleShooting()
    {
        bool shouldShoot = false;
        
        // Для телефона
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                
                if (_ignoreFingerIds.Contains(touch.fingerId))
                    continue;
                
                shouldShoot = true;
                break;
            }
        }
        // Для PC
        else if (Input.GetMouseButton(0) && !IsMouseOverIgnoredUI())
        {
            shouldShoot = true;
        }
        
        // Стреляем
        if (shouldShoot && Time.time >= _nextFireTime)
        {
            Shoot();
            _nextFireTime = Time.time + _fireRate;
        }
    }
    
    bool IsTouchOverObject(Touch touch, GameObject targetObject)
    {
        if (targetObject == null || EventSystem.current == null) 
            return false;
        
        if (!EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return false;
        
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touch.position;
        
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        
        foreach (var result in results)
        {
            if (result.gameObject == targetObject || 
                result.gameObject.transform.IsChildOf(targetObject.transform))
            {
                return true;
            }
        }
        
        return false;
    }
    
    bool IsMouseOverIgnoredUI()
    {
        if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())
            return false;
        
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        
        foreach (var result in results)
        {
            if ((_joystickObject != null && 
                 (result.gameObject == _joystickObject || 
                  result.gameObject.transform.IsChildOf(_joystickObject.transform))) ||
                (_exitButtonObject != null && 
                 (result.gameObject == _exitButtonObject || 
                  result.gameObject.transform.IsChildOf(_exitButtonObject.transform))))
            {
                return true;
            }
        }
        
        return false;
    }
    
    void Shoot()
    {
        GameObject newBullet = Instantiate(_bullet, _spawnPoint.position, _spawnPoint.rotation);
        
        // Устанавливаем слой для пули игрока
        newBullet.layer = LayerMask.NameToLayer("PlayerBullet");
        
        if (_audio != null)
            _audio.Play();
    }
}