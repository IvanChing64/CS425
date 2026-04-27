using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UnitInfo : MonoBehaviour
{
    public static UnitInfo Instance;

    public GameObject infoPanel;
    public UnitInfoField nameField, healthField, guardField, moveField, rangeField, attackField;
    public UnitInfoField attackModifierField, defenseModifierField, movementModifierField;
    public UnitInfoField lifetimeField;
    public List<UnitInfoField> nonValuedEffectFields;
    public List<UnitInfoField> valuedEffectFields;
    public static Color greyedOutColor = new Color(1, 1, 1, 170f/255f);
    public static Color inactiveColor = new Color(149f/255f, 149f/255f, 149f/255f, 80f/255f);
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

        // Move up other fields
        attackModifierField.gameObject.transform.localPosition = moveField.gameObject.transform.localPosition;
        defenseModifierField.gameObject.transform.localPosition = rangeField.gameObject.transform.localPosition;
        movementModifierField.gameObject.transform.localPosition = attackField.gameObject.transform.localPosition;

        // Update attack modifier field
        attackModifierField.fieldText.text = $"X {player.attackModifier}";
        attackModifierField.gameObject.SetActive(true);

        // Update defense modifier field
        defenseModifierField.fieldText.text = $"X {1 + (1 - player.defenseModifier)}";

        // Update movement modifier field
        if (player.restricted > 0)
        {
            movementModifierField.fieldText.text = "Restricted";
        } else
        {
            movementModifierField.fieldText.text = player.moveModifier + (player.moveModifier == 1 ? " Tile" : " Tiles");
        }
        movementModifierField.gameObject.SetActive(true);

        // Update lifetime field
        if (player.summoned)
        {
            lifetimeField.fieldText.text = player.lifetime - 1 + (player.lifetime - 1 == 1 ? " Turn" : " Turns");
            lifetimeField.gameObject.SetActive(true);
        } else
        {
            lifetimeField.gameObject.SetActive(false);
        }

        // Update effects
        UpdateEffects(player);
    }

    public void WriteEnemyStats(BaseEnemy enemy)
    {
        // Update health field
        healthField.fieldText.text = enemy.health + "/" + enemy.maxHealth;
        healthField.gameObject.SetActive(true);

        // Update guard field
        guardField.fieldText.text = enemy.guard + "/" + enemy.maxHealth;
        guardField.gameObject.SetActive(true);

        // Update movement range field
        int moveRange = enemy.moveRange + enemy.moveModifier;
        if (enemy.restricted > 0)
        {
            moveField.fieldText.text = "Restricted";
            moveField.gameObject.SetActive(true);
        } else
        {
            moveField.fieldText.text = moveRange + (moveRange == 1 ? " Tile" : " Tiles");
            moveField.gameObject.SetActive(true);
        }
        

        // Update attack range field
        int attackRange = enemy.attackRange;
        rangeField.fieldText.text = attackRange + (attackRange == 1 ? " Tile" : " Tiles");
        rangeField.gameObject.SetActive(true);

        // Update attack damage field
        int attackDamage = (int)(enemy.dmg * enemy.attackModifier);
        attackField.fieldText.text = attackDamage + " Dmg";
        attackField.gameObject.SetActive(true);

        // Move up other fields
        defenseModifierField.gameObject.transform.localPosition = new Vector3(defenseModifierField.gameObject.transform.localPosition.x, -150, 0);

        // Update attack modifier field
        attackModifierField.gameObject.SetActive(false);

        // Update defense modifier field
        defenseModifierField.fieldText.text = $"X {1 + (1 - enemy.defenseModifier)}";

        // Update movement modifier field
        movementModifierField.gameObject.SetActive(false);

        // Hide lifetime field
        lifetimeField.gameObject.SetActive(false);

        // Update effects
        UpdateEffects(enemy);
    }

    public void UpdateEffects(BaseUnit unit)
    {
        // Update Non-Valued Effect Fields
        if (unit.boost > 0)
        {
            nonValuedEffectFields[0].fieldImage.color = Color.white;
            if (unit.boost == EffectFlag.End)
            {
                nonValuedEffectFields[0].fieldImage.color = greyedOutColor;
            }
        } else
        {
            nonValuedEffectFields[0].fieldImage.color = inactiveColor;
        }

        if (unit.strengthen > 0)
        {
            nonValuedEffectFields[1].fieldImage.color = Color.white;
            if (unit.strengthen == EffectFlag.End)
            {
                nonValuedEffectFields[1].fieldImage.color = greyedOutColor;
            }
        } else
        {
            nonValuedEffectFields[1].fieldImage.color = inactiveColor;
        }

        if (unit.resistant > 0)
        {
            nonValuedEffectFields[2].fieldImage.color = Color.white;
            if (unit.resistant == EffectFlag.End)
            {
                nonValuedEffectFields[2].fieldImage.color = greyedOutColor;
            }
        } else
        {
            nonValuedEffectFields[2].fieldImage.color = inactiveColor;
        }

        if (unit.defiant == true)
        {
            nonValuedEffectFields[3].fieldImage.color = Color.white;
        } else
        {
            nonValuedEffectFields[3].fieldImage.color = inactiveColor;
        }

        if (unit.immune > 0)
        {
            nonValuedEffectFields[4].fieldImage.color = Color.white;
            if (unit.immune == EffectFlag.End)
            {
                nonValuedEffectFields[4].fieldImage.color = greyedOutColor;
            }
        } else
        {
            nonValuedEffectFields[4].fieldImage.color = inactiveColor;
        }

        if (unit.invisible > 0)
        {
            nonValuedEffectFields[5].fieldImage.color = Color.white;
            if (unit.invisible == EffectFlag.End)
            {
                nonValuedEffectFields[5].fieldImage.color = greyedOutColor;
            }
        } else
        {
            nonValuedEffectFields[5].fieldImage.color = inactiveColor;
        }

        if (unit.hinder > 0)
        {
            nonValuedEffectFields[6].fieldImage.color = Color.white;
            if (unit.hinder == EffectFlag.End)
            {
                nonValuedEffectFields[6].fieldImage.color = greyedOutColor;
            }
        } else
        {
            nonValuedEffectFields[6].fieldImage.color = inactiveColor;
        }

        if (unit.weaken > 0)
        {
            nonValuedEffectFields[7].fieldImage.color = Color.white;
            if (unit.weaken == EffectFlag.End)
            {
                nonValuedEffectFields[7].fieldImage.color = greyedOutColor;
            }
        } else
        {
            nonValuedEffectFields[7].fieldImage.color = inactiveColor;
        }

        if (unit.vulnerable > 0)
        {
            nonValuedEffectFields[8].fieldImage.color = Color.white;
            if (unit.vulnerable == EffectFlag.End)
            {
                nonValuedEffectFields[8].fieldImage.color = greyedOutColor;
            }
        } else
        {
            nonValuedEffectFields[8].fieldImage.color = inactiveColor;
        }

        if (unit.stunned > 0)
        {
            nonValuedEffectFields[9].fieldImage.color = Color.white;
        } else
        {
            nonValuedEffectFields[9].fieldImage.color = inactiveColor;
        }

        if (unit.flaming > 0)
        {
            nonValuedEffectFields[10].fieldImage.color = Color.white;
            if (unit.flaming == EffectFlag.End)
            {
                nonValuedEffectFields[10].fieldImage.color = greyedOutColor;
            }
        } else
        {
            nonValuedEffectFields[10].fieldImage.color = inactiveColor;
        }

        if (unit.frozen > 0)
        {
            nonValuedEffectFields[11].fieldImage.color = Color.white;
            if (unit.frozen == EffectFlag.End)
            {
                nonValuedEffectFields[11].fieldImage.color = greyedOutColor;
            }
        } else
        {
            nonValuedEffectFields[11].fieldImage.color = inactiveColor;
        }

        // Update Valued Effect Fields
        if (unit.dodge > 0)
        {
            valuedEffectFields[0].fieldImage.color = Color.white;
            valuedEffectFields[0].fieldText.text = $"{unit.dodge}";
        } else
        {
            valuedEffectFields[0].fieldImage.color = inactiveColor;
            valuedEffectFields[0].fieldText.text = "";
        }

        if (unit.absorb > 0)
        {
            valuedEffectFields[1].fieldImage.color = Color.white;
            valuedEffectFields[1].fieldText.text = $"{unit.absorb}";
        } else
        {
            valuedEffectFields[1].fieldImage.color = inactiveColor;
            valuedEffectFields[1].fieldText.text = $"";
        }

        if (unit.reflect > 0)
        {
            valuedEffectFields[2].fieldImage.color = Color.white;
            valuedEffectFields[2].fieldText.text = $"{unit.reflect}";
        } else
        {
            valuedEffectFields[2].fieldImage.color = inactiveColor;
            valuedEffectFields[2].fieldText.text = $"";
        }

        if (unit.regeneration > 0)
        {
            valuedEffectFields[3].fieldImage.color = Color.white;
            valuedEffectFields[3].fieldText.text = $"{unit.regeneration}";
        } else
        {
            valuedEffectFields[3].fieldImage.color = inactiveColor;
            valuedEffectFields[3].fieldText.text = $"";
        }

        if (unit.poison > 0)
        {
            valuedEffectFields[4].fieldImage.color = Color.white;
            valuedEffectFields[4].fieldText.text = $"{unit.poison}";
        } else
        {
            valuedEffectFields[4].fieldImage.color = inactiveColor;
            valuedEffectFields[4].fieldText.text = $"";
        }
    }

    public void HidePanel() => infoPanel.SetActive(false);

    public void ShowPanel() => infoPanel.SetActive(true);

}
