using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitInfo : MonoBehaviour
{
    public static UnitInfo Instance;

    public GameObject infoPanel;
    public UnitInfoField nameField, healthField, guardField, moveField, rangeField, attackField;
    //public UnitInfoField statusField;

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

        // Write name field
        nameField.fieldText.text = selectedUnit.name;

        if (selectedUnit is BasePlayer p)
        {
            WritePlayerStats(p);
        }
        else if (selectedUnit is BaseEnemy e)
        {
            WriteEnemyStats(e);
        }
        else
        {
            HidePanel();
            return;
        }

        ShowPanel();
    }

    public void WritePlayerStats(BasePlayer player)
    {
        // Update health field
        healthField.fieldText.text = player.health + "/" + player.maxHealth;
        healthField.gameObject.SetActive(true);

        // Update guard field
        guardField.fieldText.text = player.guard + "/" + player.maxHealth;
        guardField.gameObject.SetActive(true);

        // Hide other fields
        moveField.gameObject.SetActive(false);
        rangeField.gameObject.SetActive(false);
        attackField.gameObject.SetActive(false);

        // Update status field
        //statusField.fieldTransform.localPosition = new Vector3(0, -160, 0);
        //StringBuilder statusText = new StringBuilder();
        //if (player.boost > EffectFlag.None) statusText.AppendLine("Boosted");
        //statusField.fieldText.text = statusText.ToString();

    }

    public void WriteEnemyStats(BaseEnemy enemy)
    {
        // Update health field
        healthField.fieldText.text = enemy.health + "/" + enemy.maxHealth;
        healthField.gameObject.SetActive(true);

        // Update guard field
        guardField.gameObject.SetActive(false);

        // Update movement range field
        int moveRange = enemy.moveRange + enemy.moveModifier;
        moveField.fieldText.text = moveRange + (moveRange == 1 ? " Tile" : "Tiles");
        moveField.gameObject.SetActive(true);

        // Update attack range field
        int attackRange = enemy.attackRange;
        rangeField.fieldText.text = attackRange + (attackRange == 1 ? " Tile" : " Tiles");
        rangeField.gameObject.SetActive(true);

        // Update attack damage field
        int attackDamage = (int)(enemy.dmg * enemy.attackModifier + 0.5);
        attackField.fieldText.text = attackDamage + " Dmg";
        attackField.gameObject.SetActive(true);

        // Update status field
        //statusField.fieldTransform.localPosition = new Vector3(0, -220, 0);
        //StringBuilder statusText = new StringBuilder();
        //if (enemy.stunned > EffectFlag.None) statusText.AppendLine("Stunned");
        //statusField.fieldText.text = statusText.ToString();
    }

    public void HidePanel() => infoPanel.SetActive(false);

    public void ShowPanel() => infoPanel.SetActive(true);

}
