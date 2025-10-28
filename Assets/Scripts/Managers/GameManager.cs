using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState gameState;

    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        ChangeState(GameState.GenerateGrid);
    }

    public void ChangeState(GameState state)
    {
        gameState = state;
        switch (state)
        {
            case GameState.GenerateGrid:
                GridManager.Instance.GenerateGrid();
                Debug.Log($"Changing States");
                break;
            case GameState.SpawnPlayers:
                UnitManager.Instance.SpawnPlayers();
                Debug.Log($"Changing States");
                break;
            case GameState.SpawnEnemies:
                UnitManager.Instance.SpawnEnemies();
                Debug.Log($"Changing States");
                break;
            case GameState.PlayerTurn:
                break;
            case GameState.EnemyTurn:
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
    EnemyTurn =4
}
