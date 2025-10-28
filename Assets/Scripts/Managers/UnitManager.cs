using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    private List<ScriptableUnit> units;

    public BasePlayer SelectedPlayer;

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

}
