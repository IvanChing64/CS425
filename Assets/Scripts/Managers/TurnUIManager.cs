using UnityEngine;
using UnityEngine.UI;
//using TMPro;

//Developer: Ivan Ching

//Changes the Turn number text on the screen
public class TurnUIManager : MonoBehaviour
{
    public static TurnUIManager Instance;

    [SerializeField] private Text turnText;
    [SerializeField] private Text turnTitle;
    [SerializeField] private Image backgroundBox;

    public void Awake()
    {
        Instance = this;
    }

    //updates info on the turn box
    public void UpdateTurnText(int turnNumber,string title, Color boxColor)
    {
        turnText.text = "Turn #" + turnNumber;
        turnTitle.text = title;
        backgroundBox.color = boxColor;
    }
}
