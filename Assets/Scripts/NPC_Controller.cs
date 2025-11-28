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
        if (GameManager.Instance.gameState != GameState.EnemyTurn) return;

        if (!isMoving || path == null || pathIndex >= path.Count) return;

        if (tilesMovedThisTurn >= tilesPerMove)
        {
            isMoving = false;
            return;
        }

        Tile currentTargetTile = path[pathIndex];
        if (currentTargetTile == null) return;

        Vector2 targetPos = currentTargetTile.transform.position;
        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPos) < 0.05f)
        {
            pathIndex++;
            tilesMovedThisTurn++;
        }
    }

    public void SetTarget(Vector2 targetPos)
    {
        Tile startTile = GridManager.Instance.GetTileAtPosition(transform.position);
        Tile endTile = GridManager.Instance.GetTileAtPosition(targetPos);

        path = AStarManager.Instance.GeneratePath(startTile, endTile);
        pathIndex = 0;
        tilesMovedThisTurn = 0;
        isMoving = true;
    }

    public void BeginTurn()
    {
        tilesMovedThisTurn = 0;
        isMoving = true;
    }


}
