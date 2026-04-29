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
    [SerializeField] private GameScreen currentScreen;
    public static MenuManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    //Hides the continue button when no stages are cleared
    private void Start()
    {
        if (currentScreen == GameScreen.Start)
        {
            if(GameProgress.ClearedStages.Count == 0)
            {
                continueButton.gameObject.SetActive(false);
            }
        }
    }

    //Starts the game, resets the stages, resets the army
    public void StartGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(mainMenuSFX, transform, 1f);
        ArmyManager.Instance.ResetArmy();
        GameProgress.ClearedStages.Clear();
        CurrentSession.ActiveStageID = "";
        SceneManager.LoadScene("Scenes/StoryScenes");
    }

    //Quits game
    public void QuitGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(mainMenuSFX, transform, 1f);
        Application.Quit();
    }

    //Continues to stage selection
    public void ContinueGame()
    {
        SceneManager.LoadScene("Scenes/StageSelection");
    }

    //Continues to story scene
    public void ToStory()
    {
        SceneManager.LoadScene("Scenes/StoryScenes");
    }

    //Continues to controls
    public void ToControls()
    {
        SceneManager.LoadScene("Scenes/Controls");
    }

    //Continues to shop
    public void ToShop()
    {
        SceneManager.LoadScene("Scenes/Shop");
    }

    //Continues to army scene
    public void ToArmy()
    {
        SceneManager.LoadScene("Scenes/Army");
    }

    //goes back to previous scene in the unity build
    public void PreviousScene()
    {
        int previousIndex = SceneManager.GetActiveScene().buildIndex - 1;
        if(previousIndex >= 0)
        {
            SceneManager.LoadScene(previousIndex);
        }
    }

    //restart button on the end screen, resets stages, resets army.
    public void ToBeginning()
    {
        ArmyManager.Instance.ResetArmy();
        GameProgress.ClearedStages.Clear();
        CurrentSession.ActiveStageID = "";
        SceneManager.LoadScene("Scenes/MainMenu");
    }

    public enum GameScreen
    {
        Start = 0,
        Story = 1,
        Controls = 2,
        StageSelect = 3,
        Combat = 4,
        Shop = 5,
        Victory = 6
    }
}
