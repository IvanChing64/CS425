using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;
//Developer: Andrew Shelton
//Edits from Ivan Ching
//Aggregated from multiple tutorials and sources


public class NPC_Controller: MonoBehaviour
{
    //Added TurnState to begin enum turn stuff: Andrew Shelton
    private enum TurnState
    {
        Idle,
        BeginTurn,
        Targeting,
        Moving,
        Acting,
        EndTurn
    }

    enum BossState
    { 
        Invulnerable,
        Vulnerable
    }
    BossState occupiedState = BossState.Invulnerable;

    int minionsAlive = 3;
    float healthPenaltyPerMinion = 100;


    private TurnState currentState = TurnState.Idle;

    public static NPC_Controller Instance;
    public float moveSpeed = 0.5f;
    public int tilesPerMove = 4;

    public List<Tile> path;
    private int pathIndex;
    private int tilesMovedThisTurn;
    private bool isMoving;
    //private bool hasSummonedThisTurn = false;

    public bool isEliteVarient = false;
    public bool isEnraged = false;

    private BaseUnit npcUnit;

    private Enemy1 enemy1;
    private Animator unitAnimator;

    

    private void Awake()
    {
        enemy1 = GetComponent<Enemy1>();
        Instance = this;
        npcUnit = GetComponent<BaseUnit>();
        unitAnimator = GetComponent<Animator>();
        tilesPerMove = npcUnit.moveRange + npcUnit.moveModifier;; 
    }

    //MovementBehavior logic: Andrew Shelton

    /*public override void TakeDamage(float amount)
    {
        if (occupiedState == BossState.Invulnerable)
        {
            Debug.Log("Boss takes 0 damage!");
            return;
        }
        base.TakeDamage(amount);
    }*/

    // Stuff to help boss
    public void OnMinionDied()
    {
        minionsAlive--;

        //Reduce boss max health or health
        float damage = npcUnit.maxHealth - healthPenaltyPerMinion;
        npcUnit.health -= damage;

        Debug.Log($"Boss takes {damage} damage from minion death");

        if (minionsAlive <= 0)
        {
            occupiedState = BossState.Vulnerable;
            Debug.Log("Minions destroyed! Boss is now vulnerable!");
        }
    }
    // Added Enrage stuff. Not implemented yet.
    void ConquestEnrageStats()
    {
        isEnraged = true;
        npcUnit.ApplyConquestStats();
    }

    public void UpdateEnrageState()
    {
        Debug.Log($"[ENRAGE CHECK] {name} | Elite: {isEliteVarient}");


        if (!isEliteVarient) return;

        if (isEnraged) return;

        float healthPercent = (float)npcUnit.health / npcUnit.maxHealth;

        Debug.Log($"[ENRAGE HP] {healthPercent}");


        if (healthPercent <= 0.5f)
        {
            isEnraged = true;
            ConquestEnrageStats();
            Debug.Log("Enraged!");
        }
    }


  

    private Tile GetRangedTarget()
    {
        Debug.Log($"[RANGED TARGET CALLED] {name}");
        var targeting = GetComponent<EnemyTargetingManager>();
        targeting.SelectTarget();

        Tile playerTile = GridManager.Instance.GetTileForUnit(targeting.CurrentTarget.gameObject);

        List<Tile> movableTiles = RangeManager.GetTilesInRange(npcUnit.OccupiedTile,
            npcUnit.moveRange, RangeType.FloodTargeting);

        Tile bestTile = null;
        float bestScore = -Mathf.Infinity;

       

        // alreadyInRange = movableTiles.Contains(playerTile);

        foreach (var tile in movableTiles)
        {
            if (!tile.isWalkable) continue;

            float distToPlayer = Vector2.Distance(
                tile.transform.position,
                playerTile.transform.position
            );

            float distFromEnemy = Vector2.Distance(tile.transform.position, npcUnit.OccupiedTile.transform.position);

            if (distToPlayer > npcUnit.attackRange)
               continue;

            float score = distFromEnemy;

            if (score > bestScore)
            {
                bestScore = score;
                bestTile = tile;
            }

        }
        if (bestTile != null)
        {
            return bestTile;
        }
        //Pass 2: 

        /*bestScore = Mathf.Infinity;

       foreach (var tile in movableTiles) 
       {
            if (!tile.isWalkable) { continue; }
            if (tile == npcUnit.OccupiedTile) continue;

            //var testPath = AStarManager.Instance.GeneratePath(npcUnit.OccupiedTile, tile);
             //if (testPath == null || testPath.Count == 0) continue;

            var pathFromTileToPlayer = AStarManager.Instance.GeneratePath(tile, playerTile);

            if (pathFromTileToPlayer == null || pathFromTileToPlayer.Count == 0)
                continue;

            // combine path cost + distance
            float score = pathFromTileToPlayer.Count;

            if (score < bestScore)
             {
                bestScore = score;
                bestTile = tile;
             }

       }*/

       if (bestTile == null)
        {
            return GetRandomTile();
        }

        var debugPath = AStarManager.Instance.GeneratePath(npcUnit.OccupiedTile, bestTile);
        Debug.Log($"[TARGET] BestTile: {bestTile?.name}, PathLen: {debugPath?.Count}");
        Debug.Log($"Chosen tile: {bestTile?.name}");
        return bestTile;
    }

