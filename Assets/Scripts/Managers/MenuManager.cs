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

    /// <summary>
    /// Starts the game, resets the stages, army, and shop
    /// </summary>
    public void StartGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(mainMenuSFX, transform, 1f);
        ArmyManager.Instance.ResetArmy();
        GameProgress.ClearedStages.Clear();
        ShopManager.StockInitialized = false;
        CurrentSession.ActiveStageID = "";
        SceneManager.LoadScene("Scenes/StoryScenes");
    }

    /// <summary>
    /// Quits the game, closes the application
    /// </summary>
    public void QuitGame()
    {
        SoundFXManager.instance.PlaySoundFXClip(mainMenuSFX, transform, 1f);
        Application.Quit();
    }

    /// <summary>
    /// Continues an existing game session, going to the stage selection screen
    /// </summary>
    public void ContinueGame()
    {
        SceneManager.LoadScene("Scenes/StageSelection");
    }

    /// <summary>
    /// Continues to the story scene
    /// </summary>
    public void ToStory()
    {
        SceneManager.LoadScene("Scenes/StoryScenes");
    }

    /// <summary>
    /// Continues to the controls scene
    /// </summary>
    public void ToControls()
    {
        SceneManager.LoadScene("Scenes/Controls");
    }

    /// <summary>
    /// Goes to the shop scene
    /// </summary>
    public void ToShop()
    {
        SceneManager.LoadScene("Scenes/Shop");
    }

    /// <summary>
    /// Goes to the army scene
    /// </summary>
    public void ToArmy()
    {
        SceneManager.LoadScene("Scenes/Army");
    }

    /// <summary>
    /// Goes back to previous scene in the Unity build
    /// </summary>
    public void PreviousScene()
    {
        int previousIndex = SceneManager.GetActiveScene().buildIndex - 1;
        if(previousIndex >= 0)
        {
            SceneManager.LoadScene(previousIndex);
        }
    }

    /// <summary>
    /// Restart button on the end screen, resets stages, resets army.
    /// </summary>
    public void ToBeginning()
    {
        ArmyManager.Instance.ResetArmy();
        GameProgress.ClearedStages.Clear();
        CurrentSession.ActiveStageID = "";
        SceneManager.LoadScene("Scenes/MainMenu");
    }

    /// <summary>
    /// Enumerator for the screens of the game, corresponding to the build index of its Unity scene
    /// </summary>
    public enum GameScreen
    {
        Start = 0,
        Story = 1,
        Controls = 2,
        StageSelect = 3,
        Combat = 4,
        Shop = 5,
        Army = 6,
        Victory = 7
    }
}
