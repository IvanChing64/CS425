using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Developer: Andrew Shelton
//Edits from Ivan Ching
//Aggregated from multiple tutorials and sources


public class NPC_Controller: MonoBehaviour
{
    public static NPC_Controller Instance;
    public float moveSpeed = 0.1f;
    public int tilesPerMove = 4;

    public List<Tile> path;
    private int pathIndex;
    private int tilesMovedThisTurn;
    private bool isMoving;

    private BaseUnit npcUnit; 

    private void Awake()
    {
        Instance = this;
        npcUnit = GetComponent<BaseUnit>();
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;

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
        }
    }

    public void SetTarget(Tile startTile, Tile endTile = null)
    {
    
       
        if (startTile == null)
        {
            Debug.Log("Invalid start or end tile!");
            return;
        }

        if (endTile == null && UnitManager.Instance.SelectedPlayer != null)
        {
            endTile = GridManager.Instance.GetTileAtPosition(UnitManager.Instance.SelectedPlayer.transform.position);
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
        }

        if (path[0] == startTile)
        {
             path.RemoveAt(0);
        }
        
        if (path.Count > tilesPerMove)
        {
            path = path.GetRange(0, tilesPerMove);
        }

        pathIndex = 0;
        tilesMovedThisTurn = 0;
        isMoving = true;

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
            if (tile.OccupiedUnit != null && tile.OccupiedUnit.Faction == Faction.Player)
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
            target.takeDamage(npcUnit.dmg);
        } else {
             Debug.Log("nothing happened!");
        }
    }

    private void ExecuteAttacks()
    {
        CheckAndAttack();
        HasFinishedTurn = true;
        StartCoroutine(EndTurnDelay());
    }

    public void BeginTurn()
    {
        tilesMovedThisTurn = 0;
        HasFinishedTurn = false;
        isMoving = false;
        Tile startTile = npcUnit.OccupiedTile;
        SetTarget(startTile);
        //Update();
        
    }


    public bool HasFinishedTurn { get; private set; }

    private void FinishedMoves()
    {
        isMoving = false;
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
        HasFinishedTurn = true;
        StartCoroutine(EndTurnDelay());
    }

    private IEnumerator EndTurnDelay()
    {
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.ChangeState(GameState.PlayerTurn);
    }
}
