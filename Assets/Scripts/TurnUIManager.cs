using UnityEngine;
using TMPro;

//Developer: Ivan Ching

public class TurnUIManager : MonoBehaviour
{
    public static TurnUIManager Instance;

    [SerializeField] private TMP_Text turnText;

    public void Awake()
    {
        Instance = this;
    }

    public void UpdateTurnText(int turnNumber)
    {
        turnText.text = "Turn #" + turnNumber;
    }
}
