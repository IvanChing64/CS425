using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine.TestTools;
using UnityEngine.UI;
using System.Reflection;
using NUnit.Framework.Internal;
using System.Linq;
using UnityEngine.XR;

// Author: Bailey Escritor
// Card Draw

public class CardTests
{
    private GameObject testRoot;
    private DeckManager testDeckManager;
    private BasePlayer testPlayer;

    [SetUp]
    public void SetUp()
    {
        testRoot = new GameObject("DrawTestRoot");

        //Unit Set Up
        testPlayer = new GameObject("Player").AddComponent<BasePlayer>();
        testDeckManager = testRoot.AddComponent<DeckManager>();
        new GameObject("CardManager").AddComponent<CardManager>();

        testPlayer.gameObject.AddComponent<HandManager>();

        testRoot.SetActive(true);
        
    }

    [UnityTest]
    public IEnumerator Test_Draw()
    {
        testPlayer.GetComponent<HandManager>().handDrawn = false;
        testPlayer.startingDeck.Add("March");
        testPlayer.startingDeck.Add("Guard");
        testPlayer.startingDeck.Add("Slash");
        testPlayer.GetComponent<HandManager>().drawNum = 3;
        testPlayer.GetComponent<HandManager>().FillDeckFromResources();
        testPlayer.GetComponent<HandManager>().DrawHand();

        Assert.AreEqual(3, testPlayer.GetComponent<HandManager>().currentHand.Count, "Player should have a hand of 3");
        Assert.IsTrue(testPlayer.GetComponent<HandManager>().handDrawn, "Player should lose the ability to draw their hand");
        Assert.IsTrue(testPlayer.GetComponent<HandManager>().currentHand[0].cardName == testPlayer.startingDeck[0], "First Card should be March");
        Assert.IsTrue(testPlayer.GetComponent<HandManager>().currentHand[1].cardName == testPlayer.startingDeck[1], "First Card should be Guard");
        Assert.IsTrue(testPlayer.GetComponent<HandManager>().currentHand[2].cardName == testPlayer.startingDeck[2], "First Card should be Slash");
        yield return null;
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(testRoot);
        Object.DestroyImmediate(testDeckManager);
        if(testPlayer != null)
        {
            Object.DestroyImmediate(testPlayer.gameObject);
        }

        if(CardManager.instance != null)
        {
            Object.DestroyImmediate(CardManager.instance.gameObject);
        }
    }
}
