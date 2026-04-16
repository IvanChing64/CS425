using UnityEngine;
using TMPro;
using UnityEngine.UI;

//Developer: Bailey Escritor
//Displays card info from ScriptableCard on the card prefab
public class CardDisplay : MonoBehaviour
{

    public ScriptableCard cardData;
    public Image cardBorder;
    public Image cardIcon, cardCorner;
    public TMP_Text cardName, cardDescription, cardValue, cardCost;
    public float overrideValue = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updateCardDisplay(CardManager.instance.selectedPlayer);
    }

    // Updates the card display with the current card data
    public void updateCardDisplay(BaseUnit unit)
    {
        if (cardData.type == Type.Attack)
        {
            overrideValue = unit.attackModifier;
        } else if (cardData.type == Type.Movement)
        {
            overrideValue = unit.moveModifier;
        } else
        {
            overrideValue = cardData.value;
        }
        cardName.text = cardData.cardName;
        cardDescription.text = cardData.cardDescription;
        cardValue.color = Color.black;
        cardValue.text = cardData.value.ToString();

        switch (cardData.type)
        {
            case Type.Attack:
                if (cardData.cardName == "Backstab" && CardManager.instance.selectedPlayer != null && CardManager.instance.selectedPlayer.invisible > 0)
                {
                    cardValue.text = ((int)((cardData.value + UnitManager.backstabInvisibleBonus) * overrideValue)).ToString();
                } else
                {
                    cardValue.text = ((int)(cardData.value * overrideValue)).ToString();
                }
                if (cardData.damaging)
                {
                    if (overrideValue > 1f)
                    {
                        cardValue.color = Color.red;
                    } else if (overrideValue < 1f)
                    {
                        cardValue.color = Color.blue;
                    }
                }
                
                break;
        
            case Type.Movement:
                if (overrideValue > 0f)
                {
                    cardValue.color = Color.blue;
                } else if (overrideValue < 0f)
                {
                    cardValue.color = Color.red;
                    if (-overrideValue >= cardData.value)
                    {
                        cardValue.text = "0";
                        break;
                    }
                }
                cardValue.text = ((int)(cardData.value + overrideValue)).ToString();
                break;
        }

        if (cardData.cost >= 0)
        {
            cardCost.text = cardData.cost.ToString();
        } else
        {
            cardCost.text = "0";
        }

        cardBorder.color = cardData.cardBorderColor;
        cardIcon.sprite = cardData.cardIcon;
        cardCorner.sprite = cardData.cardCorner;
    }    
}
