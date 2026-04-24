using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

//Developer: Ivan Ching

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState gameState;
    public int turnNumber;
    public bool unitMoving = false;

    public StageData stageData;

    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        stageData = CurrentSession.ActiveStageData;
        ChangeState(GameState.GenerateGrid);
    }
    
    //Game state manager
    public void ChangeState(GameState state)
    {
        if (gameState == state)
        {
            Debug.Log($"Attempted to change to the same game state: {state} - {gameState}");
            return;
        }
        // Set internal game state
        gameState = state;

        // If it is the player or enemy turn
        if (gameState == GameState.PlayerTurn || gameState == GameState.EnemyTurn)
        {
            // If the player is victorious
            if (CheckPlayerVictory())
            {
                ArmyManager.Instance.GainCurrency(stageData.currency);
                EndScreenManager.Instance.SetWinningText();
                gameState = GameState.EndScreen;
            }
            // Otherwise, if the enemy is victorious
            else if (CheckEnemyVictory())
            {
                EndScreenManager.Instance.SetLosingText();
                gameState = GameState.EndScreen;
            }
        }


        //Game State of the game and the ability to switch the state of game
        switch (gameState)
        {
            case GameState.GenerateGrid:
                turnNumber = 0;
                GridManager.Instance.GenerateGrid();
                // Debug.Log($"Changing States (Generate Grid -> Spawn Players)");
                break;
            case GameState.SpawnPlayers:
                UnitManager.Instance.SpawnPlayers();
                // Debug.Log($"Changing States (Spawn Players -> Spawn Enemies)");
                break;
            case GameState.SpawnEnemies:
                UnitManager.Instance.SpawnEnemies();
                // Debug.Log($"Changing States (Spawn Enemies -> Player Turn)");
                break;
            case GameState.PlayerTurn:
                turnNumber += 1;
                // Debug.Log("Player Turn");
                // Debug.Log("Turn Number" + turnNumber);
                UnitManager.Instance.ApplyEndTurnEffects(Faction.Enemy);
                if (CheckEnemyVictory())
                {
                    EndScreenManager.Instance.SetLosingText();
                    ChangeState(GameState.EndScreen);
                }
                combatUIManager.Instance.ShowEndTurnOption();
                TurnUIManager.Instance.UpdateTurnText(turnNumber);
                CardManager.instance.NextTurn();
                if (CheckPlayerVictory())
                {
                    EndScreenManager.Instance.SetWinningText();
                    ChangeState(GameState.EndScreen);
                }
                break;
            case GameState.EnemyTurn:
                // Debug.Log("Enemy Turn");
                // Debug.Log("Turn Number" + turnNumber);
                UnitManager.Instance.ApplyEndTurnEffects(Faction.Player);
                if (CheckPlayerVictory())
                {
                    EndScreenManager.Instance.SetWinningText();
                    ChangeState(GameState.EndScreen);
                }
                combatUIManager.Instance.hideEndTurnOption();
                UnitManager.Instance.BeginEnemyTurn();
                CardManager.instance.ToggleCardArea(false);
                if (CheckEnemyVictory())
                {
                    EndScreenManager.Instance.SetLosingText();
                    ChangeState(GameState.EndScreen);
                }
                break;
            case GameState.EndScreen:
                //Debug.Log("End screen reached");
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
    Null = -1,
    GenerateGrid = 0,
    SpawnPlayers = 1,
    SpawnEnemies = 2,
    PlayerTurn = 3,
    EnemyTurn = 4,
    EndScreen = 5,
}

public static class GameProgress
{
    public static HashSet<string> ClearedStages = new HashSet<string>();
}
