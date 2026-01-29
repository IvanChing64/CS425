using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// <para>Script for handling the end screen</para>
/// </summary>
public class EndScreenManager : MonoBehaviour
{
    public static EndScreenManager Instance;
    [SerializeField] private Canvas EndScreenCanvas;
    [SerializeField] private Text EndScreenTitle;

    void Awake()
    {
        Instance = this;
        HideEndScreen();
    }

    public void ShowEndScreen()
    {
        EndScreenCanvas.gameObject.SetActive(true);
    }

    public void HideEndScreen()
    {
        EndScreenCanvas.gameObject.SetActive(false);
    }

    public void SetWinningText()
    {
        GameProgress.ClearedStages.Add(CurrentSession.ActiveStageID);
        EndScreenTitle.text = "Victory!";
    }

    public void SetLosingText()
    {
        EndScreenTitle.text = "Defeat.";
    }

    /// <summary>
    /// <para>Currently, goes to the main menu. Should go to the stage map when that is implemented</para>
    /// </summary>
    public void EndGameButton()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
        //SceneManager.LoadScene("Scenes/StageSelection"); // or whatever we call it
    }
    public void ContinueButton()
    {
        SceneManager.LoadScene("Scenes/StageSelection");
    }
}
