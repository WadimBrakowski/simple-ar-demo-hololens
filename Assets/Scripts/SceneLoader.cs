using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadRoverScene()
    {
        SceneManager.LoadScene("RoverScene");
    }

    public void LoadSetupScene()
    {
        SceneManager.LoadScene("Setup Scene");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
