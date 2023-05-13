using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static void LoadMainLevelScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(1);
    }

    public static void LoadMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}