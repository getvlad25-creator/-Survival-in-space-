using UnityEngine;
using UnityEngine.SceneManagement;

public class LozeMenu : MonoBehaviour
{
    public void RestartGame()
    {
       SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    
    public void ExitTomenu ()
    {
        SceneManager.LoadScene(0);
    }
}
