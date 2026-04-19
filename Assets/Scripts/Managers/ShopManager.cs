using System;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    public static List<ScriptableItem> ItemsInShop;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
    }

    bool BuyItem(ScriptableItem item)
    {
        bool purchased = ArmyManager.Instance.AttemptPurchase(item.cost);
        if (!purchased) return false;

        switch (item.type)
        {
            case ItemType.UnitUpgrade:
                break;
            case ItemType.NewUnit:
                break;
            case ItemType.PartyBuff:
                break;
            case ItemType.DeckAddition:
                break;
            default:
                Debug.Log("Invalid shop item type");
                break;
        }

        return true;
    }

    void RestockShop()
    {

    }

    void RestockItemIn(int slot)
    {
        ItemsInShop[slot] = null;
    }
}
