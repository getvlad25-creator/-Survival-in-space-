using UnityEngine;
using UnityEngine.UI;

public class Exitbutton : MonoBehaviour
{
    [SerializeField] private Button exitButton;
    
    void Start()
    {
        if (exitButton == null)
            exitButton = GetComponent<Button>();
            
        exitButton.onClick.AddListener(ExitApplication);
    }
    
    public void ExitApplication()
    {
        // Сохраняем рекорд перед выходом
        if (RecordManager.Instance != null)
        {
            RecordManager.Instance.Save();
        }
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    void OnDestroy()
    {
        if (exitButton != null)
            exitButton.onClick.RemoveListener(ExitApplication);
    }
}