    public void DecrementCurrentSummons()
    {
        currentSummons = Mathf.Max(0, currentSummons - 1);
        Debug.Log($"[DECREMENT] currentSummons now: {currentSummons}");

    
    }

    [SerializeField] private int healAmount = 10;

    private bool CanSummonThisTurn()
    {
        Debug.Log($"[CHECK] CanSummon? Turn: {GameManager.Instance.turnNumber}");

        return GameManager.Instance.turnNumber % 2 == 0
        && currentSummons < maxSummons;

    }

    [SerializeField]
    private GameObject enemyPrefab;


    [SerializeField] private int maxSummons = 6;
    [SerializeField] private int summonsPerCast = 2;
    //[SerializeField] private int summonCooldownTurns = 2;

    //private int nextSummonTurn = 0;
    private int currentSummons = 0;

    public void SpawnEnemyAtTile(Tile tile)
    {
        GameObject enemy = Instantiate(enemyPrefab);

        enemy.transform.position = tile.transform.position;

        BaseUnit unit = enemy.GetComponent<BaseUnit>();
        
        unit.OccupiedTile = tile;
        unit.summoner = this;
        unit.isSummoned = true;
        tile.OccupiedUnit = unit;

        var baseEnemy = enemy.GetComponent<BaseEnemy>();
        if (baseEnemy != null && UnitManager.Instance != null)
        {
            UnitManager.Instance.enemiesSpawned.Add(baseEnemy);

            UnitManager.Instance.enemyUnitCount = UnitManager.Instance.enemiesSpawned.Count;
        }
    }

    private bool SummonGrunts()
    {
        if (currentSummons >= maxSummons) return false;

        // spawn 2 grunts near the support
        var neighbors = npcUnit.OccupiedTile.Neighbors;
        if (neighbors == null || neighbors.Count == 0) return false;

        int spawned = 0;

        foreach (Tile tile in neighbors)
        {
            if (tile.OccupiedUnit != null || !tile.isWalkable) continue;


            SpawnEnemyAtTile(tile);

            currentSummons++;
            spawned++;
            
            if (spawned >= summonsPerCast || currentSummons >= maxSummons)
                break;
        }
        if (spawned > 0)
        {
            

            Debug.Log($"[SUMMON] Spawned {spawned}. Total: {currentSummons}");
            return true;
        }
        return false;
    }
    private Tile GetSupportTarget()
    {
        Debug.Log($"Summons: {currentSummons}, Turn: {GameManager.Instance.turnNumber}");

        bool isElite = isEliteVarient;
        if (isElite && CanSummonThisTurn() && currentSummons < maxSummons)
        {
            bool didSummon = SummonGrunts();

            if (didSummon)
            {
                //hasSummonedThisTurn = true;
                return npcUnit.OccupiedTile;
            }
            return GetRandomTile();
        }
        BaseUnit lowestHealthAlly = null;
        float lowestHealth = Mathf.Infinity;


        foreach (var unit in UnitManager.Instance.enemiesSpawned)
        {
            if (unit == npcUnit) continue;

            if (unit.Faction != npcUnit.Faction) continue;

            if (unit.health >= unit.maxHealth) continue;

            if (unit.health < lowestHealth)
            {
                lowestHealth = unit.health;
                lowestHealthAlly = unit;

            }

        }

        if (lowestHealthAlly == null)
            return GetRandomTile();

        Tile allyTile = lowestHealthAlly.OccupiedTile;

        bool inHealRange = RangeManager.GetTilesInRange(npcUnit.OccupiedTile, npcUnit.attackRange, RangeType.FloodTargeting).Contains(allyTile);

        if (inHealRange)
        {
            HealTarget(lowestHealthAlly, healAmount);
        }
        //New Things. Set back to return allytile if not working.

        List<Tile> movableTiles = RangeManager.GetTilesInRange(npcUnit.OccupiedTile, npcUnit.moveRange, RangeType.FloodTargeting);

        Tile bestTile = null;
        float bestScore = Mathf.Infinity;

        foreach (var tile in movableTiles)
        {
            if (!tile.isWalkable) continue;

            float distToAlly = Vector2.Distance(tile.transform.position,
                allyTile.transform.position);

            // Check if this tile would allow healing the ally
            bool canHealFromTile = distToAlly <= npcUnit.attackRange;

            if (!canHealFromTile) continue;
          


            float distToSelf = Vector2.Distance(tile.transform.position, npcUnit.OccupiedTile.transform.position);

            float score = distToAlly + (distToSelf * 0.1f);

            if (score < bestScore)
            {
                bestScore = score;
                bestTile = tile;
            }
        }
        if (bestTile != null)
        {
            return bestTile;
        }
        return allyTile;

    }
   

