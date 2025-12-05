using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// <para>Script for handling menu buttons functionality</para>
/// </summary>
/// <remarks>by Liam Riel</remarks>
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
