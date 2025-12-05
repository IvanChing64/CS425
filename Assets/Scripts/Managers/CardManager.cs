using System.Collections.Generic;
using UnityEngine;

//Developer: Bailey Escritor
//Aggregated from multiple tutorials
public class CardManager : MonoBehaviour
{
    /*
    Make background elements for card area, better than disabling card prefabs
    Move Cards to bottom of screen, change camera output
    Implement Separate Deck Manager to better handle individual player decks
    Maybe instead of instantiating card prefabs, have a pool of card objects to reuse
    Known In-Editor Glitch: When using inspector on CardManager, opening the list of testDeckPrefabs can cause Editor errors. testDeckPrefabs should not be modified in inspector during runtime.
    */

    public static CardManager instance;
    public GameObject cardPrefab;
    //Temporary hardcoded cards for testing
    public List<GameObject> testDeckPrefabs = new List<GameObject>();
    public BaseCard selectedCard;
    [HideInInspector] public List<BaseCard> currentDeck = new List<BaseCard>();//MinMaxSize: 6, MaxMaxSize: 9
    [HideInInspector] public List<GameObject> currentHand = new List<GameObject>();//Always Size: 3
    [SerializeField] private int maxHandSize = 3;
    public int maxDeckSize;
    private int deckIndex = 0; // pointer into currentDeck for drawing
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

        ShuffleDeck();
        DrawHand();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*
    void Start()
    {
        // If deck is empty and autofill is enabled, populate from Resources
        if (currentDeck == null || currentDeck.Count == 0)
        {
            FillDeckFromResources();
        }

        // Optionally shuffle here if you want a random draw order
        // ShuffleDeck();
        DrawHand();
    }*/


    public void SelectCard(BaseCard card)
    {
        if (selectedCard != null)
        {
            DeselectCard();
        }
        selectedCard = card;
        selectedCard.transform.position += new Vector3(0, 0.85f, 0);
        Debug.Log("Card Selected: " + card.cardName);
    }

    public void DeselectCard()
    {
        Debug.Log("Card Deselected: " + selectedCard.cardName);
        selectedCard.transform.position -= new Vector3(0, 0.85f, 0);
        selectedCard = null;
    }

    public void PlaySelectedCard()
    {
        if (selectedCard != null)
        {
            selectedCard.PlayCard();
            //Destroy(selectedCard.gameObject);
            //currentHand.Remove(selectedCard.gameObject);
            DeselectCard();
        }
        else
        {
            Debug.Log("No card selected to play.");
        }
    }

    public void SetUnitValue(BaseUnit unit, BaseCard card)
    {
        // Implementation for setting unit values based on the played card
    }

    void ShowInfo(BaseCard card)
    {
        // Implementation for displaying card information
    }
    
    void ShuffleDeck() //May have no use
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

    public void DrawHand()
    {
        // Remove previously instantiated hand cards from scene
        if (selectedCard != null) 
        {
            DeselectCard();
        }
        for (int i = 0; i < currentHand.Count; i++)
        {
            if (currentHand[i] != null)
            {
                Destroy(currentHand[i]);
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
            int idx = deckIndex % currentDeck.Count;
            GameObject prefabGO = currentDeck[idx].gameObject;
            Vector3 spawnPos = this.transform.position + new Vector3(i * 3, 0, 0);
            GameObject newCard = Instantiate(prefabGO, spawnPos, Quaternion.identity);
            currentHand.Add(newCard);
            deckIndex = (deckIndex + 1) % Mathf.Max(1, currentDeck.Count);
        }

        Debug.Log("Hand Drawn with " + currentHand.Count + " cards. (deckIndex=" + deckIndex + ")");
    }

    // Populate `currentDeck` from Player Deck Data
    //Some AI was used to help implement this function
    public void FillDeckFromResources()
    {
        currentDeck.Clear();

        //Temporarily hardcoded cards for testing
        for (int i = 0; i < testDeckPrefabs.Count; i++)
        {
            GameObject cardGO = Instantiate(testDeckPrefabs[i], new Vector3(-6,-6), Quaternion.identity);
            BaseCard baseCard = cardGO.GetComponent<BaseCard>();
            if (baseCard != null)
            {
                currentDeck.Add(baseCard);
                Debug.Log("Added card to deck: " + baseCard.cardName);
            }
            else
            {
                Debug.LogWarning("cardPrefab does not have a BaseCard component.");
                Destroy(cardGO);
            }
        }
    }

    // Call this draw a fresh hand
    public void NextTurn()
    {
        ShuffleDeck();
        DrawHand();
    }
}