    private Tile GetRandomTile()
    {
        var movableTiles = RangeManager.GetTilesInRange(npcUnit.OccupiedTile,
        npcUnit.moveRange, RangeType.FloodTargeting);

        var validTiles = new List<Tile>();

        foreach (var tile in movableTiles)
        {
            if (!tile.isWalkable) continue;
            if (tile == npcUnit.OccupiedTile) continue;

            var path = AStarManager.Instance.GeneratePath(npcUnit.OccupiedTile, tile);
            if (path == null || path.Count == 0) continue;

            validTiles.Add(tile);
        }

        if (validTiles.Count == 0) return npcUnit.OccupiedTile;

        int index = UnityEngine.Random.Range(0, validTiles.Count);
        return validTiles[index];
    }

    private void HealTarget(BaseUnit target, int healAmount)
    {
        if (target == null) return;

        target.health = Mathf.Min(target.health + healAmount, target.maxHealth);

        target.UpdateHealth();
    }

    private Tile GetClosestReachablePlayerTile(Tile startTile)
    {
        BaseUnit bestTarget = null;
        float closestDist = Mathf.Infinity;

        foreach (var unit in UnitManager.Instance.playersSpawned)
        {
            if (unit == null) continue;
            if (unit.invisible != 0) continue;

            Tile targetTile = unit.OccupiedTile;
            if (targetTile == null) continue;

            //Try to make a path

            List<Tile> path = AStarManager.Instance.GeneratePath(startTile, targetTile);

            if (path == null || path.Count == 0) continue;

            float dist = path.Count;

            if (dist < closestDist)
            {
                closestDist = dist;
                bestTarget = unit;
            }
        }
        if (bestTarget == null)
        {
            return GetRandomTile();
        }
        return bestTarget.OccupiedTile;
    }

    private BaseUnit GetbestHPPlayer()
    {
        BaseUnit bestTarget = null;
        float bestHP = -Mathf.Infinity;

        foreach (var player in UnitManager.Instance.playersSpawned)
        {
            if (player.health > bestHP)
            {
                bestHP = player.health;
                bestTarget = player;
            }
        }

        return bestTarget;

    }

    private int CountUnitsInRange(Tile center, int range)
    {
        int count = 0;

        var tiles = RangeManager.GetTilesInRange(center, range, RangeType.FloodTargeting);

        foreach (var player in UnitManager.Instance.playersSpawned)
        {
            if (tiles.Contains(player.OccupiedTile)) {
                count++;
            }
        }
        return count;
    }

