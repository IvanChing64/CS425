using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;


public class AStarTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void AStar_Finds_Straight_Path()
    {

        // Use the Assert class to test conditions
        var gridGO = new GameObject("GridManager");
        var grid = gridGO.AddComponent<GridManager>();
        grid.GenerateGrid(5, 1);

        var aStarGO = new GameObject("AStarManager");
        var aStar = aStarGO.AddComponent<AStarManager>();

        Tile start = grid.GetTileAt(0, 0);
        Tile end = grid.GetTileAt(4, 0);

        List<Tile> path = aStar.GeneratePath(start, end);

        Assert.IsNotNull(path);
        Assert.AreEqual(5, path.Count);
        Assert.AreEqual(end, path[path.Count - 1]);

        

    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator NewTestScriptWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        /*var gridGO = new GameObject("GridManager");
        var grid = gridGO.AddComponent<GridManager>();
        grid.GenerateGrid(5, 1);

        var aStarGO = new GameObject("AStarManager");
        var aStar = aStarGO.AddComponent<AStarManager>();

        Tile start = grid.GetTileAt(0, 0);
        Tile end = grid.GetTileAt(4, 0);

        List<Tile> path = aStar.GeneratePath(start, end);

        Assert.IsNotNull(path);
        Assert.AreEqual(5, path.Count);
        Assert.AreEqual(end, path[path.Count - 1]);
        */
        yield return null;
    }
}
