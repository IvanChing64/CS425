using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Scenes/SampleScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
