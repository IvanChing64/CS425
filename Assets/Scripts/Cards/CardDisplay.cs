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
            overrideValue = unit.attackBoost;
        } else if (cardData.type == Type.Movement)
        {
            //overrideValue = GetComponentInParent<BaseCard>().GetComponentInParent<BaseUnit>().moveRange;
        } else
        {
            overrideValue = cardData.value;
        }
        cardName.text = cardData.cardName;
        cardDescription.text = cardData.cardDescription;
        cardValue.text = cardData.value.ToString();
        if (cardData.type == Type.Attack && overrideValue != 1f)
        {
            cardValue.text = ((int)(cardData.value * overrideValue)).ToString();
            if (overrideValue > 1f)
            {
                cardValue.color = Color.red;
            } else if (overrideValue < 1f)
            {
                cardValue.color = Color.blue;
            }
        }
        cardCost.text = cardData.cost.ToString();

        cardBorder.color = cardData.cardBorderColor;
        cardIcon.sprite = cardData.cardIcon;
        cardCorner.sprite = cardData.cardCorner;
    }    
}
