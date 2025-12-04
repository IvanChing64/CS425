using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState gameState;
    public int turnNumber;

    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        ChangeState(GameState.GenerateGrid);
    }

    //Game state manager
    public void ChangeState(GameState state)
    {
        gameState = state;
        switch (state)
        {
            case GameState.GenerateGrid:
                turnNumber = 0;
                GridManager.Instance.GenerateGrid();
                Debug.Log($"Changing States (Generate Grid -> Spawn Players)");
                break;
            case GameState.SpawnPlayers:
                UnitManager.Instance.SpawnPlayers();
                Debug.Log($"Changing States (Spawn Players -> Spawn Enemies)");
                break;
            case GameState.SpawnEnemies:
                UnitManager.Instance.SpawnEnemies();
                Debug.Log($"Changing States (Spawn Enemies -> Player Turn)");
                break;
            case GameState.PlayerTurn:
                turnNumber += 1;
                Debug.Log("Player Turn");
                Debug.Log("Turn Number" + turnNumber);
                TurnUIManager.Instance.UpdateTurnText(turnNumber);
                break;
            case GameState.EnemyTurn:
                turnNumber += 1;
                Debug.Log("Enemy Turn");
                Debug.Log("Turn Number" + turnNumber);
                UnitManager.Instance.BeginEnemyTurn();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }
}

public enum GameState{
    GenerateGrid = 0,
    SpawnPlayers = 1,
    SpawnEnemies = 2,
    PlayerTurn=3,
    EnemyTurn =4,
}