    private Tile GetBossTarget()
    {
        BaseUnit target = GetbestHPPlayer();
        if (target == null)
            return npcUnit.OccupiedTile;

        Tile targetTile = target.OccupiedTile;

        List<Tile> movableTiles = RangeManager.GetTilesInRange(
            npcUnit.OccupiedTile,
            npcUnit.moveRange,
            RangeType.FloodTargeting);

        Tile bestTile = npcUnit.OccupiedTile;
        float bestScore = Mathf.Infinity;

        foreach (var tile in movableTiles)
        {
            if (!tile.isWalkable) continue;

            var path = AStarManager.Instance.GeneratePath(tile, targetTile);
            if (path == null) continue;

            int distToTarget = path.Count;

            if (distToTarget < bestScore)
            {
                bestScore = distToTarget;
                bestTile = tile;
            }
        }

        return bestTile;

    }

    private void UseSingleTargetAttack(BaseUnit target)
    {
        if (target == null) return;

        Debug.Log($"[Boss] Single Target Attack against {target.name}");

        target.takeDamage(npcUnit.dmg, false, false, npcUnit);
    }

    private Tile GetBossMove()
    {
        Tile targetTile = GetbestHPPlayer().OccupiedTile;

        if (occupiedState == BossState.Invulnerable)
        {
            return npcUnit.OccupiedTile;
        }

        // Vulnerable: move like ranged unit
        List<Tile> movableTiles = RangeManager.GetTilesInRange(
            npcUnit.OccupiedTile,
            npcUnit.moveRange,
            RangeType.FloodTargeting);

        Tile bestTile = null;
        float bestScore = -Mathf.Infinity;

        foreach (var tile in movableTiles)
        {
            if (!tile.isWalkable) continue;

            var pathToPlayer = AStarManager.Instance.GeneratePath(tile, targetTile);
            if (pathToPlayer == null) continue;

            int distToPlayer = pathToPlayer.Count;

            var pathFromStart = AStarManager.Instance.GeneratePath(npcUnit.OccupiedTile, tile);
            if (pathFromStart == null) continue;

            int moveDistance = pathFromStart.Count;

            float score = -distToPlayer + moveDistance;

            // discourage staying still
            if (tile == npcUnit.OccupiedTile)
                score -= 1000f;


            if (score > bestScore)
            {
                bestScore = score;
                bestTile = tile;
            }
        }

        if (bestTile != null)
            return bestTile;

        Debug.Log($"[BossMove] Selected tile: {bestTile?.name}");
        return GetRandomTile();
    }

    private void BossAttack()
    {
        Tile myTile = npcUnit.OccupiedTile;
        BaseUnit target = GetbestHPPlayer();

        if (target == null) return;

        Tile targetTile = target.OccupiedTile;

        float dist = Vector2.Distance(
            myTile.transform.position,
            targetTile.transform.position
        );


        bool shouldAOE = CountUnitsInRange(myTile, 1) >= 1
            || CountUnitsInRange(target.OccupiedTile, 1) >= 1;

        if (shouldAOE)
        {
            Debug.Log("Boss using AOE!");
            BossAOEAttack(myTile);
        } 
        if (dist > npcUnit.attackRange)
            return;

        Debug.Log("Boss targeting highest HP player");
        UseSingleTargetAttack(target);
    }
    private bool HasPlayersInAOERange()
    {
        var tilesInRange = RangeManager.GetTilesInRange(npcUnit.OccupiedTile, npcUnit.attackRange, RangeType.FloodTargeting);

        foreach (var unit in UnitManager.Instance.playersSpawned)
        {
            if (tilesInRange.Contains(unit.OccupiedTile)) return true;
        }
        return false;
    
    }

    private int CountPlayersInAOE()
    {
        int count = 0;

        var tilesInRange = RangeManager.GetTilesInRange(
            npcUnit.OccupiedTile,
            aoeRange,
            RangeType.FloodTargeting
        );

        Debug.Log($"[AOE DEBUG] TilesInRange count: {tilesInRange.Count}");
        Debug.Log($"[AOE DEBUG] Players count: {UnitManager.Instance.playersSpawned.Count}");

        foreach (var unit in UnitManager.Instance.playersSpawned)
        {

            if (unit.OccupiedTile == null)
            {
                continue;
            }

            if (tilesInRange.Contains(unit.OccupiedTile))
            {
                count++;
            } 
        }


        return count;
    }
    [SerializeField] private int aoeRange = 3;
   
