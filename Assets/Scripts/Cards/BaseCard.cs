using UnityEngine;
using UnityEngine.UI;

//Developer: Bailey Escritor
//Aggregated from multiple tutorials, Mainly Sinuous Deckbuilding Card Game Tutorials
//Base class for all card types
public abstract class BaseCard : MonoBehaviour
{
    public string cardName;
    public Type cardType;
    public int range, areaRange, value, cost;
    public AreaOfEffectType AoE;
    public RangeType rangeType;
    public bool selected, pierce;
    [Tooltip("Game Object for visible card")]
    public GameObject cardHolder; // Reference to the card holder GameObject

    //Play the card's effect, overridden in derived classes
    public virtual void PlayCard()
    {
        CardManager.instance.selectedPlayer.GetComponent<HandManager>().actionPoints -= cost;
    }

    public virtual void SelectCard()
    {
        selected = true;
        //Debug.Log("Base Card Selected");
    }

    public virtual void DeselectCard()
    {
        selected = false;
        //Debug.Log("Base Card Deselected");
    }

    //Copies properties from ScriptableCard
    public virtual void CopyScriptableCard(ScriptableCard card)
    {
        cardName = card.cardName;
        cardType = card.type;
        value = card.value;
        range = card.range;
        areaRange = card.areaRange;
        cost = card.cost;
        AoE = card.areaEffect;
        rangeType = card.rangeType;
    }

    public virtual void ButtonPressed()
    {
        if (GameManager.Instance.gameState != GameState.PlayerTurn) return;
        if (CardManager.instance == null)
        {
            Debug.LogWarning("CardManager.instance is null.");
            return;
        }
        if(CardManager.instance.selectedCard == this)
        {
            if(UnitManager.Instance.SelectedPlayer != null)
            {
                moveCard(false);
                CardManager.instance.DeselectCard();
            }
        }
        else
        {
            if (CardManager.instance.selectedCard != null)
            {
                CardManager.instance.selectedCard.DeselectCard();
                Vector3 posit = new Vector3(CardManager.instance.selectedCard.cardHolder.transform.position.x, 100, CardManager.instance.selectedCard.cardHolder.transform.position.z);
                CardManager.instance.moveCard(CardManager.instance.selectedCard.cardHolder, false, false, posit);
            }
            
            CardManager.instance.SelectCard(this);
            SelectCard();
        }
    }

    public void moveCard(bool forward)
    {
        if (selected) return;
        if (forward)
        {
            Vector3 posit = new Vector3(cardHolder.transform.position.x, 140, cardHolder.transform.position.z);
            CardManager.instance.moveCard(cardHolder, forward, true, posit);
        } else
        {
            Vector3 posit = new Vector3(cardHolder.transform.position.x, 96, cardHolder.transform.position.z);
            CardManager.instance.moveCard(cardHolder, forward, true, posit);
        }
    }
}
