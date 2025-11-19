using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    [SerializeField] private int width, height;
    [SerializeField] private Tile grassTile, mountainTile;
    [SerializeField] private Transform cam;
    private Dictionary<Vector2, Tile> tiles;

    private void Awake()
    {
        Instance = this;
    }

    //Generates Grid based on w/h of inspector values
    //Switchs game state
    public void GenerateGrid()
    {
        tiles = new Dictionary<Vector2, Tile>();
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
            Debug.Log($"Creating {spawnedTile.name} offset={isOffset}");
            spawnedTile.Init(x,y);

            tiles[currentPos] = spawnedTile;
                //var randomTile = UnityEngine.Random.Range(0, 6) == 3 ? mountainTile : grassTile;
                //var spawnedTile = Instantiate(randomTile, new Vector3(x, y), Quaternion.identity);
                //spawnedTile.name = $"Tile {x} {y}";

                //var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                //Debug.Log($"Creating {spawnedTile.name} offset={isOffset}");
                //spawnedTile.Init(x,y);

                //tiles[new Vector2(x, y)] = spawnedTile;
            }
        }
        cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);

        GameManager.Instance.ChangeState(GameState.SpawnPlayers);
    }

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
                // We assume mountain tiles contain "mountain" in their name, 
                // as set during grid generation.
                if (neighborTile.gameObject.name.Contains("mountain"))
                {
                    mountainCount++;
                }
            }
        }
        return mountainCount >= 3;
    }
        
    public Tile GetPlayerSpawnTile()
    {
        return tiles.Where(t => t.Key.x < 2 && t.Value.Walkable && !IsSurroundedByMountains(t.Value)).OrderBy(t => UnityEngine.Random.value).First().Value;
    }

    public Tile GetEnemySpawnTile()
    {
        return tiles.Where(t => t.Key.x >= width - 2 && t.Value.Walkable && !IsSurroundedByMountains(t.Value)).OrderBy(t => UnityEngine.Random.value).First().Value;
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (tiles.TryGetValue(pos, out var tile)) return tile;
        return null;
    }
}