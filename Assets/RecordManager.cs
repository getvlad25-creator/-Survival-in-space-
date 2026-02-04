using UnityEngine;
using UnityEngine.SceneManagement;

public class RecordManager : MonoBehaviour
{
    public static RecordManager Instance; 
    
    // Основные переменные
    private int record = 0;
    private int kills = 0;
    private bool showRecord = false;
    
    // Стиль для отображения текста
    private GUIStyle style;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Не уничтожаем при загрузке новой сцены
            
            LoadRecord(); // Загружаем сохраненный рекорд
            SetupStyle(); // Настраиваем стиль отображения
            
            SceneManager.sceneLoaded += OnSceneLoaded; 
        }
        else
        {
            Destroy(gameObject); // Уничтожаем дубликат
        }
    }
    
    void LoadRecord()
    {
        record = PlayerPrefs.GetInt("MyRecord", 0);
    }
    
    // Настраиваем стиль
    void SetupStyle()
    {
        style = new GUIStyle();
        style.fontSize = 60;
        style.normal.textColor = Color.yellow;
        style.alignment = TextAnchor.LowerRight;
        style.fontStyle = FontStyle.Bold;
    }
    
    // Вызывается при загрузке новой сцены
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateDisplayState();
    }
    
    // Определяем, нужно ли показывать рекорд на текущей сцене
    void UpdateDisplayState()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        showRecord = !sceneName.Contains("Menu"); // Показываем везде, кроме меню
        
        if (showRecord) ResetKills(); // Сбрасываем счетчик при старте уровня
    }
    
    // Увеличиваем счетчик убийств
    public void AddKill()
    {
        kills++;
    }
    
    // Сохраняем рекорд
    public void Save()
    {
        if (kills > record)
        {
            record = kills;
            PlayerPrefs.SetInt("MyRecord", record);
            PlayerPrefs.Save();
        }
        
        ResetKills(); // Сбрасываем после сохранения
    }
    
    // Сбрасываем текущий счетчик
    public void ResetKills()
    {
        kills = 0;
    }
    
    // Отображаем рекорд на экране
    void OnGUI()
    {
        if (showRecord)
        {
            GUI.Label(new Rect(Screen.width - 255, Screen.height - 65, 175, -220), 
                     "RECORD: " + record.ToString(), style);
        }
    }
    
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}