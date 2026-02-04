using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public int menuScene;
    public int gameScene;
    public int looseScene;
    public int getcheat;

    public void ToMenu()
    {
        SceneManager.LoadScene(menuScene);
    }

    public void ToGame()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void ToLoose()
    {
        SceneManager.LoadScene(looseScene);
    }

    public void GetCheat()
    {
        SceneManager.LoadScene(getcheat);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}