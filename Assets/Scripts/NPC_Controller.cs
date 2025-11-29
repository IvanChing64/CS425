using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NPC_Controller: MonoBehaviour
{
    public float moveSpeed = 1f;
    public int tilesPerMove = 3;

    public List<Tile> path;
    private int pathIndex;
    private int tilesMovedThisTurn;
    private bool isMoving;

 

    private void Update()
    {
        if (GameManager.Instance == null) return;

        if (GameManager.Instance.gameState != GameState.EnemyTurn) return;

        if (!isMoving || path == null || pathIndex >= path.Count) return;

        if (tilesMovedThisTurn >= tilesPerMove)
        {
            isMoving = false;
            return;
        }

        Tile currentTargetTile = path[pathIndex];
        Vector2 targetPos = currentTargetTile.transform.position;
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        if (currentTargetTile == null) return;

       
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) < 0.05f)
        {
            pathIndex++;
            tilesMovedThisTurn++;
        }
    }

    public void SetTarget(Tile startTile, Tile endTile)
    {
    
       
        if (startTile == null || endTile == null || !endTile.Walkable)
        {
            Debug.Log("Invalid start or end tile!");
            return;
        }

        path = AStarManager.Instance.GeneratePath(startTile, endTile);
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
        isMoving = true;
    }


}
