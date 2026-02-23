using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// <para>Script for handling menu buttons functionality</para>
/// </summary>
/// <remarks>by Liam Riel</remarks>
public class MenuManager : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Button continueButton;
    [SerializeField] private AudioClip[] mainMenuSFX;
    public static MenuManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if(GameProgress.ClearedStages.Count == 0)
        {
            continueButton.gameObject.SetActive(false);
        }
    }

    public void StartGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(mainMenuSFX, transform, 1f);
        GameProgress.ClearedStages.Clear();
        CurrentSession.ActiveStageID = "";
        SceneManager.LoadScene("Scenes/StageSelection");
    }

    public void QuitGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(mainMenuSFX, transform, 1f);
        Application.Quit();
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene("Scenes/StageSelection");
    }

    public void ToStory()
    {
        SceneManager.LoadScene("Scenes/StoryScenes");
    }

    public void ToControls()
    {
        SceneManager.LoadScene("Scenes/Controls");
    }

    public void PreviousScene()
    {
        int previousIndex = SceneManager.GetActiveScene().buildIndex - 1;
        if(previousIndex >= 0)
        {
            SceneManager.LoadScene(previousIndex);
        }
    }
}
