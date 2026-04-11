using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitInfo : MonoBehaviour
{
    public static UnitInfo Instance;

    public GameObject infoPanel;
    public Text nameText;
    public Text healthText;
    public Text moveText;
    public Text rangeText;
    public Text damageText;
    //public Text statusText;

    void Awake()
    {
        Instance = this;
        HidePanel();
    }

    public void UpdatePanel()
    {
        BaseUnit selectedUnit = UnitManager.Instance.SelectedUnit;
        if (selectedUnit == null)
        {
            HidePanel();
            return;
        }

        // Update name
        nameText.text = selectedUnit.name;

        // Update health
        int curHealth = (int)selectedUnit.health;
        int maxHealth = (int)selectedUnit.maxHealth;
        damageText.text = curHealth + "/" + maxHealth;

        if (UnitManager.Instance.SelectedEnemy == null)
        {
            moveText.text = "";
            rangeText.text = "";
            damageText.text = "";
        }
        else
        {
            // Update movement range
            int moveRange = selectedUnit.moveRange + selectedUnit.moveModifier;
            moveText.text = moveRange + (moveRange == 1 ? " Tile" : " Tiles");

            // Update attack range
            int attackRange = selectedUnit.attackRange;
            rangeText.text = attackRange + (attackRange == 1 ? " Tile" : " Tiles");
            
            // Update attack damage
            int attackDamage = (int)selectedUnit.dmg;
            damageText.text = attackDamage + " Dmg";
        }

        ShowPanel();
    }

    public void HidePanel() => infoPanel.SetActive(false);

    public void ShowPanel() => infoPanel.SetActive(true);

}
