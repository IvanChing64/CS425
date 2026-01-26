using UnityEngine;
using TMPro;
using UnityEngine.UI;

//Developer: Bailey Escritor
//Displays card info from ScriptableCard on the card prefab
public class CardDisplay : MonoBehaviour
{

    public ScriptableCard cardData;
    public Image cardBorder, cardIcon;
    public TMP_Text cardName, cardDescription, cardValue;
    public int overrideValue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        updateCardDisplay();
    }

    // Updates the card display with the current card data
    public void updateCardDisplay()
    {
        cardName.text = cardData.cardName;
        cardDescription.text = cardData.cardDescription;
        cardValue.text = cardData.value.ToString();

        cardBorder.sprite = cardData.cardBorder;
        cardIcon.sprite = cardData.cardIcon;
    }    


}
