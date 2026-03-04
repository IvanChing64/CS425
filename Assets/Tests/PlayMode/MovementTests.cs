using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// Tests player movement behavior, some code borrowed from Ivan's test
/// </summary>
/// <remarks>By Liam Riel</remarks>>
public class MovementTests
{
    private BasePlayer player;

    [SetUp]
    public void SetUp()
    {
        // Grid setup
        new GameObject("GridManager").AddComponent<GridManager>();
        var dictField = typeof(GridManager).GetField("tiles", BindingFlags.NonPublic | BindingFlags.Instance);
        Dictionary<Vector2, Tile> tileDictionary = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < 7; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                Vector2 pos = new Vector2(x, y);
                Tile tile = new GameObject($"Tile {x} {y} (TestTile)").AddComponent<TestTile>();
                tile.transform.position = pos;

                if (x == 2 && y == 3)
                    tile.isWalkable = false;
                else
                    tile.isWalkable = true;

                tileDictionary.Add(pos, tile);
            }
        }
        
        dictField.SetValue(GridManager.Instance, tileDictionary);

        // Player setup
        player = new GameObject("TestPlayer").AddComponent<BasePlayer>();
        player.moveRange = 3;
        player.OccupiedTile = GridManager.Instance.GetTileAtPosition(new Vector2(3, 3));
        player.OccupiedTile.OccupiedUnit = player;
    }

    // A Test behaves as an ordinary method
    //[Test]
    //public void Test_MovementLogic()
    //{
    //    // Use the Assert class to test conditions
    //}

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator Test_PlayerMovement()
    {
        List<Tile> range = player.GetTilesInMoveRange();

        // Check that the movement range contains tiles at its maximum extent
        Assert.Contains(GridManager.Instance.GetTileAtPosition(new Vector2(3, 0)), range, "The player cannot access their northernmost range");
        Assert.Contains(GridManager.Instance.GetTileAtPosition(new Vector2(3, 6)), range, "The player cannot access their southernmost range");
        Assert.Contains(GridManager.Instance.GetTileAtPosition(new Vector2(6, 3)), range, "The player cannot access their easternmost range");

        // Check that the movement range does not contain tiles which should be unwalkable
        Assert.IsFalse(range.Contains(GridManager.Instance.GetTileAtPosition(new Vector2(2, 3))), "The player can access unwalkable tiles");

        // Check that the movement range does not contain tiles that would take a higher range to get to
        Assert.IsFalse(range.Contains(GridManager.Instance.GetTileAtPosition(new Vector2(1, 3))), "The player can access the tile beyond the mountains");

        Assert.IsFalse(range.Contains(GridManager.Instance.GetTileAtPosition(new Vector2(1, 1))), "The player can access the southwestern tile");
        Assert.IsFalse(range.Contains(GridManager.Instance.GetTileAtPosition(new Vector2(1, 5))), "The player can access the northwestern tile");
        Assert.IsFalse(range.Contains(GridManager.Instance.GetTileAtPosition(new Vector2(5, 5))), "The player can access the northeastern tile");
        Assert.IsFalse(range.Contains(GridManager.Instance.GetTileAtPosition(new Vector2(5, 1))), "The player can access the northwestern tile");

        yield return null;
    }

    [TearDown]
    public void TearDown()
    {
        
    }
}
