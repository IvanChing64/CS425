using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

//Developer: Bailey Escritor
//Aggregated from multiple tutorials
public class HandManager : MonoBehaviour
{
    /*
    HandManager Responsibilities:
    - Manages the unit's current hand of cards
    - Draws cards from the unit's deck at the start of their turn
    - Shuffles the deck when needed
    */

    public static HandManager instance;
    public GameObject supportCardPrefab, attackCardPrefab, movementCardPrefab;
    //public BaseCard selectedCard;
    [HideInInspector] public List<ScriptableCard> currentDeck = new List<ScriptableCard>();//MinMaxSize: 6, MaxMaxSize: 9
    [HideInInspector] public List<GameObject> currentHand = new List<GameObject>();//Always Size: 3
    [SerializeField] private int maxHandSize = 3;
    public int minDeckSize, maxDeckSize;
    private int deckIndex = 0; // pointer into currentDeck for drawing
    public bool handDrawn = false;
    public bool handSelected = false;
    //public int currentHandSize;

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
    
        if (currentDeck == null || currentDeck.Count == 0)
        {
            FillDeckFromResources();
        }

        //ShuffleDeck();
        //DrawHand();
    }

    public void SetUnitValue(BaseUnit unit, BaseCard card)
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
            ScriptableCard temp = currentDeck[i];
            int randomIndex = Random.Range(i, currentDeck.Count);
            currentDeck[i] = currentDeck[randomIndex];
            currentDeck[randomIndex] = temp;
        }
        Debug.Log("Deck Shuffled");
    }

    public void DrawHand()
    {
        if (handDrawn)
        {
            Debug.LogWarning("Hand already drawn for this turn.");
            return;
        }

        // Remove previously instantiated hand cards from scene
        if (CardManager.instance.selectedCard != null) 
        {
            CardManager.instance.DeselectCard();
        }

        for (int i = 0; i < currentHand.Count; i++)
        {
            if (currentHand[i] != null)
            {
                Destroy(currentHand[i]);
                Debug.Log("Destroyed card: " + currentHand[i].name);
            }
        }
        currentHand.Clear();

        if (currentDeck == null || currentDeck.Count == 0)
        {
            Debug.LogWarning("Current deck is empty - cannot draw cards.");
            return;
        }

        // Draw up to maxHandSize cards from the deck starting at deckIndex
        for (int i = 0; i < maxHandSize; i++)
        {
            if (currentDeck.Count == 0) break;

            //Some AI was used to help implement this function
            int index = deckIndex % currentDeck.Count;
            ScriptableCard card = currentDeck[index];
            GameObject newCard = null;
            Vector3 spawnPos = CardManager.instance.transform.position + new Vector3(i * 3, 0, 0);

            switch (card.type)
            {
                case Type.Support:
                    newCard = Instantiate(supportCardPrefab, spawnPos, Quaternion.identity);
                    break;
                case Type.Attack:
                    newCard = Instantiate(attackCardPrefab, spawnPos, Quaternion.identity);
                    break;
                case Type.Movement:
                    newCard = Instantiate(movementCardPrefab, spawnPos, Quaternion.identity);
                    break;
                default:
                    Debug.LogWarning("Unknown card type: " + card.type);
                    continue;
            }
            currentHand.Add(newCard);
            newCard.GetComponent<BaseCard>().CopyScriptableCard(card);
            deckIndex = (deckIndex + 1) % Mathf.Max(1, currentDeck.Count);
        }

        handDrawn = true;

        Debug.Log("Hand Drawn with " + currentHand.Count + " cards.");
    }

    // Populate `currentDeck` from Player Deck Data
    public void FillDeckFromResources()
    {
        currentDeck.Clear();

        //FindAnyObjectByType<DeckManager>().GetDeck(this);
        DeckManager.instance.GetDeck(this);
        Debug.Log("Deck filled from resources with " + currentDeck.Count + " cards.");
    }

    // Call this to draw a fresh hand
    public void NextTurn()
    {
        handDrawn = false;
        ShuffleDeck();
        DrawHand();
    }

    public void ToggleHandVisibility(bool show)
    {
        if (currentHand.Count == 0)
        {
            Debug.Log("No cards in hand to show.");
            return;
        }

        if (show)
        {
            foreach (GameObject card in currentHand)
            {
                card.SetActive(true);
            }
            handSelected = true;
            Debug.Log("Hand is now shown.");
        } else
        {
            foreach (GameObject card in currentHand)
            {
                card.SetActive(false);
            }
            handSelected = false;
            Debug.Log("Hand is now hidden.");
        }
    }
}