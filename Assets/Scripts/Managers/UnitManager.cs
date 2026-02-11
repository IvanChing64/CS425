using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

//Developer: Ivan Ching
//Edits from Andrew Shelton
//Aggregated from multiple tutorials

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    private List<ScriptableUnit> units;
    //Adding reference to player tile.
    private Tile playerTile;
    private List<Tile> enemyTiles = new List<Tile>();

    public BasePlayer SelectedPlayer;

    private List<BaseEnemy> enemiesSpawned = new List<BaseEnemy>();

    public int enemyUnitCount;
    public int playerUnitCount;

    private void Awake()
    {
        Instance = this;

        units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
    }

    public void SpawnPlayers(int count = 1)
    {
        playerUnitCount = count;
        for (int i = 0; i < playerUnitCount; i++)
        {
            var randomPrefab = GetRandomUnit<BasePlayer>(Faction.Player);
            var spawnedPlayer = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetPlayerSpawnTile();

            randomSpawnTile.setUnit(spawnedPlayer);

            SelectedPlayer = spawnedPlayer;
            Debug.Log("Spawned Player: " + SelectedPlayer.name);

            //Adding reference to player tile.
            playerTile = randomSpawnTile;
        }
        GameManager.Instance.ChangeState(GameState.SpawnEnemies);
    }

    public void SpawnEnemies(int count = 1)
    {
        enemyUnitCount = count;
        for (int i = 0; i < enemyUnitCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
            var spawnedEnemy = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();

            randomSpawnTile.setUnit(spawnedEnemy);

            enemiesSpawned.Add(spawnedEnemy);
            enemyTiles.Add(randomSpawnTile);

            //New section added to help with NPC_Controller
            /*Tile currentPlayerTile = GridManager.Instance.GetTileAtPosition(SelectedPlayer.transform.position);

            var npcController = spawnedEnemy.GetComponent<NPC_Controller>();
            if (npcController != null && SelectedPlayer != null)
            {
               npcController.SetTarget(randomSpawnTile, currentPlayerTile);
            }*/
        }
        GameManager.Instance.ChangeState(GameState.PlayerTurn);
    }



    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)units.Where(u=>u.Faction == faction).OrderBy(o=>Random.value).First().UnitPrefab;
    }
    public void SetSelectedPlayer(BasePlayer player)
    {
        if (SelectedPlayer != null && SelectedPlayer != player)
        {
            // Deselect current player and hide move range highlights
            SelectedPlayer.GetTilesInMoveRange().ForEach(t => t.ShowHighlight(false));
        }
        SelectedPlayer = player;
    }

    //New.
    public void BeginEnemyTurn()
    {
        Debug.Log("BeginEnemyTurn: SelectedPlayer = " + SelectedPlayer);


        Tile currentPlayerTile = GridManager.Instance.GetTileForUnit(SelectedPlayer.gameObject);
        Debug.Log("Player is at tile: " + currentPlayerTile?.name);

        for (int i = 0; i < enemiesSpawned.Count; i++)
        {
            var enemy = enemiesSpawned[i];
            var npcController = enemy.GetComponent<NPC_Controller>();
            if (npcController != null && SelectedPlayer != null)
            {
                npcController.BeginTurn();
               

                Tile enemyTile = GridManager.Instance.GetTileForUnit(enemy.gameObject);
                Debug.Log($"Enemy {enemy.name} at {enemyTile?.name}, chasing {currentPlayerTile?.name}");
                npcController.SetTarget(enemyTile);
                Debug.Log($"EnemyTurn started. Enemy {enemy.name} moving from {enemyTile.name} to {currentPlayerTile.name}");

            }
        }

        //StartCoroutine(CheckEnemiesFinished());
        
    }

  /* private IEnumerator CheckEnemiesFinished()
    {
        bool allDone = false;
        while (!allDone)
        {
            allDone = true;
            foreach (var enemy in enemiesSpawned)
            {
                var npcController = enemy.GetComponent<NPC_Controller>();
                if (npcController != null && !npcController.HasFinishedTurn)
                {
                    allDone = false;
                    break;
                }
            }
            yield return null;
        }
        GameManager.Instance.ChangeState(GameState.PlayerTurn);
    }*/
   
}
