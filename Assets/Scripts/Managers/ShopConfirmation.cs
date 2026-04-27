using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopConfirmation : MonoBehaviour
{

    [SerializeField] private RectTransform confPanel;
    [SerializeField] private Coroutine moveRoutine;
    [SerializeField] private List<ScriptableArmySlot> units;
    [SerializeField] private List<TMP_Text> textFields; 
    [SerializeField] private int selectedSlot; 

    // Loads scriptable army slots and hides confirmation panel
    void Start()
    {
        ScriptableArmySlot[] slots = Resources.LoadAll<ScriptableArmySlot>("Army");
        units.AddRange(slots);
        confPanel.gameObject.SetActive(false);
        ShowConfPanel(false);
    }

    /// <summary>
    /// Hides or shows the confirmation panel
    /// </summary>
    public void ShowConfPanel(bool show)
    {
        confPanel.gameObject.SetActive(true);

        Vector3 showPosit = new Vector3(0f, 0f, 0f);
        Vector3 hidePosit = new Vector3(0f, -1000f, 0f);
        Vector3 posit;

        if (show)
        {
            posit = showPosit;
            if (moveRoutine != null)
            {
                StopCoroutine(MoveConfPanel(hidePosit));
            }
        } else
        {
            posit = hidePosit;
            if (moveRoutine != null)
            {
                StopCoroutine(MoveConfPanel(showPosit));
            }
        }

        moveRoutine = StartCoroutine(MoveConfPanel(posit));
    }

    /// <summary>
    /// Hides or shows the confirmation panel
    /// </summary>
    private IEnumerator MoveConfPanel(Vector3 position)
    {
        Vector3 startPos = confPanel.anchoredPosition;
        float duration = 0.1f;
        float time = 0;
        while(time < duration)
        {
            confPanel.anchoredPosition = Vector2.Lerp(startPos, position, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        confPanel.anchoredPosition = position;
        moveRoutine = null;
    }

    /// <summary>
    /// Updates the text for confirmation panel
    /// </summary>
    public void UpdateConfPanel(int index)
    {
        selectedSlot = index;
        ShowConfPanel(true);

        ScriptableUnit uni = null;
        if (ShopManager.CurrentItemsInShop[index].Type == ItemType.UnitUpgrade)
        {
            uni = ((UnitUpgradeItem)ShopManager.CurrentItemsInShop[index]).upgradedUnit;
        } else if (ShopManager.CurrentItemsInShop[index].Type == ItemType.NewUnit)
        {
            uni = ((NewUnitItem)ShopManager.CurrentItemsInShop[index]).newUnit;
        }

        ScriptableArmySlot foundUnit = units.Find(unit => unit.name == uni.name);

        // Update Unit Stat Info and Description
        textFields[0].text = foundUnit.name;
        textFields[1].text = $"HP: {foundUnit.health}";
        textFields[2].text = $"Energy: {foundUnit.energy}";
        textFields[3].text = foundUnit.description;

        // Update Unit Card Info
        //BasePlayer un = (BasePlayer)ArmyManager.Instance.unitsInArmy[index].UnitPrefab;
        int move = 0, att = 0, supp = 0, summ = 0;

        foreach (string card in ((BasePlayer)uni.UnitPrefab).startingDeck)
        {
            switch (DeckManager.instance.GetCardByName(card).type)
            {
                case Type.Movement:
                    move++;
                    break;

                case Type.Attack:
                    att++;
                    break;

                case Type.Support:
                    supp++;
                    break;

                case Type.Summon:
                    summ++;
                    break;
            }
        }

        textFields[4].text = move.ToString();
        textFields[5].text = att.ToString();
        textFields[6].text = supp.ToString();
        textFields[7].text = summ.ToString();
    }

    /// <summary>
    /// Updates the text for confirmation panel
    /// </summary>
    public void ClearConfPanel()
    {
        ShowConfPanel(false);
        foreach (TMP_Text text in textFields)
        {
            text.text = "";
        }
    }

    /// <summary>
    /// Buy selected unit using buy button
    /// </summary>
    public void BuyItem()
    {
        int currency = ArmyManager.Instance.GetCurrency();
        ShopManager.Instance.BuyItemInSlot(selectedSlot);
        if (currency != ArmyManager.Instance.GetCurrency())
        {
            ClearConfPanel();
        }
        
    }
}
