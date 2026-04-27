using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArmySceneManager : MonoBehaviour
{
    public static ArmySceneManager instance;
    public static bool objectsInitialized = false;

    public List<ScriptableArmySlot> scriptableArmySlots;
    public List<UnitArmySlot> armySlots;
    public Text armySize;
    public TMP_Text unitName, health, energy, description;
    public TMP_Text attack, movement, support, summon;
    public TMP_Text stats, desc, cards;
    public Image attackIcon, moveIcon, suppIcon, summIcon;

    void Awake()
    {
        instance = this;

        ScriptableArmySlot[] slots = Resources.LoadAll<ScriptableArmySlot>("Army");
        scriptableArmySlots.AddRange(slots);
        armySize.text = $"Army: {ArmyManager.Instance.unitsInArmy.Count} / 6";
        

        UpdateArmySlots();
        ClearInfoPanel();
    }

    // Updates all the army slots
    public void UpdateArmySlots()
    {
        for (int i = 0; i < ArmyManager.Instance.unitsInArmy.Count; i++)
        {
            UpdateArmySlot(i, ArmyManager.Instance.unitsInArmy[i].name);
        }
    }

    // Update the given army slot
    public void UpdateArmySlot(int index, string name)
    {
        ScriptableArmySlot foundUnit = scriptableArmySlots.Find(unit => unit.name == name);
        

        if (foundUnit == null)
        {
            return;
        }
        armySlots[index].unitImage.sprite = foundUnit.unitImage;
        armySlots[index].unitImage.SetNativeSize();

        armySlots[index].armyName.text = foundUnit.name;
        armySlots[index].health.text = $"HP: {foundUnit.health}";
        armySlots[index].energy.text = $"Energy: {foundUnit.energy}";

    }


    // Update the army info panel
    public void UpdateInfoPanel(ScriptableArmySlot unit, int index)
    {
        // Hide Titles and Icons
        stats.enabled = true;
        desc.enabled = true;
        cards.enabled = true;
        attackIcon.enabled = true;
        moveIcon.enabled = true;
        suppIcon.enabled = true;
        summIcon.enabled = true;

        if (index > ArmyManager.Instance.unitsInArmy.Count)
        {
            return;
        }

        // Update Unit Stat Info and Description
        unitName.text = unit.name;
        health.text = $"HP: {unit.health}";
        energy.text = $"Energy: {unit.energy}";
        description.text = unit.description;

        // Update Unit Card Info
        BasePlayer un = (BasePlayer)ArmyManager.Instance.unitsInArmy[index].UnitPrefab;
        int move = 0, att = 0, supp = 0, summ = 0;

        foreach (string card in un.startingDeck)
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

        movement.text = move.ToString();
        attack.text = att.ToString();
        support.text = supp.ToString();
        summon.text = summ.ToString();
    }

    // Clear army info panel
    public void ClearInfoPanel()
    {
        // Clear Unit Stat Info and Description
        unitName.text = "";
        health.text = "";
        energy.text = "";
        description.text = "";

        // Clear Card Info
        movement.text = "";
        attack.text = "";
        support.text = "";
        summon.text = "";

        // Hide Titles and Icons
        stats.enabled = false;
        desc.enabled = false;
        cards.enabled = false;
        attackIcon.enabled = false;
        moveIcon.enabled = false;
        suppIcon.enabled = false;
        summIcon.enabled = false;
    }

    // Army Slot Button
    public void ArmyButton(int slotNum)
    {
        if (slotNum > ArmyManager.Instance.unitsInArmy.Count - 1)
        {
            ClearInfoPanel();
            return;
        }

        ScriptableArmySlot foundUnit = scriptableArmySlots.Find(unit => unit.name == ArmyManager.Instance.unitsInArmy[slotNum].name);

        UpdateInfoPanel(foundUnit, slotNum);
    }

    
}
