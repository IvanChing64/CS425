using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
//Developer: Andrew Shelton
//Developed with help from Game Dev Garnet on youtube
public class AStarManager : MonoBehaviour
{
    public static AStarManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<Tile> GeneratePath(Tile start, Tile end)
    {

        if (start == null || end == null)
        {
            Debug.Log("Start or end tile is null!");
            return null;
        }

        List<Tile> openSet = new List<Tile> { start };
        HashSet<Tile> closedSet = new HashSet<Tile>();

        Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();
        Dictionary<Tile, float> gScore = new Dictionary<Tile, float> { [start] = 0 };
        Dictionary<Tile, float> fScore = new Dictionary<Tile, float> { [start] = Heuristic(start, end) };

        

        while (openSet.Count > 0)
        {
            
            Tile current = GetLowestFScore(openSet, fScore);
            //Debug.Log("Visiting: " + current.name);

            //Debug.Log("Comparing current " + current.name + " with end " + end.name);


            if (current == end)
            {
                return ReconstructPath(cameFrom, current);
            }
            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Tile neighbor in GridManager.Instance.GetNeighborsOf(current))
            {
                if (closedSet.Contains(neighbor)) continue;

                if (neighbor.OccupiedUnit != null && neighbor != end) continue;

                if (!neighbor.Walkable && neighbor != end) continue;
                
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
            //Debug.Log("Checking tile: " + current.name);


        }
        Debug.Log("Start: " + start.name + " End: " + end.name);
        if (end == null || !end.Walkable) Debug.Log("End tile invalid!");
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
        Debug.Log("Path length: " + path.Count);
        return path;
        
    }
    
    
}

    
