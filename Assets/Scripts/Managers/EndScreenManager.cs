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
    [SerializeField] private Text RewardText;

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
        SetRewardText();
        GameProgress.ClearedStages.Add(CurrentSession.ActiveStageID);
        EndScreenTitle.text = "Victory!";
    }

    public void SetRewardText()
    {
        RewardText.text = $"Reward: {GameManager.Instance.stageData.currency} Gold\n" +
                          $"Total: {ArmyManager.Instance.GetCurrency()} Gold";
    }

    public void SetLosingText()
    {
        EndScreenTitle.text = "Defeat.";
        RewardText.gameObject.SetActive(false);
    }

    public void EndGameButton()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }

    public void ContinueButton()
    {
        if(CurrentSession.ActiveStageID == "17")
        {
            SceneManager.LoadScene("Scenes/Victory");
        }
        else
        {
            SceneManager.LoadScene("Scenes/StageSelection");
        } 
    }
}