    private void PerformAOEAttack()
    {
        Debug.Log("[AOE] Boss used AOE attack!");

        var tilesInRange = RangeManager.GetTilesInRange(npcUnit.OccupiedTile, aoeRange, RangeType.FloodTargeting);

        foreach (var unit in UnitManager.Instance.playersSpawned.ToList())
        {
            if (tilesInRange.Contains(unit.OccupiedTile))
            {
                unit.takeDamage(npcUnit.dmg, false, false, npcUnit);
            }
        }
     
    }

    private void BossAOEAttack(Tile centerTile)
    {
        Debug.Log("[Boss] AOE Attack!");

        var tilesInRange = RangeManager.GetTilesInRange(
            centerTile,
            aoeRange, // make sure this exists
            RangeType.FloodTargeting
        );

        foreach (var player in UnitManager.Instance.playersSpawned.ToList())
        {
            if (tilesInRange.Contains(player.OccupiedTile))
            {
                Debug.Log($"[AOE] Hitting {player.name}");

                player.takeDamage(npcUnit.dmg, false, false, npcUnit);
            }
        }
    }


    private Tile GetMeleeTarget()
    {
        var targeting = GetComponent<EnemyTargetingManager>();
        targeting.SelectTarget();

        Tile playerTile = GridManager.Instance.GetTileForUnit(targeting.CurrentTarget.gameObject);

        List<Tile> movableTiles = RangeManager.GetTilesInRange(
        npcUnit.OccupiedTile,
        npcUnit.moveRange,
        RangeType.FloodTargeting);

        Tile bestTile = null;
        float bestScore = Mathf.Infinity;

        foreach (var tile in movableTiles) {

            if (!tile.isWalkable) continue;
            if (tile == npcUnit.OccupiedTile) continue;

            var pathToPlayer = AStarManager.Instance.GeneratePath(tile, playerTile);
            if (pathToPlayer == null || pathToPlayer.Count == 0) continue;

            var pathFromStart = AStarManager.Instance.GeneratePath(npcUnit.OccupiedTile, tile);
            if (pathFromStart == null) continue;

            int moveDistance = pathFromStart.Count;

            // Skip tiles that don't use enough movement
            if (moveDistance < npcUnit.moveRange)
                continue;

            float score = pathToPlayer.Count;

            if (score < bestScore)
            {
                bestScore = score;
                bestTile = tile;
            }
        }

        if (bestTile == null)
            return GetRandomTile();

        return bestTile;
    }
    


    private void Update()
    {
       /* if (GameManager.Instance == null) return;

        if (GameManager.Instance.gameState != GameState.EnemyTurn) return;

        


        if (!isMoving || path == null) return;

        if (pathIndex >= path.Count)
        {
            FinishedMoves();
            return;

        }

        if (tilesMovedThisTurn >= tilesPerMove)
        {
            
            
            //Debug.Log("NPC stopped: reached tilesPerMove limit.");
            FinishedMoves();
            return;
            
           
        }
        

        Tile currentTargetTile = path[pathIndex];
        Vector2 targetPos = currentTargetTile.transform.position;
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        if (currentTargetTile == null)
        {
            Debug.Log("Current target tile is null!");
            return;
        }
       
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            //Debug.Log("NPC reached tile: " + currentTargetTile.name);
            pathIndex++;
            tilesMovedThisTurn++;
        }*/
    }
    //New: Function for setting enemy behavior for movement based on current flag

    public void SetBehaviorTarget(Tile startTile)
    {
        //Added for if target is already in attackRange:
        var targeting = GetComponent<EnemyTargetingManager>();
        if (enemy1.movementBehavior != Enemy1.MovementBehavior.Support)
        {
            if (targeting == null)
            {
                Debug.LogError("Missing EnemyTargetingManager!");
                return;
            }

            if (targeting.CurrentTarget == null || targeting.CurrentTarget.gameObject == null)
            {
                targeting.SelectTarget();
            }
            
            if (targeting.CurrentTarget == null || targeting.CurrentTarget.gameObject == null) 
            {
                Debug.LogWarning("No valid target found.");
                SetTarget(startTile, startTile); // do nothing
                return;
            }

            Tile targetTile = GridManager.Instance.GetTileForUnit(targeting.CurrentTarget.gameObject);

            if (RangeManager.GetTilesInRange(startTile, npcUnit.attackRange, RangeType.FloodTargeting).Contains(targetTile))
            {
                SetTarget(startTile, startTile);
                return;
            }
        }

        Tile chosenTile = null;

        switch (enemy1.movementBehavior)
        {
            case Enemy1.MovementBehavior.Melee:
                chosenTile = GetMeleeTarget();
                break;

            case Enemy1.MovementBehavior.Ranged:
                chosenTile = GetRangedTarget();
                break;
            //Future cases for other behaviors here

            case Enemy1.MovementBehavior.Support:
                chosenTile = GetSupportTarget();
                break;

            case Enemy1.MovementBehavior.Boss:
                chosenTile = GetBossTarget();
                break;


            default:
                targeting.SelectTarget();
                //chosenTile = GridManager.Instance.GetTileForUnit(targeting.CurrentTarget.gameObject);
                chosenTile = GetClosestReachablePlayerTile(startTile);
                break;
        }
        SetTarget(startTile, chosenTile);
    }

