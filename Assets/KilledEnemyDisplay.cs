using UnityEngine;
using TMPro; 

public class KilledEnemyDisplay : MonoBehaviour
{
    public static KilledEnemyDisplay Instance;
    
    public TMP_Text killText;
    private int killCount;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        UpdateKillUI();
    }

    public void AddKill()
    {
        killCount++;
        UpdateKillUI();
    }

    void UpdateKillUI()
    {
        if (killText != null)
        {
            killText.text = killCount.ToString();
        }
    }

    public void SetRecord()
    {
        if (PlayerPrefs.GetInt("Record") < killCount)
        {
            PlayerPrefs.SetInt("Record", killCount);
        }
    }
}