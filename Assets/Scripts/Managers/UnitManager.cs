using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    private List<ScriptableUnit> units;
    //Adding reference to player tile.
    private Tile playerTile;
    private List<Tile> enemyTiles = new List<Tile>();

    public BasePlayer SelectedPlayer;

    private List<BaseEnemy> enemiesSpawned = new List<BaseEnemy>();

    private void Awake()
    {
        Instance = this;

        units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
    }

    public void SpawnPlayers()
    {
        var playerUnitCount = 1;
        for (int i = 0; i < playerUnitCount; i++)
        {
            var randomPrefab = GetRandomUnit<BasePlayer>(Faction.Player);
            var spawnedPlayer = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetPlayerSpawnTile();

            randomSpawnTile.setUnit(spawnedPlayer);

            SelectedPlayer = spawnedPlayer;
            //Adding reference to player tile.
            playerTile = randomSpawnTile;
        }
        GameManager.Instance.ChangeState(GameState.SpawnEnemies);
    }

    public void SpawnEnemies()
    {
        var enemyCount = 1;
        for (int i = 0; i < enemyCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
            var spawnedEnemy = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();

            randomSpawnTile.setUnit(spawnedEnemy);

            enemiesSpawned.Add(spawnedEnemy);
            enemyTiles.Add(randomSpawnTile);

            //New section added to help with NPC_Controller
            var npcController = spawnedEnemy.GetComponent<NPC_Controller>();
            if (npcController != null && SelectedPlayer != null)
            {
                npcController.SetTarget(randomSpawnTile, playerTile);
            }
        }
        GameManager.Instance.ChangeState(GameState.PlayerTurn);
    }



    private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
    {
        return (T)units.Where(u=>u.Faction == faction).OrderBy(o=>Random.value).First().UnitPrefab;
    }
    public void SetSelectedPlayer(BasePlayer player)
    {
        SelectedPlayer = player;
    }

    //New.
    public void BeginEnemyTurn()
    {
        Tile currentPlayerTile = GridManager.Instance.GetTileAtPosition(SelectedPlayer.transform.position);
        for (int i = 0; i < enemiesSpawned.Count; i++)
        {
            var enemy = enemiesSpawned[i];
            var npcController = enemy.GetComponent<NPC_Controller>();
            if (npcController != null && SelectedPlayer != null)
            {
                npcController.BeginTurn();
                Tile enemyTile = GridManager.Instance.GetTileAtPosition(enemy.transform.position);
                npcController.SetTarget(enemyTiles[i], currentPlayerTile);
            }
        }
    }
   
}
