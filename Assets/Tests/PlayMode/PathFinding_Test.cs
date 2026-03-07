using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using System.Reflection;

public class PathFinding_Test
{
   
    [SetUp]
    public void SetUp()
    {
        new GameObject("GridManager").AddComponent<GridManager>();
        var dictField = typeof(GridManager).GetField("tiles", BindingFlags.NonPublic | BindingFlags.Instance);
        Dictionary<Vector2, Tile> mockTile = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                Vector2 pos = new Vector2(x, y);
                Tile tile = new GameObject($"Tile {x} {y} (TestTile)").AddComponent<TestTile>();
                tile.transform.position = pos;

                if (x == 2 && y == 0)
                 tile.isWalkable = false;
                else
                    tile.isWalkable = true;
                mockTile.Add(pos, tile);


            }
        }
        dictField.SetValue(GridManager.Instance, mockTile);


    }

    [UnityTest]
    public IEnumerator PathFinding_TestWithEnumeratorPasses()
    {
     
        // --- Validate ---
        var aStarGO = new GameObject("AStarManager");
        var aStar = aStarGO.AddComponent<AStarManager>();

        Tile start = GridManager.Instance.GetTileAtPosition(new Vector2(0, 0));
        Tile end = GridManager.Instance.GetTileAtPosition(new Vector2(4, 0));

        Assert.IsNotNull(start);
        Assert.IsNotNull(end);

        var path = aStar.GeneratePath(start, end);

        Assert.IsNotNull(path);
        Assert.Greater(path.Count, 0);

        yield return null;
    }

}
