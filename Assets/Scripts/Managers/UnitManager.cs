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

    [SerializeField] private List<ScriptableUnit> playersToSpawn = new List<ScriptableUnit>();
    [SerializeField] private List<ScriptableUnit> enemiesToSpawn = new List<ScriptableUnit>();

    private List<ScriptableUnit> units;
    //Adding reference to player tile.
    private Tile playerTile;
    private List<Tile> enemyTiles = new List<Tile>();

    public BasePlayer SelectedPlayer;
    public List<BasePlayer> playersSpawned = new List<BasePlayer>();
    public List<BaseEnemy> enemiesSpawned = new List<BaseEnemy>();

    public int enemyUnitCount;
    public int playerUnitCount;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnPlayers()
    {
        playersSpawned.Clear();

        for (int i = 0; i < playersToSpawn.Count; i++)
        {
            var unitData = playersToSpawn[i];
            var randomSpawnTile = GridManager.Instance.GetPlayerSpawnTile();

            if (randomSpawnTile != null)
            {
                BasePlayer spawnedPlayer = Instantiate((BasePlayer)unitData.UnitPrefab, randomSpawnTile.transform.position, Quaternion.identity);
                spawnedPlayer.name = $"DEBUG_Player_{i}_{randomSpawnTile.Position.x}_{randomSpawnTile.Position.y}";
                Debug.Log($"Found Tile for player{i}");
                randomSpawnTile.setUnit(spawnedPlayer);
                playersSpawned.Add(spawnedPlayer);
                //Debug.Log($"added to list:{i}");  
                if (i == 0) SelectedPlayer = spawnedPlayer;
                if (randomSpawnTile.OccupiedUnit == null)
                {
                    Debug.Log("Tiles failed to hold unit" + randomSpawnTile.Position);
                }
                Debug.Log($"Spawned Player {i}: {spawnedPlayer.name}s at {randomSpawnTile.Position}");
            }
            
            Debug.Log("Spawned Player: " + SelectedPlayer.name);

            //Adding reference to player tile.
            playerTile = randomSpawnTile;
            var foundInScene = GameObject.FindObjectsByType<BasePlayer>(FindObjectsSortMode.None);
            Debug.Log($"[FINAL CHECK] Hierarchy physically contains {foundInScene.Length} player objects.");

            foreach (var p in foundInScene)
            {
                Debug.Log($"Found {p.name} - Active: {p.gameObject.activeInHierarchy} - Parent: {(p.transform.parent != null ? p.transform.parent.name : "None")}");
            }

        }
        playerUnitCount = playersSpawned.Count();
        Debug.Log($"Spawned {playersSpawned.Count} players total.");
        GameManager.Instance.ChangeState(GameState.SpawnEnemies);
    }

    public void SpawnEnemies()
    {
        enemiesSpawned.Clear();
        enemyTiles.Clear();
        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            var unitData = enemiesToSpawn[i];
            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();
            if(randomSpawnTile != null)
            {
                BaseEnemy spawnedEnemy = Instantiate((BaseEnemy)unitData.UnitPrefab, randomSpawnTile.transform.position, Quaternion.identity);
                spawnedEnemy.name = $"Enemy_{i}";
                randomSpawnTile.setUnit(spawnedEnemy);
                enemiesSpawned.Add(spawnedEnemy);
                enemyTiles.Add(randomSpawnTile);
            }

            
        }
        enemyUnitCount = enemiesSpawned.Count();
        GameManager.Instance.ChangeState(GameState.PlayerTurn);
    }



    //private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    //{
    //    return (T)units.Where(u => u.Faction == faction).OrderBy(o => Random.value).First().UnitPrefab;
    //}

    public void SetSelectedPlayer(BasePlayer player)
    {
        if (SelectedPlayer != null && SelectedPlayer != player)
        {
            // Deselect current player and hide move range highlights
            SelectedPlayer.GetTilesInMoveRange().ForEach(t => t.ShowHighlight(false, Tile.nonwalkableColor));
            SelectedPlayer.GetTilesInAttackRange().ForEach(t => t.ShowHighlight(false, Tile.nonwalkableColor));
            SelectedPlayer.OccupiedTile.ShowHighlight(false, Tile.nonwalkableColor);
        }
        SelectedPlayer = player;

        if (SelectedPlayer != null)
        {
            Debug.Log("Selected Player: " + SelectedPlayer.name);
            // Show move range highlights for the newly selected player
            SelectedPlayer.GetTilesInMoveRange().ForEach(t => t.ShowHighlight(true, Tile.walkableColor));
            if (SelectedPlayer.moveRange > 0) SelectedPlayer.OccupiedTile.ShowHighlight(true, Tile.walkableColor);

            if (SelectedPlayer.canAttack)
            {
                GridManager.Instance.GetNeighborsOf(SelectedPlayer.OccupiedTile).ForEach(t => t.ShowHighlight(true, Tile.attackableColor));
                if (SelectedPlayer.OccupiedTile.IsNextToEnemy()) combatUIManager.Instance.showCombatOption(SelectedPlayer, null);
            }
        }
    }

    public void BeginEnemyTurn()
    {
        EnemyTargetingManager.TargetCounts.Clear();

        Debug.Log("BeginEnemyTurn: SelectedPlayer = " + SelectedPlayer);


        Tile currentPlayerTile = GridManager.Instance.GetTileForUnit(SelectedPlayer.gameObject);
        Debug.Log("Player is at tile: " + currentPlayerTile?.name);

        for (int i = 0; i < enemiesSpawned.Count; i++)
        {
            //NEW: Check if enemy is null or destroyed before trying to access it
            var enemy = enemiesSpawned[i];
            if (enemy == null || enemy.gameObject == null)
            {
                enemiesSpawned.RemoveAt(i);
                continue;
            }
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

    }
}