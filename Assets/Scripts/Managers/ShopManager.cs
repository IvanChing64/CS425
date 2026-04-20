using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    public static List<ScriptableItem> CurrentItemsInShop;

    public static bool ItemListsInititialized = false;
    public static List<UnitUpgradeItem> UnitUpgradeItems;
    public static List<NewUnitItem> NewUnitItems;
    //public static List<PartyBuffItem> PartyBuffItems;
    //public static List<DeckAdditionItem> DeckAdditionItems;

    void Awake()
    {
        Instance = this;

        if (ItemListsInititialized) return;

        UnitUpgradeItems = new List<UnitUpgradeItem>(Resources.LoadAll<UnitUpgradeItem>("Items"));
        NewUnitItems = new List<NewUnitItem>(Resources.LoadAll<NewUnitItem>("Items"));
    }

    public bool BuyItem(ScriptableItem item)
    {
        bool purchased = ArmyManager.Instance.AttemptPurchase(item.cost);
        if (!purchased) return false;

        switch (item.Type)
        {
            case ItemType.UnitUpgrade:
                UnitUpgradeItem upgradeItem = item as UnitUpgradeItem;
                if (ArmyManager.Instance.RemoveUnit(upgradeItem.originalUnit))
                    ArmyManager.Instance.AddUnit(upgradeItem.upgradedUnit);
                else
                    Debug.Log("Original unit not found in army");
                break;

            case ItemType.NewUnit:
                NewUnitItem newUnitItem = item as NewUnitItem;
                ArmyManager.Instance.AddUnit(newUnitItem.newUnit);
                break;

            case ItemType.PartyBuff:
                Debug.Log("Party buff item not implemented");
                break;

            case ItemType.DeckAddition:
                Debug.Log("Deck addition item not implemented");
                // ArmyManager.Instance.AddCardToDecks(deckAddItem.card);
                break;

            default:
                Debug.Log("Invalid shop item type");
                break;
        }

        return true;
    }

    public void RestockShop()
    {
        // The first two shop items should be new units
        // TODO: Ensure that two *different* items are always shown
        CurrentItemsInShop[0] = NewUnitItems[Random.Range(0, NewUnitItems.Count)];
        CurrentItemsInShop[1] = NewUnitItems[Random.Range(0, NewUnitItems.Count)];

        // The next three? (maybe two) items should be unit upgrades
        CurrentItemsInShop[2] = UnitUpgradeItems[Random.Range(0, UnitUpgradeItems.Count)];
        CurrentItemsInShop[3] = UnitUpgradeItems[Random.Range(0, UnitUpgradeItems.Count)];
        CurrentItemsInShop[4] = UnitUpgradeItems[Random.Range(0, UnitUpgradeItems.Count)];

        // The next can be whatever
    }

    public void RestockItemIn(int slot)
    {
        CurrentItemsInShop[slot] = null;
    }

    public void TestFunction()
    {
        ArmyManager.Instance.RemoveUnit("Knight");
    }
}
