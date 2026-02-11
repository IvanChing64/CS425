using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

//Developer: Ivan Ching
//Aggregated from multiple tutorials and sources

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    //[SerializeField] private int width, height;
    [SerializeField] private Tile grassTile, mountainTile;
    [SerializeField] private Transform cam;
    private Dictionary<Vector2, Tile> tiles;
    //min 10, max 38
    //default 18
    public static int width = 18;
    //min 5, max 18
    //default 10
    public static int height = 10;

    private void Awake()
    {
        Instance = this;
    }

    private HashSet<Tile> GetReachableTiles(Tile startTile)
    {
        HashSet<Tile> reachable = new HashSet<Tile>();
        Queue<Tile> frontier = new Queue<Tile>();

        if(startTile == null || !startTile.isWalkable)
        {
            return reachable;
        }

        frontier.Enqueue(startTile);
        reachable.Add(startTile);

        while (frontier.Count > 0)
        {
            Tile current = frontier.Dequeue();
            foreach(Tile neighbor in GetNeighborsOf(current))
            {
                if(neighbor.isWalkable && !reachable.Contains(neighbor))
                {
                    reachable.Add(neighbor);
                    frontier.Enqueue(neighbor);
                }
            }
        }
        return reachable;
    }

    //Generates Grid based on w/h of inspector values
    //Switchs game state
    public void GenerateGrid()
    {
        //open dictionary for tiles storage and data usage
        tiles = new Dictionary<Vector2, Tile>();
        //2 for loops for width and height for grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                Tile tileToSpawn = grassTile;
                Vector2 currentPos = new Vector2(x, y);

                bool isNeighborMountain = false;

                Vector2 leftPos = new Vector2(x - 1, y);
                if(tiles.ContainsKey(leftPos) && tiles[leftPos].gameObject.name.Contains("mountain"))
                {
                    isNeighborMountain = true;
                }

                Vector2 belowPos = new Vector2(x, y - 1);
                if (!isNeighborMountain && tiles.ContainsKey(belowPos) && tiles[belowPos].gameObject.name.Contains("mountain"))
                {
                    isNeighborMountain = true;
                }

                //Define base mountain chance (e.g., 1 in 10 for a scattered mountain)
                int baseMountainChance = 10;
            
                // Define increased chance multiplier if a neighbor is a mountain
                int neighborMountainMultiplier = 4; 

                int mountainRoll = UnityEngine.Random.Range(0, baseMountainChance);

                if (isNeighborMountain)
                {
                    // Increase the chance of a mountain
                    if (mountainRoll < neighborMountainMultiplier)  
                    {
                        tileToSpawn = mountainTile;
                    }
                }
                else
                {
                    // Use the base chance for an isolated mountain
                    if (mountainRoll == 0) // Only on a roll of 0 (1 in 10 chance)
                    {
                        tileToSpawn = mountainTile;
                    }
                }

                var spawnedTile = Instantiate(tileToSpawn, new Vector3(x, y), Quaternion.identity);
            

                //Creates tiles with mountain or grass name currently
                if (tileToSpawn == mountainTile)
                {
                    spawnedTile.name = $"Tile {x} {y} (mountain)";
                }
                else
                {
                    spawnedTile.name = $"Tile {x} {y} (grass)";
                }


                //Checkerboard coloring
                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);

                //Shows tiles name in debug console
                //Debug.Log($"Creating {spawnedTile.name} offset={isOffset}");
                spawnedTile.Init(x,y);

                tiles[currentPos] = spawnedTile;

            }
        }

        Tile startCheck = tiles.Values.FirstOrDefault(t => t.Position.x == 0 && t.isWalkable);
        var reachableTiles = GetReachableTiles(startCheck);
        bool canReachEnemySide = reachableTiles.Any(t => t.Position.x == width - 1);

        if (!canReachEnemySide)
        {
            Debug.Log("can reach otherside, clearing way");
            foreach (var t in tiles.Values) Destroy(t.gameObject);
            tiles.Clear();
            GenerateGrid();
            return;
        }


        //set camera and change states to spawn units
        //Changed camera settings
        //Centers camera to the middle of board
        float centerX = (float)width / 2f - 0.5f;
        float centerY = (float)height / 2f - 0.5f;
        cam.transform.position = new Vector3(centerX, centerY, -10);

        //change the camera's perspective
        Camera cameraComponent = cam.GetComponent<Camera>();
        if (cameraComponent.orthographic)
        {

            float heightZoom = (float)height / 2f;
            float widthZoom = ((float)width / cameraComponent.aspect) / 2f;

            float baseZoom = Mathf.Max(heightZoom, widthZoom);

            //change '+x.0f' for zoom of the camera
            cameraComponent.orthographicSize = baseZoom + 3.0f;
        }
        GameManager.Instance.ChangeState(GameState.SpawnPlayers);
    }


    //Helper functions: mountiain checks  
    private bool IsSurroundedByMountains(Tile tile)
    {
        // Get the coordinates of the tile
        Vector2 tileCoords = new Vector2(tile.transform.position.x, tile.transform.position.y);
        
        // Define the relative positions of directions
        Vector2[] neighborOffsets = new Vector2[]
        {
            new Vector2(0, 1),  // North
            new Vector2(0, -1), // South
            new Vector2(1, 0),  // East
            new Vector2(-1, 0)  // West
        };

        int mountainCount = 0;

        foreach (var offset in neighborOffsets)
        {
            Vector2 neighborPos = tileCoords + offset;

            // 1. Check if the neighbor tile exists in the dictionary
            if (tiles.TryGetValue(neighborPos, out Tile neighborTile))
            {
                // 2. Check if the neighbor tile is a mountain tile
                if (neighborTile.gameObject.name.Contains("mountain"))
                {
                    mountainCount++;
                }
            }
        }
        return mountainCount >= 3;
    }

    //Helper functions: Spawn tiles   
    public Tile GetPlayerSpawnTile()
    {
        Tile startPoint = tiles.Values.First(t => t.Position.x == 0 && t.isWalkable);
        var mainArea = GetReachableTiles(startPoint);

        return mainArea.Where(t => t.Position.x < 2 && !IsSurroundedByMountains(t)).OrderBy(t => UnityEngine.Random.value).First();
    }

    public Tile GetEnemySpawnTile()
    {
        Tile startPoint = tiles.Values.First(t => t.Position.x == 0 && t.isWalkable);
        var mainArea = GetReachableTiles(startPoint);

        return mainArea.Where(t => t.Position.x >= width -2 && !IsSurroundedByMountains(t)).OrderBy(t => UnityEngine.Random.value).First();
    }

    //Helper functions: Get tiles  
    public Tile GetTileAtPosition(Vector2 pos)
    {
        //Adding code so that it rounds the positionto grid coordinates.

        Vector2 gridPos = new Vector2(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));

        if (tiles.TryGetValue(gridPos, out var tile)) return tile;
        //Debug Log to help check tile positions
        Debug.Log("No tile found at position: " + gridPos);
        return null;
    }

    //Added function to get tile for unit based on its position
    public Tile GetTileForUnit(GameObject unit)
    {
        if (unit == null)
        {
            Debug.LogWarning("GetTileForUnit called with null unit.");
            return null;
        }

        Vector2 rawPos = unit.transform.position;
        Vector2 gridPos = new Vector2(Mathf.RoundToInt(rawPos.x), Mathf.RoundToInt(rawPos.y));

        Tile tile = GetTileAtPosition(gridPos);
        if (tile == null)
        {
            Debug.LogWarning($"No tile found for unit {unit.name} at raw {rawPos}, rounded {gridPos}");
        }
        return tile;
    }

    public List<Tile> GetNeighborsOf(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();

        Vector2 pos = new Vector2(tile.transform.position.x, tile.transform.position.y);

        // List of the 4 adjacent directions
        Vector2[] dirs = new Vector2[]
        {
            new Vector2(1, 0),   // Right
            new Vector2(-1, 0),  // Left
            new Vector2(0, 1),   // Up
            new Vector2(0, -1)   // Down
        };

        foreach (Vector2 d in dirs)
        {
            Vector2 checkPos = pos + d;

            if (tiles.TryGetValue(checkPos, out Tile neighborTile))
            {
                neighbors.Add(neighborTile);
            }
        }

        return neighbors;
    }

}