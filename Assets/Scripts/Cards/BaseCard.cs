using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

//Developer: Bailey Escritor
//Aggregated from multiple tutorials, Mainly Sinuous Deckbuilding Card Game Tutorials
//Base class for all card types
public abstract class BaseCard : MonoBehaviour
{
    //public BaseCard instance;
    //[SerializeField] private GameObject highlightEffect;
    public string cardName;
    public Type cardType;
    public int range, value, cost;
    public bool isPlayed, AoE;
    public GameObject cardHolder; // Reference to the card holder GameObject

    //Play the card's effect, overridden in derived classes
    public virtual void PlayCard()
    {
        CardManager.instance.selectedPlayer.GetComponent<HandManager>().actionPoints -= cost;
        isPlayed = true;
    }

    public virtual void SelectCard()
    {
        Debug.Log("Base Card Selected");
    }

    public virtual void DeselectCard()
    {
        Debug.Log("Base Card Deselected");
    }

    //Copies properties from ScriptableCard
    public virtual void CopyScriptableCard(ScriptableCard card)
    {
        cardName = card.cardName;
        cardType = card.type;
        value = card.value;
        range = card.range;
        cost = card.cost;
        AoE = card.areaEffect;
        //Debug.Log("Card Copied: " + cardName);
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
                CardManager.instance.DeselectCard();
                //CardManager.instance.PlaySelectedCard();
            }
            else
            {
                Debug.Log("Select a unit.");
            }
        }
        else
        {
            CardManager.instance.SelectCard(this);
            SelectCard();
        }
    }
}
