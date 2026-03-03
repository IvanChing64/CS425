//Author: Ivan Ching
//Player Attack Test

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using System.Reflection;

public class TestTile : Tile { }

public class AttackTests
{

    private GameObject testRoot;
    private combatUIManager uiManager;
    private BasePlayer player;
    private BaseEnemy enemy;

    [SetUp]
    public void SetUp()
    {
        testRoot = new GameObject("CombatTestRoot");
        testRoot.SetActive(false);

        uiManager = testRoot.AddComponent<combatUIManager>();

        //Set Up UI Panels
        GameObject combatPanel = new GameObject("CombatPanel");
        combatPanel.transform.SetParent(testRoot.transform);
        GameObject endTurnPanel = new GameObject("EndTurnPanel");
        endTurnPanel.transform.SetParent(testRoot.transform);

        var flags = BindingFlags.NonPublic | BindingFlags.Instance;
        typeof(combatUIManager).GetField("combatPanel", flags).SetValue(uiManager, combatPanel);
        typeof(combatUIManager).GetField("endTurnPanel", flags).SetValue(uiManager, endTurnPanel);

        //Set Up Mock Managers
        if (CardManager.instance == null)
        {
            new GameObject("CardManager").AddComponent<CardManager>();
        }
        //Set Up Grid Manager to include a dictionary for the tiles
        if (GridManager.Instance == null)
        {
            GameObject GridObject = new GameObject("GridManager");
            GridManager GMScript = GridObject.AddComponent<GridManager>();

            var dictField = typeof(GridManager).GetField("tiles", BindingFlags.NonPublic | BindingFlags.Instance);
            dictField.SetValue(GMScript, new Dictionary<Vector2, Tile>());
            
        }

        //Unit Set UP
        player = new GameObject("Player").AddComponent<BasePlayer>();
        enemy = new GameObject("Enemy").AddComponent<BaseEnemy>();

        typeof(BaseUnit).GetField("hurtSFX", flags).SetValue(enemy, new AudioClip[0]);

        //Unit Tiles Set Up
        player.OccupiedTile = new GameObject("Tile").AddComponent<TestTile>();
        enemy.OccupiedTile = new GameObject("Tile").AddComponent<TestTile>();

        //Health Bar Set Up
        GameObject healthBarObject = new GameObject("HealthBar");
        healthBarObject.transform.SetParent(enemy.transform);
        healthbar barScript = healthBarObject.AddComponent<healthbar>();
        Slider dummySlider = new GameObject("DummySlider").AddComponent<Slider>();
        dummySlider.transform.SetParent(healthBarObject.transform);
        foreach (var field in typeof(healthbar).GetFields(flags))
        {
            if (field.FieldType == typeof(Slider)) field.SetValue(barScript, dummySlider);
        }

        //Grid Dictionary Set Up
        var gridDict = (Dictionary<Vector2, Tile>)typeof(GridManager).GetField("tiles", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(GridManager.Instance);
        player.OccupiedTile.transform.position = new Vector3(0, 0, 0);
        enemy.OccupiedTile.transform.position = new Vector3(1, 0, 0);
        gridDict[Vector2.zero] = player.OccupiedTile;
        gridDict[Vector2.right] = enemy.OccupiedTile;

        testRoot.SetActive(true);
    }

    [UnityTest]
    public IEnumerator Test_ExecuteCombat()
    {
        //Variables
        enemy.maxHealth = 100f;
        enemy.health = 100f;
        enemy.guard = 0f;
        player.dmg = 25f;
        player.canAttack = true;

        //Sound Manager Test Fix (Bypass)
        if(SoundFXManager.instance != null)
        {
            Object.DestroyImmediate(SoundFXManager.instance.gameObject);
        }


        //Unit Testing
        uiManager.showCombatOption(player, enemy);
        uiManager.ExecuteCombat();
        Assert.AreEqual(75f, enemy.health, "Enemy health should be decreased by player damage");
        Assert.IsFalse(player.canAttack, "player should lose the ability to attack again");

        yield return null;
    }

    [TearDown]
    public void Teardown()
    {
        //removal of game objects
        Object.DestroyImmediate(testRoot);
        if(player != null)
        {
            if(player.OccupiedTile != null)
            {
                Object.DestroyImmediate(player.OccupiedTile.gameObject);
            }
            Object.DestroyImmediate(player.gameObject);
        }
        if(enemy != null)
        {
            if (enemy.OccupiedTile != null)
            {
                Object.DestroyImmediate(enemy.OccupiedTile.gameObject);
            }
            Object.DestroyImmediate(enemy.gameObject);
        }
        if(CardManager.instance != null)
        {
            Object.DestroyImmediate(CardManager.instance.gameObject);
        }
        if (SoundFXManager.instance != null)
        {
            Object.DestroyImmediate(SoundFXManager.instance.gameObject);
        }
        if (GridManager.Instance != null)
        {
            Object.DestroyImmediate(GridManager.Instance.gameObject);
        }

        combatUIManager.Instance = null;
    }
}
