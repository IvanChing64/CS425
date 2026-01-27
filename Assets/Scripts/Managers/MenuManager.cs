using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// <para>Script for handling menu buttons functionality</para>
/// </summary>
/// <remarks>by Liam Riel</remarks>
public class MenuManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] mainMenuSFX;
    public static MenuManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void StartGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(mainMenuSFX, transform, 1f);  
        SceneManager.LoadScene("Scenes/SampleScene");
    }

    public void QuitGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(mainMenuSFX, transform, 1f);
        Application.Quit();
    }
}
