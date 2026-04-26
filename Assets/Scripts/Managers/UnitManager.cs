using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

//Developer: Ivan Ching
//Edits from Andrew Shelton
//Aggregated from multiple tutorials

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    [SerializeField] private List<ScriptableUnit> playersToSpawn = new List<ScriptableUnit>();
    [SerializeField] private List<ScriptableUnit> enemiesToSpawn = new List<ScriptableUnit>();

    //Adding reference to player tile.
    private Tile playerTile;
    public bool targeting = false;
    public Tile targetedTile, previousTile; 
    private List<Tile> enemyTiles = new List<Tile>();

    public BaseUnit SelectedUnit;
    public UnitSelector selector;

    public List<BasePlayer> playersSpawned = new List<BasePlayer>();
    public List<BaseEnemy> enemiesSpawned = new List<BaseEnemy>();

    public int enemyUnitCount;
    public int playerUnitCount;

    public static float boostHinderValue = 0.25f;
    public static float strengthenWeakenValue = 0.25f;
    public static float resistantVulnerableValue = 0.25f;
    public static float invisibleAttackBoost = 0.15f;
    public static float frozenDefenseDown = 0.1f;
    public static float poisonAttackDown = 0.1f;
    public static float maxDodgeChance = 0.85f;
    public static float guardEfficiency = 0.90f;
    public static float reflectEfficiency = 0.75f;
    public static float absorbEfficiency = 0.5f;
    public static int backstabInvisibleBonus = 15;
    public static int maxRegenStacks = 5;
    //public static int 

    public List<ScriptableUnit> MB;
    public List<ScriptableUnit> WB;
    public List<ScriptableUnit> FB;
    public List<ScriptableUnit> MU;
    public List<ScriptableUnit> WU;
    public List<ScriptableUnit> FU;


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

        switch(DeckManager.instance.DEBUGTEAMSELECTOR)
        {
            case 0:
                playersToSpawn = MB;
                break;

            case 1:
                playersToSpawn = WB;
                break;

            case 2:
                playersToSpawn = FB;
                break;

            case 3:
                playersToSpawn = MU;
                break;

            case 4:
                playersToSpawn = WU;
                break;

            case 5:
                playersToSpawn = FU;
                break;

            default:
                playersToSpawn = ArmyManager.Instance.unitsInArmy;
                break;
        }
    }

    private void Update()
    {
        if (targeting && !PauseMenu.instance.blocker.activeInHierarchy)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            int range = 0;

            if (hit.collider != null)
            {
                GameObject objectOver = hit.collider.gameObject;
                if (previousTile != targetedTile)
                {
                    previousTile = targetedTile;
                }
                
                targetedTile = objectOver.GetComponent<Tile>();
                if (targetedTile != null)
                {
                    if (CardManager.instance.selectedCard.cardType == Type.Attack)
                    {
                        range = CardManager.instance.selectedCard.areaRange;

                        if (previousTile != null)
                        {
                            foreach (Tile t in SelectedPlayer.GetTilesInAOEAttackRange(previousTile, range))
                            {
                                t.ShowHighlight(false, Tile.nonwalkableColor);
                            }
                        }

                        foreach (Tile t in SelectedPlayer.GetTilesInAttackRange())
                        {
                            if (t.OccupiedUnit == null || t.OccupiedUnit.Faction == Faction.Enemy)t.ShowHighlight(true, Tile.targetableColor);
                        }

                    } else if (CardManager.instance.selectedCard.cardType == Type.Support)
                    {
                        range = CardManager.instance.selectedCard.range;

                        if (previousTile != null)
                        {
                            previousTile.ShowHighlight(false, Tile.nonwalkableColor);
                        }

                        if (range == 0)
                        {
                            SelectedPlayer.OccupiedTile.ShowHighlight(true, Tile.targetableColor);
                        } else
                        {
                            foreach (Tile t in SelectedPlayer.GetTilesInAttackRange())
                            {
                                if (t.OccupiedUnit == null || t.OccupiedUnit.Faction == Faction.Player)t.ShowHighlight(true, Tile.targetableColor);
                            }
                        }
                        
                    } else if (CardManager.instance.selectedCard.cardType == Type.Movement)
                    {
                        range = CardManager.instance.selectedCard.range;

                        if (SelectedPlayer.moveRange == 0)
                        {
                            SelectedPlayer.moveRange = range;

                            if (previousTile != null)
                            {
                                previousTile.ShowHighlight(false, Tile.nonwalkableColor);
                            }

                            if (CardManager.instance.selectedCard.rangeType != RangeType.FloodMovementUnrestricted)
                            {
                                foreach (Tile t in SelectedPlayer.GetTilesInMoveRange())
                                {
                                    t.ShowHighlight(true, Tile.targetableColor);
                                }
                            } else
                            {
                                foreach (Tile t in SelectedPlayer.GetTilesInUnrestrictedMoveRange())
                                {
                                    if (t.Walkable)t.ShowHighlight(true, Tile.targetableColor);
                                }
                            }

                            
                            SelectedPlayer.OccupiedTile.ShowHighlight(false, Tile.nonwalkableColor);

                            SelectedPlayer.moveRange = 0;
                        } else
                        {
                            if (previousTile != null)
                            {
                                previousTile.ShowHighlight(false, Tile.nonwalkableColor);
                            }

                            if (CardManager.instance.selectedCard.rangeType != RangeType.FloodMovementUnrestricted)
                            {
                                foreach (Tile t in SelectedPlayer.GetTilesInMoveRange())
                                {
                                    t.ShowHighlight(true, Tile.targetableColor);
                                }
                            } else
                            {
                                foreach (Tile t in SelectedPlayer.GetTilesInUnrestrictedMoveRange())
                                {
                                    if (t.Walkable)t.ShowHighlight(true, Tile.targetableColor);
                                }
                            }
                        }
                        
                    } else if (CardManager.instance.selectedCard.cardType == Type.Summon)
                    {
                        range = CardManager.instance.selectedCard.range;

                        if (previousTile != null)
                        {
                            previousTile.ShowHighlight(false, Tile.nonwalkableColor);
                        }

                        foreach (Tile t in SelectedPlayer.GetTilesInMoveRange())
                        {
                            t.ShowHighlight(true, Tile.targetableColor);
                        }
                    }
                
                    if (CardManager.instance.selectedCard.cardType == Type.Attack)
                    {
                       foreach (Tile t in SelectedPlayer.GetTilesInAttackRange())
                        {
                            if (targetedTile == t)
                            {
                                foreach (Tile p in SelectedPlayer.GetTilesInAOEAttackRange(targetedTile, range))
                                {
                                    if (p.OccupiedUnit == null || p.OccupiedUnit.Faction == Faction.Enemy)p.ShowHighlight(true, Tile.attackableColor);
                                }
                                if (t.OccupiedUnit == null || t.OccupiedUnit.Faction == Faction.Enemy)t.ShowHighlight(true, Tile.attackableColor);
                                break;
                            }
                        } 
                    } else if (CardManager.instance.selectedCard.cardType == Type.Support)
                    {
                        foreach (Tile t in SelectedPlayer.GetTilesInAttackRange())
                        {
                            if (targetedTile == t)
                            {
                                if (t.OccupiedUnit == null || t.OccupiedUnit.Faction == Faction.Player)t.ShowHighlight(true, Tile.supportableColor);
                                break;
                            }
                        } 

                        if (targetedTile == SelectedPlayer.OccupiedTile)
                        {
                            SelectedPlayer.OccupiedTile.ShowHighlight(true, Tile.supportableColor);
                        }

                    } else if (CardManager.instance.selectedCard.cardType == Type.Movement)
                    {
                        if (CardManager.instance.selectedCard.rangeType != RangeType.FloodMovementUnrestricted) {
                            foreach (Tile t in SelectedPlayer.GetTilesInMoveRange())
                            {
                                if (targetedTile == t)
                                {
                                    List<Tile> path = AStarManager.Instance.GeneratePath(SelectedPlayer.OccupiedTile, t);
                                    foreach (Tile p in path)
                                    {
                                        p.ShowHighlight(true, Tile.walkableColor);
                                        if (SelectedPlayer.OccupiedTile == p)
                                        {
                                            p.ShowHighlight(true, Tile.walkableColor);
                                        }
                                    }
                                    t.ShowHighlight(true, Tile.walkableColor);
                                    break;
                                }
                                SelectedPlayer.OccupiedTile.ShowHighlight(false, Tile.nonwalkableColor);
                            } 
                        } else
                        {
                            foreach (Tile t in SelectedPlayer.GetTilesInUnrestrictedMoveRange())
                            {
                                if (targetedTile == t)
                                {
                                    if (t.Walkable)t.ShowHighlight(true, Tile.walkableColor);
                                    break;
                                }
                            } 
                        }

                    } else if (CardManager.instance.selectedCard.cardType == Type.Summon)
                    {
                        foreach (Tile t in SelectedPlayer.GetTilesInMoveRange())
                        {
                            if (targetedTile == t)
                            {
                                t.ShowHighlight(true, Tile.summonableColor);
                                break;
                            }
                        }
                    }

                } else
                {
                    if (CardManager.instance.selectedCard.cardType == Type.Attack)
                    {
                        if (previousTile != null)
                        {
                            foreach (Tile t in SelectedPlayer.GetTilesInAOEAttackRange(previousTile, range))
                            {
                                t.ShowHighlight(false, Tile.nonwalkableColor);
                            }
                        }
                    }
                }
            }
        } 
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
                spawnedPlayer.name = unitData.name;
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

        StageData data = CurrentSession.ActiveStageData;
        if(data == null || data.enemies == null || data.enemies.Count == 0)
        {
            Debug.LogError("UnitManager: No StageData or Enemy prefabs on this stage");
            GameManager.Instance.ChangeState(GameState.PlayerTurn);
            return;
        }

        int countToSpawn = data.enemyCount;

        for (int i = 0; i < countToSpawn; i++)
        {
            GameObject prefab = data.enemies[i % data.enemies.Count];
            //var unitData = enemiesToSpawn[i];
            var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();
            if(randomSpawnTile != null)
            {
                BaseEnemy spawnedEnemy = Instantiate(prefab.GetComponent<BaseEnemy>(), randomSpawnTile.transform.position, Quaternion.identity);
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
                int totalRange = SelectedEnemy.attackRange + SelectedEnemy.moveRange + SelectedEnemy.moveModifier;
                RangeManager.GetTilesInRange(SelectedEnemy.OccupiedTile, totalRange).ForEach(t => t.ShowHighlight(false, Tile.nonwalkableColor));
            }

            SelectedUnit.GetTilesInMoveRange().ForEach(t => t.ShowHighlight(false, Tile.nonwalkableColor));
            SelectedUnit.GetTilesInAttackRange().ForEach(t => t.ShowHighlight(false, Tile.nonwalkableColor));
            SelectedUnit.OccupiedTile.ShowHighlight(false, Tile.nonwalkableColor);

        }
        SelectedUnit = unit;

        selector.PlaceOnUnit(SelectedUnit);
        UnitInfo.Instance.UpdatePanel();
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
        // int moveRange = NPC_Controller.Instance.tilesPerMove;
        // TODO: enemies should be initialized with correct movement range
        int moveRange = SelectedEnemy.moveRange + SelectedEnemy.moveModifier;

        if (SelectedEnemy.restricted > 0)
        {
            moveRange = 0;
        }

        if ((int)SelectedEnemy.stunned == 1)
        {
            return;
        }

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

        List<BaseEnemy> enemies = enemiesSpawned;

        for (int i = 1; i <= enemyUnitCount; i++)
        {
            int initial = enemyUnitCount;
            enemies[enemyUnitCount - i].ResetValues();
            if (initial > enemyUnitCount)
            {
                i--;
            }

        }

        foreach (var enemy in enemies)
        {
            if (enemy == null || enemy.gameObject == null) continue;

            var npc = enemy.GetComponent<NPC_Controller>();
            if (npc != null) {
                if ((int)enemy.stunned >= 1)
                {
                    continue;
                }    

                enemyControllers.Add(npc);
            }
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

    public void ApplyEndTurnEffects(Faction faction)
    {
        if (faction == Faction.Player)
        {
            for (int i = 1; i <= playerUnitCount; i++)
            {
                int initial = playerUnitCount;
                playersSpawned[playerUnitCount - i].ApplyEndTurnEffects();
                if (initial > playerUnitCount)
                {
                    i--;
                }

            }
        } else if (faction == Faction.Enemy)
        {
            for (int i = 1; i <= enemyUnitCount; i++)
            {
                int initial = enemyUnitCount;
                enemiesSpawned[enemyUnitCount - i].ApplyEndTurnEffects();
                if (initial > enemyUnitCount)
                {
                    i--;
                }

            }
        }
    }
}