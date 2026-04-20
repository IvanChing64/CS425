using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    public static List<ScriptableItem> CurrentItemsInShop;

    void Awake()
    {
        Instance = this;
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
        CurrentItemsInShop[0] = new NewUnitItem();
        CurrentItemsInShop[1] = new NewUnitItem();

        // The next three? (maybe two) items should be unit upgrades
        CurrentItemsInShop[2] = new UnitUpgradeItem();
        CurrentItemsInShop[3] = new UnitUpgradeItem();
        CurrentItemsInShop[4] = new UnitUpgradeItem();

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
