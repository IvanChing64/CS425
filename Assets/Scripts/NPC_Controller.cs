using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
            
            
            Debug.Log("NPC stopped: reached tilesPerMove limit.");
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
            Debug.Log("NPC reached tile: " + currentTargetTile.name);
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


    public void BeginTurn()
    {
        tilesMovedThisTurn = 0;
        HasFinishedTurn = false;
        isMoving = false;
        Tile startTile = npcUnit.OccupiedTile; 
        Update();
        
    }

    public bool HasFinishedTurn { get; private set; }

    private void FinishedMoves()
    {
        isMoving = false;
        HasFinishedTurn = true;
        if (path != null && path.Count > 0)
        {
            Tile finalTile = path[path.Count -1];
            finalTile.setUnit(npcUnit);
            Debug.Log($"{gameObject.name} committed to tile: {finalTile.name}.");
            //future implementation of enemy combat here i think.
        
        }
        Debug.Log($"{gameObject.name} finished moving.");
        GameManager.Instance.ChangeState(GameState.PlayerTurn);
    }


}