    public void SetTarget(Tile startTile, Tile endTile = null)
    {
    
       
        if (startTile == null)
        {
            Debug.Log("Invalid start or end tile!");
            return;
        }

        if (endTile == null)
        {
            var target = GetComponent<EnemyTargetingManager>();
            if (target != null)
            {
                if(target.CurrentTarget == null || target.CurrentTarget.gameObject == null)
                {
                    target.SelectTarget();
                }
               
                if (target.CurrentTarget != null)
                {
                    endTile = GridManager.Instance.GetTileForUnit(target.CurrentTarget.gameObject);
                }
            }
                
        }

        if (!endTile.isWalkable) Debug.Log("End tile is terrain-blocked");

        path = AStarManager.Instance.GeneratePath(startTile, endTile);
        Debug.Log($"[SETTARGET BEFORE TRIM] PathLen: {path?.Count}");
        if (path != null && path.Count > 1)
        {
            Tile lastTile = path[path.Count - 1];
            if (lastTile == endTile)
            {
                path.RemoveAt(path.Count - 1);
            }
        } else
        {
            path = new List<Tile> {npcUnit.OccupiedTile};
        }

        if (path[0] == startTile)
        {
            path.RemoveAt(0);
        }
        int count = Mathf.Min(npcUnit.moveRange, path.Count);

        if (path.Count > npcUnit.moveRange + npcUnit.moveModifier)
        {
            if (npcUnit.moveRange + npcUnit.moveModifier < 0)
            {
                path = path.GetRange(0, count);
            } else
            {
                path = path.GetRange(0, npcUnit.moveRange + npcUnit.moveModifier);
            }
            
        }
        Debug.Log($"[SETTARGET AFTER TRIM] PathLen: {path?.Count}");

        pathIndex = 0;
        tilesMovedThisTurn = 0;
        //isMoving = true;

        Debug.Log("Enemy target set to: " + endTile.name);

        //Adding debug to show path
        Debug.Log("Start tile: " + startTile?.name);
        Debug.Log("End tile: " + endTile?.name + " Walkable: " + (endTile?.Walkable));
    }

    private List<Tile> GetTilesInAttackRange(Tile startTile,int range)
    {
        List<Tile> inRangeTiles = new List<Tile>();
        HashSet<Tile> visited = new HashSet<Tile>();
        Queue<(Tile tile, int distance)> queue = new Queue<(Tile, int)>();

        queue.Enqueue((startTile, 0));
        visited.Add(startTile);

        while(queue.Count > 0)
        {
            var current = queue.Dequeue();
            if(current.distance > 0)
            {
                inRangeTiles.Add(current.tile);
            }

            if(current.distance < range)
            {
                foreach(Tile neighbor in GridManager.Instance.GetNeighborsOf(current.tile))
                {
                    if (!visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue((neighbor, current.distance + 1));
                    }
                }
            }
        }
        return inRangeTiles;
    }

