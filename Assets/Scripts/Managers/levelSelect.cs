using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class levelSelect : MonoBehaviour
{
    public string stageID;
    [Header("Grid Settings")]
    public int stageWidth;
    public int stageHeight;
    public bool isRandomSize;

    [Header("Branching")]
    public Button[] nextSelectableNode;
    public string requiredStageID;

    private Button myNodes;

    private void Start()
    {
        myNodes = GetComponent<Button>();
        RefreshNodes();
    }

    public void RefreshNodes()
    {
        bool isUnlocked = string.IsNullOrEmpty(requiredStageID) || GameProgress.ClearedStages.Contains(requiredStageID);
        myNodes.interactable = isUnlocked;
        GetComponent<Image>().color = isUnlocked ? Color.white : Color.gray;
    }
    public void selectStage()
    {
        if (isRandomSize)
        {
            GridManager.width = Random.Range(10, 20);
            GridManager.height = Random.Range(10, 20);
        }
        CurrentSession.ActiveStageID = stageID;
        
        SceneManager.LoadScene("Scenes/SampleScene");
    }
}
