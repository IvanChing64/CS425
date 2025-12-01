using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;
    public BaseCard selectedCard;
    public List<BaseCard> currentDeck = new List<BaseCard>();
    public List<BaseCard> currentHand = new List<BaseCard>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SelectCard(BaseCard card)
    {
        selectedCard = card;
        Debug.Log("Card Selected: " + card.name);
    }

    void DeselectCard()
    {
        Debug.Log("Card Deselected: " + selectedCard.name);
        selectedCard = null;
    }

    void PlaySelectedCard()
    {
        if (selectedCard != null)
        {
            selectedCard.PlayCard();
            DeselectCard();
        }
        else
        {
            Debug.Log("No card selected to play.");
        }
    }

    void SetUnitValue(BaseUnit unit)
    {
        // Implementation for setting unit values based on the played card
    }

    void ShowInfo(BaseCard card)
    {
        // Implementation for displaying card information
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < currentDeck.Count; i++)
        {
            BaseCard temp = currentDeck[i];
            int randomIndex = Random.Range(i, currentDeck.Count);
            currentDeck[i] = currentDeck[randomIndex];
            currentDeck[randomIndex] = temp;
        }
        Debug.Log("Deck Shuffled");
    }

    void DrawHand()
    {
        currentHand.Clear();
        for (int i = 0; i < 5 && currentDeck.Count > 0; i++)
        {
            BaseCard drawnCard = currentDeck[0];
            currentDeck.RemoveAt(0);
            currentHand.Add(drawnCard);
        }
        Debug.Log("Hand Drawn with " + currentHand.Count + " cards.");
    }
}
