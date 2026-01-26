using UnityEngine;
using UnityEngine.SceneManagement;


//Author:Ivan Ching
//Aggregated from multiple tutorials
//Usage: Pause menu to change scenes,freeze time, and change settings
public class PauseMenu : MonoBehaviour
{

    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject settingUI;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    //Unfreeze time and closes the panal
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    //Freeze time and opens the pause menu panel
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    //Opens another menu panal on top of the pause menu, and closes it.
    public void SettingsMenu()
    {
        settingUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    //closes settings and opens the settings panal
    public void SettingsBack()
    {
        settingUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    //changes scenes to main menu
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        Debug.Log("Loading Menu");
    }
}