    private void CheckAndAttack()
    {
        //AOE logic:
        int aoeCount = CountPlayersInAOE();

        if (isEliteVarient && enemy1.movementBehavior == BaseUnit.MovementBehavior.Melee)
        {

            if (aoeCount >= 1) 
            {
                PerformAOEAttack();
                return; 
            }
        }

        //End of AOE logic

        List<Tile> attackableTiles = npcUnit.GetTilesInAttackRange();
        Debug.Log($"NPC checking attack. Range: {npcUnit.attackRange}. Tiles found in range: {attackableTiles.Count}");
        BaseUnit target = null;
        

        foreach (Tile tile in attackableTiles)
        {
            //Changed a little bit
            if (tile.OccupiedUnit != null && tile.OccupiedUnit.Faction == Faction.Player && tile.OccupiedUnit.invisible == 0)
            {
                Debug.Log($"Found {tile.OccupiedUnit.name} on tile {tile.name}. Faction: {tile.OccupiedUnit.Faction}");
                target = tile.OccupiedUnit;
                Debug.Log($"{target}");
                break;
            } 
        }

        if(target != null)
        {
            if (unitAnimator != null)
            {
                unitAnimator.SetTrigger("attack");
            }
            Debug.Log($"{npcUnit.name} attacks {target.name}");
            target.takeDamage(npcUnit.dmg * npcUnit.attackModifier, true, false, npcUnit);
        } else {
            Debug.Log("nothing happened!");
        }
    }

    private void ExecuteAttacks()
    {
        if (enemy1.movementBehavior == Enemy1.MovementBehavior.Support)
        {
           //Support units won't attack
           HasFinishedTurn = true;
           return;
        }
        CheckAndAttack();
        HasFinishedTurn = true;
        //StartCoroutine(EndTurnDelay());
    }

    public void BeginTurn()
    {

        tilesMovedThisTurn = 0;
        HasFinishedTurn = false;
        //isMoving = false;
        Debug.Log($"[BEGIN TURN] {name}");
        //New: Ensure targeting happens first
        npcUnit.ApplyConquestStats();
        var targeting = GetComponent<EnemyTargetingManager>();
        if (enemy1.movementBehavior != Enemy1.MovementBehavior.Support)
        {
            if (targeting != null && (targeting.CurrentTarget == null || targeting.CurrentTarget.gameObject == null))
            {
                targeting.SelectTarget();
            }
        }
        Tile startTile = npcUnit.OccupiedTile;
        if (startTile == null)
        {
            Debug.LogError($"{name} has no OccupiedTile placeholder!");
            HasFinishedTurn = true;
            return;
            
        }
        //GetComponent<EnemyTargetingManager>().SelectTarget();
        //SetTarget(startTile);

        //Changed to set behavior target for movement based on current flag
        SetBehaviorTarget(startTile);
        //StartCoroutine(MoveAlongPath());
        //Update();

    }


    public bool HasFinishedTurn { get; private set; }

    private void FinishedMoves()
    {
        //isMoving = false;
        Tile old = npcUnit.OccupiedTile;

        if (path != null && path.Count > 0)
        {
            
            Tile finalTile = path[path.Count -1];
            if (old != null)
            {
                old.setUnit(null);
            }
            npcUnit.OccupiedTile = finalTile;
            finalTile.setUnit(npcUnit);
            Debug.Log($"{gameObject.name} committed to tile: {finalTile.name}.");
            
            //future implementation of enemy combat here i think.
        
        }
        Debug.Log($"{gameObject.name} finished moving.");
        //ExecuteAttacks();
        npcUnit.moveRange = tilesPerMove;
        HasFinishedTurn = true;
        //StartCoroutine(EndTurnDelay());
    }

