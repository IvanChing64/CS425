using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//Developer: Andrew Shelton
//Edits from Ivan Ching
//Aggregated from multiple tutorials and sources


public class NPC_Controller: MonoBehaviour
{
    public static NPC_Controller Instance;
    public float moveSpeed = 0.5f;
    public int tilesPerMove = 4;

    public List<Tile> path;
    private int pathIndex;
    private int tilesMovedThisTurn;
    private bool isMoving;

    private BaseUnit npcUnit;

    private Enemy1 enemy1;

    private void Awake()
    {
        enemy1 = GetComponent<Enemy1>();
        Instance = this;
        npcUnit = GetComponent<BaseUnit>();
        tilesPerMove = npcUnit.moveRange; 
    }

    //MovementBehavior logic: Andrew Shelton

    private Tile GetRangedTarget()
    {
        var targeting = GetComponent<EnemyTargetingManager>();
        targeting.SelectTarget();

        Tile playerTile = GridManager.Instance.GetTileForUnit(targeting.CurrentTarget.gameObject);

        //Moves to a tile within attack range of the player
        List<Tile> tiles = RangeManager.GetTilesInRange(playerTile, npcUnit.attackRange, RangeType.FloodTargeting);
        return tiles.Count > 0 ? tiles[0] : playerTile; // Default to player's tile if no valid tiles found
    }

    [SerializeField] private int healAmount = 10;

    private Tile GetSupportTarget()
    {
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
       var tiles = GridManager.Instance.AllTiles;
        int index = UnityEngine.Random.Range(0, tiles.Count);
        return tiles[index];
    }

    private void HealTarget(BaseUnit target, int healAmount)
    {
        if (target == null) return;

        target.health = Mathf.Min(target.health + healAmount, target.maxHealth);

        target.UpdateHealth();
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
            if (targeting.CurrentTarget == null)
                targeting.SelectTarget();

            Tile targetTile = GridManager.Instance.GetTileForUnit(GetComponent<EnemyTargetingManager>().CurrentTarget.gameObject);

            if (RangeManager.GetTilesInRange(startTile, npcUnit.attackRange, RangeType.FloodTargeting).Contains(targetTile))
            {
                SetTarget(startTile, startTile);
                return;
            }
        }

        Tile chosenTile = null;

        switch (enemy1.movementBehavior)
        {
            case Enemy1.MovementBehavior.Ranged:
                chosenTile = GetRangedTarget();
                break;
            //Future cases for other behaviors here

            case Enemy1.MovementBehavior.Support:
                chosenTile = GetSupportTarget();
                break;


            default:
                targeting.SelectTarget();
                chosenTile = GridManager.Instance.GetTileForUnit(targeting.CurrentTarget.gameObject);
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
        
        if (path.Count > npcUnit.moveRange)
        {
            path = path.GetRange(0, npcUnit.moveRange);
        }

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
        List<Tile> attackableTiles = GetTilesInAttackRange(npcUnit.OccupiedTile, npcUnit.attackRange);
        Debug.Log($"NPC checking attack. Range: {npcUnit.attackRange}. Tiles found in range: {attackableTiles.Count}");
        BaseUnit target = null;
        

        foreach (Tile tile in attackableTiles)
        {
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
            Debug.Log($"{npcUnit.name} attacks {target.name}");
            target.takeDamage(npcUnit.dmg * npcUnit.attackModifier);
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
        //New: Ensure targeting happens first
        var targeting = GetComponent<EnemyTargetingManager>();
        if (enemy1.movementBehavior != Enemy1.MovementBehavior.Support)
        {
            if (targeting != null || (targeting.CurrentTarget == null || targeting.CurrentTarget.gameObject == null))
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
        ExecuteAttacks();
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





    public IEnumerator TakeTurn()
    {
        tilesMovedThisTurn = 0;
        HasFinishedTurn = false;

        
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
        
        FinishedMoves();
        CheckForHealAfterMove();
    }

    public static IEnumerator RunEnemyTurn(List<NPC_Controller> enemies)
    {
        foreach (var npc in enemies)
        {
            if (npc == null) continue;

            yield return npc.StartCoroutine(npc.TakeTurn());
        }
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.ChangeState(GameState.PlayerTurn);
    }
}
