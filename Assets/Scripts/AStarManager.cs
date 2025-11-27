using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class AStarManager : MonoBehaviour
{
    public static AStarManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<Tile> GeneratePath(Tile start, Tile end)
    {
        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();

        Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
        Dictionary<Tile, float> gScore = new Dictionary<Tile, float>();
        Dictionary<Tile, float> fScore = new Dictionary<Tile, float>();

        gScore[start] = 0;
        fScore[start] = Heuristic(start, end);
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            Tile current = GetLowestFScore(openSet, fScore);

            if (current == end)
            {
                return ReconstructPath(cameFrom, current);
            }
            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Tile neighbor in GridManager.Instance.GetNeighborsOf(current))
            {
                if (!neighbor.Walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }
                float tentativeG = gScore[current] + 1;

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, end);

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
            
        }
        return null;
     }

    private float Heuristic(Tile a, Tile b)
    {

        return Mathf.Abs(a.transform.position.x - b.transform.position.x) + Mathf.Abs(a.transform.position.y - b.transform.position.y);
    }

    private Tile GetLowestFScore(List<Tile> openSet, Dictionary<Tile, float> fScore)
    {
        Tile lowest = openSet[0];
        float lowestScore = fScore.ContainsKey(lowest) ? fScore[lowest] : Mathf.Infinity;

        foreach (Tile tile in openSet)
        {
            float score = fScore.ContainsKey(tile) ? fScore[tile] : Mathf.Infinity;
            if (score < lowestScore)
            {
                lowest = tile;
                lowestScore = score;
            }
        }
        return lowest;
    }

    private List<Tile> ReconstructPath(Dictionary<Tile, Tile> cameFrom, Tile current)
    {
        List<Tile> path = new List<Tile> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }
        return path;
    }
    
    
}

    
