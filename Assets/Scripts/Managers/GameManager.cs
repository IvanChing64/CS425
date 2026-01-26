using System;
using UnityEngine;

//Developer: Ivan Ching

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
        // Set internal game state
        gameState = state;

        // If it is the player or enemy turn
        if (gameState == GameState.PlayerTurn || gameState == GameState.EnemyTurn)
        {
            if (CheckPlayerVictory())
            {
                EndScreenManager.Instance.SetWinningText();
                gameState = GameState.EndScreen;
            }
            else if (CheckEnemyVictory())
            {
                EndScreenManager.Instance.SetLosingText();
                gameState = GameState.EndScreen;
            }
        }

        switch (gameState)
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
                CardManager.instance.NextTurn();
                break;
            case GameState.EnemyTurn:
                turnNumber += 1;
                Debug.Log("Enemy Turn");
                Debug.Log("Turn Number" + turnNumber);
                TurnUIManager.Instance.UpdateTurnText(turnNumber);
                UnitManager.Instance.BeginEnemyTurn();
                break;
            case GameState.EndScreen:
                Debug.Log("End screen reached");
                EndScreenManager.Instance.ShowEndScreen();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public bool CheckPlayerVictory() => UnitManager.Instance.enemyUnitCount <= 0;

    public bool CheckEnemyVictory() => UnitManager.Instance.playerUnitCount <= 0;
}

public enum GameState
{
    GenerateGrid = 0,
    SpawnPlayers = 1,
    SpawnEnemies = 2,
    PlayerTurn = 3,
    EnemyTurn = 4,
    EndScreen = 5,
}
