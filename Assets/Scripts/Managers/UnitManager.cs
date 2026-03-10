using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

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

    public BaseUnit SelectedUnit;
    //public GameObject selector;

    public List<BasePlayer> playersSpawned = new List<BasePlayer>();
    public List<BaseEnemy> enemiesSpawned = new List<BaseEnemy>();

    public int enemyUnitCount;
    public int playerUnitCount;

    public BasePlayer SelectedPlayer
    {
        get => SelectedUnit as BasePlayer;
        set => SelectedUnit = value;
    }

    public BaseEnemy SelectedEnemy
    {
        get => SelectedUnit as BaseEnemy;
        set => SelectedUnit = value;
    }

    private void Awake()
    {
        Instance = this;
    }


    //Party Function for future purposes
    public void AddUnitToParty(ScriptableUnit unit)
    {
        playersToSpawn.Add(unit);
    }
    public void RemoveUnitFromParty(ScriptableUnit unit)
    {
        playersToSpawn.Remove(unit);
    }
    public void SetNumberEnemies(List<ScriptableUnit> wave)
    {
        enemiesToSpawn = wave;
    }


    //Spawns players on random spaces
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


    //spawns enemies on random spaces
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


    private void SetSelectedUnit(BaseUnit unit)
    {
        if (SelectedUnit != null && SelectedUnit != unit)
        {
            // Deselect current unit and hide move range highlights
            if (SelectedEnemy != null)
            {
                int totalRange = SelectedEnemy.attackRange + NPC_Controller.Instance.tilesPerMove;
                RangeManager.GetTilesInRange(SelectedEnemy.OccupiedTile, totalRange).ForEach(t => t.ShowHighlight(false, Tile.nonwalkableColor));
            }

            SelectedUnit.GetTilesInMoveRange().ForEach(t => t.ShowHighlight(false, Tile.nonwalkableColor));
            SelectedUnit.GetTilesInAttackRange().ForEach(t => t.ShowHighlight(false, Tile.nonwalkableColor));
            SelectedUnit.OccupiedTile.ShowHighlight(false, Tile.nonwalkableColor);
        }

        SelectedUnit = unit;

        //if (SelectedUnit == null)
        //{
        //    selector.SetActive(false);
        //}
        //else
        //{
        //    selector.SetActive(true);
        //    selector.transform.position = SelectedUnit.OccupiedTile.Position;
        //    selector.transform.position += Vector3.back;
        //}
    }

    public void SetSelectedPlayer(BasePlayer player)
    {
        SetSelectedUnit(player);
        CardManager.instance.SetSelectedPlayer(SelectedPlayer);

        if (SelectedPlayer == null)
        {
            Debug.Log("UnitManager selected null player");
            return;
        }
        
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

    public void SetSelectedEnemy(BaseEnemy enemy)
    {
        SetSelectedUnit(enemy);
        CardManager.instance.SetSelectedPlayer(null);

        if (SelectedEnemy == null)
        {
            Debug.Log("UnitManager selected null enemy");
            return;
        }

        int attackRange = SelectedEnemy.attackRange;
        int moveRange = NPC_Controller.Instance.tilesPerMove;
        // TODO: enemies should be initialized with correct movement range
        //int moveRange = SelectedEnemy.moveRange;

        Debug.Log("Selected Enemy: " + SelectedEnemy.name);

        // Show move and attack range highlights for enemy unit
        int totalRange = attackRange + moveRange;

        if (totalRange > 0)
        {
            RangeManager.GetTilesInRange(SelectedEnemy.OccupiedTile, totalRange).ForEach(t => t.ShowHighlight(true, Tile.attackableColor));
        }

        if (moveRange > 0)
        {
            RangeManager.GetTilesInRange(SelectedEnemy.OccupiedTile, moveRange).ForEach(t => t.ShowHighlight(false, Tile.walkableColor));
            RangeManager.GetTilesInRange(SelectedEnemy.OccupiedTile, moveRange).ForEach(t => t.ShowHighlight(true, Tile.walkableColor));
        }
    }

    public void BeginEnemyTurn()
    {
        //Added to help with the enemy targeting null after player object is destroyed

        if (SelectedPlayer == null || SelectedPlayer.gameObject == null)
        {
            playersSpawned.RemoveAll(p => p == null || p.gameObject == null);

            if (playersSpawned.Count == 0)
            {
                GameManager.Instance.ChangeState(GameState.EndScreen);
                return;
            }

            SelectedPlayer = playersSpawned[0];
        }



        EnemyTargetingManager.TargetCounts.Clear();

        Debug.Log("BeginEnemyTurn: SelectedPlayer = " + SelectedPlayer);


        Tile currentPlayerTile = GridManager.Instance.GetTileForUnit(SelectedPlayer.gameObject);
        Debug.Log("Player is at tile: " + currentPlayerTile?.name);

        //Just adding this for testing purposes

        List<NPC_Controller> enemyControllers = new List<NPC_Controller>();

        foreach (var enemy in enemiesSpawned)
        {
            if (enemy == null || enemy.gameObject == null) continue;

            var npc = enemy.GetComponent<NPC_Controller>();
            if (npc != null)
                enemyControllers.Add(npc);

        }

        StartCoroutine(NPC_Controller.RunEnemyTurn(enemyControllers));

        /*for (int i = 0; i < enemiesSpawned.Count; i++)
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
        }*/

    }
}