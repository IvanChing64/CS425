using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

//Author: Ivan Ching
//Developed from mulitple sources
//Purpose: this script allows the user to select their stage. It holds the code for the node completion logic and also changes the scene to the stage scene. 
//It contains code that when swapping to the other scene gives the grid manager a range of width and height to create the board.


public class levelSelect : MonoBehaviour
{
    //Variables
    public string stageID;

    [Header("Grid Settings")]
    public int stageWidthMin;
    public int stageWidthMax;
    public int stageHeightMin;
    public int stageHeightMax;
    public bool isRandomSize;

    [Header("Branching")]
    public List<string> requiredStageIDs;

    public bool isStart;
    private Button myNodes;

    //When entering the scene, checks nodes to see what is available to the player
    private void Start()
    {
        myNodes = GetComponent<Button>();
        RefreshNodes();
    }

    //Function to 'refresh' the nodes.
    //Checks the availability if the stage is selectable with stage completion requirements.
    //Colors the node according to if the stage is unlocked to be played or not
    public void RefreshNodes()
    {
        bool isUnlocked = false;

        if (isStart)
        {
            isUnlocked = true;
        }

        if(requiredStageIDs != null && requiredStageIDs.Count > 0)
        {
            foreach(string id in requiredStageIDs)
            {
                if (GameProgress.ClearedStages.Contains(id))
                {
                    isUnlocked = true;
                    break;
                }
            }
        }

        myNodes.interactable = isUnlocked;
        Image nodeColor = GetComponent<Image>();
        if (isUnlocked)
        {
            nodeColor.color = Color.white;
        } else
        {
            nodeColor.color = Color.gray;
        }
         
    }

    //Function to give GenerateGrid a random range of numbers to be able to generate a grid
    //Using the inspector, specific nodes and have a set width and height,  i.e. boss stage
    //Sets the active stage ID for tracking progress when leaving the game
    //Loads the next scene, in this case, the grid screen
    public void selectStage()
    {
        if (isRandomSize)
        {
            GridManager.width = Random.Range(stageWidthMin, stageWidthMax);
            GridManager.height = Random.Range(stageHeightMin, stageWidthMax);
        } else
        {
            GridManager.width = 30;
            GridManager.height = 20;
        }
        CurrentSession.ActiveStageID = stageID;
        
        SceneManager.LoadScene("Scenes/SampleScene");
    }
}
