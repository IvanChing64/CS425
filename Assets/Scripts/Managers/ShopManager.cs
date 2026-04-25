using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// A class for handling the shop and purchasing items
/// </summary>
/// <remarks>Primary author: Liam Riel</remarks>
public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    public static bool StockInitialized = false;
    public static List<ScriptableItem> CurrentItemsInShop;
    public static float RerollCostMultiplier = 1.0f;

    public static bool ItemListsInititialized = false;
    public static List<UnitUpgradeItem> UnitUpgradeItems;
    public static List<NewUnitItem> NewUnitItems;
    //public static List<PartyBuffItem> PartyBuffItems;
    //public static List<DeckAdditionItem> DeckAdditionItems;

    [SerializeField] private Text currencyText;
    [SerializeField] private Text armyCapText;
    [SerializeField] private Text rerollCostText;
    [SerializeField] private int rerollBaseCost;

    public List<ShopItemSlot> itemSlots;

    void Awake()
    {
        Instance = this;

        if (!ItemListsInititialized)
        {
            UnitUpgradeItems = new List<UnitUpgradeItem>(Resources.LoadAll<UnitUpgradeItem>("Items/Unit Upgrades"));
            NewUnitItems = new List<NewUnitItem>(Resources.LoadAll<NewUnitItem>("Items/New Units"));
            ItemListsInititialized = true;
        }

        if (!StockInitialized)
        {
            InitializeStock();
            StockInitialized = true;
        }

        UpdateItemSlots();
        
        UpdateCurrencyText();
        UpdateArmyCapText();
        UpdateRerollCostText();
    }

    /// <summary>
    /// Used when the shop has not yet stocked any items
    /// </summary>
    private void InitializeStock()
    {
        CurrentItemsInShop = new List<ScriptableItem>(itemSlots.Count);

        for (int i = 0; i < itemSlots.Count; i++)
        {
            CurrentItemsInShop.Add(NewUnitItems[0]);
        }

        RestockShop();
    }

    /// <summary>
    /// Called when the attempts to buy the item in an item slot
    /// </summary>
    /// <param name="slot">The item slot to buy the item from</param>
    public void BuyItemInSlot(int slot)
    {
        if (!BuyItem(CurrentItemsInShop[slot])) return;

        RestockItemIn(slot);
        UpdateItemSlot(slot);
    }

    /// <summary>
    /// Checks if the player has enough money to buy an item and if so, buys it and applies its effect
    /// </summary>
    /// <param name="item">The item to buy</param>
    /// <returns>Whether the item was bought</returns>
    public bool BuyItem(ScriptableItem item)
    {
        // If we can't purchase the upgrade, return false
        bool purchasable = ArmyManager.Instance.GetCurrency() >= item.cost;
        if (!purchasable) return false;

        switch (item.Type)
        {
            // If we are buying a unit upgrade item
            case ItemType.UnitUpgrade:
                UnitUpgradeItem upgradeItem = item as UnitUpgradeItem;
                if (ArmyManager.Instance.RemoveUnit(upgradeItem.originalUnit))
                    ArmyManager.Instance.AddUnit(upgradeItem.upgradedUnit);
                else
                {
                    Debug.Log("Original unit not found in army");
                    return false;
                }

                break;

            // If we are buying a new unit
            case ItemType.NewUnit:
                NewUnitItem newUnitItem = item as NewUnitItem;
                bool added = ArmyManager.Instance.AddUnit(newUnitItem.newUnit);
                if (!added)
                {
                    Debug.Log("Army at capacity, new unit cannot be added");
                    return false;
                }
                break;

            default:
                Debug.Log("Invalid shop item type");
                break;
        }

        ArmyManager.Instance.AttemptPurchase(item.cost);
        UpdateArmyCapText();
        UpdateCurrencyText();
        return true;
    }

    /// <summary>
    /// Restocks/rerolls the statically stored shop items
    /// </summary>
    /// <remarks>Does not update the item slots shown in the shop</remarks>
    public void RestockShop()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            RestockItemIn(i);
        }
    }

    /// <summary>
    /// Restocks/rerolls one of the statically stored shop items
    /// </summary>
    /// <param name="slot">The item slot/index to restock</param>
    public void RestockItemIn(int slot)
    {
        ScriptableItem itemToAdd;

        switch (slot)
        {
            // The first two slots should stock a new unit
            case < 2:
                itemToAdd = NewUnitItems[Random.Range(0, NewUnitItems.Count)];
                break;

            // The next two slots should stock unit upgrades, if there are no possible unit upgrades stock new units instead
            case < 4:
                itemToAdd = GetApplicableUpgradeItem();
                if (itemToAdd == null) itemToAdd = NewUnitItems[Random.Range(0, NewUnitItems.Count)];
                break;
            default:
                Debug.Log("Shop item slot Out of Bounds or upgrade type undefined");
                return;
        }

        CurrentItemsInShop[slot] = itemToAdd;
    }

    /// <summary>
    /// Finds and returns a possible unit upgrade item the player can use, unit upgrades should not be for units the player does not have or that are already in stock
    /// </summary>
    /// <returns>A random unit upgrade of possible ones the player can obtain, null if there are none</returns>
    private UnitUpgradeItem GetApplicableUpgradeItem()
    {
        List<UnitUpgradeItem> applicableList = new(UnitUpgradeItems.Where(item => ArmyManager.Instance.HasUnit(item.originalUnit) && !CurrentItemsInShop.Contains(item)));

        if (applicableList.Count == 0) return null;

        return applicableList[Random.Range(0, applicableList.Count)];
    }

    /// <summary>
    /// The reroll button uses this function to attempt to reroll the shop and update the reroll cost
    /// </summary>
    public void RerollButton()
    {
        // If we can't purchase a reroll, return;
        int rerollCost = (int)Math.Round(rerollBaseCost * RerollCostMultiplier);
        bool purchased = ArmyManager.Instance.AttemptPurchase(rerollCost);
        if (!purchased) return;

        RerollShop();

        // Update the reroll cost (multiplier)
        RerollCostMultiplier += 0.5f;
        UpdateRerollCostText();
        UpdateCurrencyText();
    }

    /// <summary>
    /// Restocks/rerolls the static shop items and updates the shop scene's slots to match
    /// </summary>
    public void RerollShop()
    {
        RestockShop();
        UpdateItemSlots();
    }

    /// <summary>
    /// Updates the visual item slots in the shop scene to match the internal shop stock
    /// </summary>
    public void UpdateItemSlots()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            UpdateItemSlot(i);
        }
    }

    /// <summary>
    /// Updates the specified item slot in the shop scene to reflect the static shop stock
    /// </summary>
    /// <param name="slot"></param>
    public void UpdateItemSlot(int slot)
    {
        itemSlots[slot].itemImage.sprite = CurrentItemsInShop[slot].itemSprite;
        itemSlots[slot].itemImage.SetNativeSize();

        string desc;
        if (CurrentItemsInShop[slot] is NewUnitItem nuItem)
        {
            desc = $"New {nuItem.newUnit.name} for army";
        }
        else if (CurrentItemsInShop[slot] is UnitUpgradeItem uuItem)
        {
            desc = $"Upgrade {uuItem.originalUnit.name} to {uuItem.upgradedUnit.name}";
        }
        else
        {
            desc = $"Buy {CurrentItemsInShop[slot].Type.ToString()}";
        }

        itemSlots[slot].itemDescription.text = desc;

        itemSlots[slot].costText.text = $"Cost: {CurrentItemsInShop[slot].cost}";
    }

    /// <summary>
    /// Updates the currency text with the player's current gold
    /// </summary>
    public void UpdateCurrencyText()
    {
        currencyText.text = $"Gold: {ArmyManager.Instance.GetCurrency()}";
    }

    /// <summary>
    /// Updates the player's army capacity text with the player's army size, and turns it red if it's full
    /// </summary>
    public void UpdateArmyCapText()
    {
        int armyCount = ArmyManager.Instance.unitsInArmy.Count;
        int armyCap = ArmyManager.Instance.ArmyCapacity;
        armyCapText.text = $"{armyCount} / {armyCap} units";

        armyCapText.color = (armyCount >= armyCap)? Color.softRed : Color.whiteSmoke;
    }

    /// <summary>
    /// Updates the text for the cost of rerolls
    /// </summary>
    public void UpdateRerollCostText()
    {
        rerollCostText.text = $"Cost: {(int)Math.Round(rerollBaseCost * RerollCostMultiplier)}";
    }
}