    private IEnumerator EndTurnDelay()
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.ChangeState(GameState.PlayerTurn);
    }

    //Added for support behavior: checks for lowest-health ally in range after moving and heals if possible

    private void CheckForHealAfterMove()
    {
        BaseUnit lowest = null;
        float lowestHealth = Mathf.Infinity;

        // Find lowest-health ally
        foreach (var unit in UnitManager.Instance.enemiesSpawned)
        {
            if (unit == npcUnit) continue;

            if (unit.Faction != npcUnit.Faction) continue;

            if (unit.health < lowestHealth)
            {
                lowestHealth = unit.health;
                lowest = unit;
            }
        }

        if (lowest == null) return;

        Tile allyTile = lowest.OccupiedTile;

        bool inHealRange = RangeManager
            .GetTilesInRange(npcUnit.OccupiedTile, npcUnit.attackRange, RangeType.FloodTargeting)
            .Contains(allyTile);

        if (inHealRange && lowest.health < lowest.maxHealth)
        {
            HealTarget(lowest, healAmount);
        }
    }

    private IEnumerator MoveAlongPath()
    {
        while (pathIndex < path.Count && tilesMovedThisTurn < npcUnit.moveRange)
        {
            Tile currentTargetTile = path[pathIndex];
            if (currentTargetTile == null) yield break;

            Vector2 targetPos = currentTargetTile.transform.position;

            while (Vector2.Distance(transform.position, targetPos) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }

            pathIndex++;
            tilesMovedThisTurn++;
            yield return null;
        }
    }




    public IEnumerator TakeTurn()
    {
        currentState = TurnState.BeginTurn;
        HasFinishedTurn = false;

        while (currentState != TurnState.Idle)
        {
            switch (currentState)
            {
                case TurnState.BeginTurn:
                    tilesMovedThisTurn = 0;

                    if (npcUnit.restricted != EffectFlag.None)
                    {
                        npcUnit.moveRange = 0;
                    }

                    var targeting = GetComponent<EnemyTargetingManager>();
                    if (enemy1.movementBehavior != Enemy1.MovementBehavior.Support)
                    {
                        if (targeting != null)
                            targeting.SelectTarget();
                    }

                    if (npcUnit.OccupiedTile == null)
                    {
                        Debug.LogError($"{name} has no OccupiedTile!");
                        currentState = TurnState.EndTurn;
                        break;
                    }

                    currentState = TurnState.Targeting;
                    break;

                case TurnState.Targeting:
                    if (unitAnimator != null)
                        unitAnimator.SetBool("IsMoving", true);

                    SetBehaviorTarget(npcUnit.OccupiedTile);
                    currentState = TurnState.Moving;
                    break;

                case TurnState.Moving:
                    yield return StartCoroutine(MoveAlongPath());
                    currentState = TurnState.Acting;
           
                    break;

                case TurnState.Acting:
                    if (unitAnimator != null)
                        unitAnimator.SetBool("IsMoving", false);

                    FinishedMoves();
                    CheckForHealAfterMove();
                    if (enemy1.movementBehavior == Enemy1.MovementBehavior.Boss)
                    {
                        BossAttack();
                    }
                    else
                    {
                        CheckAndAttack();
                    }    
                    currentState = TurnState.EndTurn;
                    break;

                case TurnState.EndTurn:
                    HasFinishedTurn = true;
                    currentState = TurnState.Idle;
                    break;
            }

            yield return null;
        }
        /*tilesMovedThisTurn = 0;
        HasFinishedTurn = false;

        if (!(npcUnit.restricted == EffectFlag.None))
        {
            npcUnit.moveRange = 0;
        }

        
        //Select target
        var targeting = GetComponent<EnemyTargetingManager>();
        if (enemy1.movementBehavior != Enemy1.MovementBehavior.Support) { 
            if (targeting != null)
            targeting.SelectTarget();
        }
        Tile startTile = npcUnit.OccupiedTile;
        if (startTile == null)
        {
            Debug.LogError($"{name} has no OccupiedTile placeholder!");
            HasFinishedTurn = true;
            yield break;
        }

        if(unitAnimator != null)
        {
            unitAnimator.SetBool("IsMoving", true);
        }

        //SetTarget(startTile);

        //Changed to set behavior target for movement based on current flag
        SetBehaviorTarget(startTile);

        while (pathIndex < path.Count && tilesMovedThisTurn < npcUnit.moveRange)
        {
            Tile currentTargetTile = path[pathIndex];
            if (currentTargetTile == null) break;

            Vector2 targetPos = currentTargetTile.transform.position;

            while (Vector2.Distance(transform.position, targetPos) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return null;
            }
            pathIndex++;
            tilesMovedThisTurn++;
            yield return null;
        }
            if(unitAnimator != null)
            {
                unitAnimator.SetBool("IsMoving", false);
        }

        FinishedMoves();
        CheckForHealAfterMove();*/
    }

    public static IEnumerator RunEnemyTurn(List<NPC_Controller> enemies)
    {

        // for (int i = 0; i < 9; i++)
        // {
            
        // }



        foreach (var npc in enemies)
        {
            if (npc == null) continue;

            yield return npc.StartCoroutine(npc.TakeTurn());
        }
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.ChangeState(GameState.PlayerTurn);
    }
}
