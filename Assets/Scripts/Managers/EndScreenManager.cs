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
    [SerializeField] private TextMeshProUGUI EndScreenTitle;

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
        EndScreenTitle.text = "Victory!";
    }

    public void SetLosingText()
    {
        EndScreenTitle.text = "Defeat.";
    }

    /// <summary>
    /// <para>Currently, restarts the scene. Should go to the stage map when that is implemented</para>
    /// </summary>
    public void EndGameButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // SceneManager.LoadScene("Scenes/StageMap") // or whatever we call it
    }
}
