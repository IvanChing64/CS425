using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

    private void InitializeStock()
    {
        CurrentItemsInShop = new List<ScriptableItem>(itemSlots.Count);

        for (int i = 0; i < itemSlots.Count; i++)
        {
            CurrentItemsInShop.Add(NewUnitItems[0]);
        }

        RestockShop();
    }

    public void BuyItemInSlot(int slot)
    {
        if (!BuyItem(CurrentItemsInShop[slot])) return;

        RestockItemIn(slot);
        UpdateItemSlot(slot);
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

        UpdateArmyCapText();
        UpdateCurrencyText();
        return true;
    }

    public void RestockShop()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            RestockItemIn(i);
        }
    }

    public void RestockItemIn(int slot)
    {
        ScriptableItem itemToAdd;

        switch (slot)
        {
            case < 2:
                itemToAdd = NewUnitItems[Random.Range(0, NewUnitItems.Count)];
                break;
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

    private UnitUpgradeItem GetApplicableUpgradeItem()
    {
        List<UnitUpgradeItem> applicableList = new(UnitUpgradeItems.Where(item => ArmyManager.Instance.HasUnit(item.originalUnit) && !CurrentItemsInShop.Contains(item)));

        if (applicableList.Count == 0) return null;

        return applicableList[Random.Range(0, applicableList.Count)];
    }

    public void RerollButton()
    {
        int rerollCost = (int)Math.Round(rerollBaseCost * RerollCostMultiplier);
        bool purchased = ArmyManager.Instance.AttemptPurchase(rerollCost);
        if (!purchased) return;

        RerollShop();
        RerollCostMultiplier += 0.5f;

        UpdateRerollCostText();
    }

    public void RerollShop()
    {
        RestockShop();
        UpdateItemSlots();
    }

    public void UpdateItemSlots()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            UpdateItemSlot(i);
        }
    }

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

    public void UpdateCurrencyText()
    {
        currencyText.text = $"Gold: {ArmyManager.Instance.GetCurrency()}";
    }

    public void UpdateArmyCapText()
    {
        int armyCount = ArmyManager.Instance.unitsInArmy.Count;
        int armyCap = ArmyManager.Instance.ArmyCapacity;
        armyCapText.text = $"{armyCount} / {armyCap} units";

        if (armyCount >= armyCap)
        {
            armyCapText.color = Color.softRed;
        }
    }

    public void UpdateRerollCostText()
    {
        rerollCostText.text = $"Cost: {(int)Math.Round(rerollBaseCost * RerollCostMultiplier)}";
    }
}
