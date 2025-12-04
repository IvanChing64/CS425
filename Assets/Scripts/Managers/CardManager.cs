using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    /*
    Make background elements for card area, better than disabling card prefabs
    Move Cards to bottom of screen, change camera output
    Implement Separate Deck Manager to better handle individual player decks
    */

    public static CardManager instance;
    public GameObject cardPrefab;
    //Temporary hardcoded cards for testing
    public List<GameObject> testDeckPrefabs = new List<GameObject>();
    public BaseCard selectedCard;
    public List<BaseCard> currentDeck = new List<BaseCard>();//MinMaxSize: 6, MaxMaxSize: 9
    public List<GameObject> currentHand = new List<GameObject>();//Always Size: 3
    [SerializeField] private int maxHandSize = 3;
    public int maxDeckSize;
    private int deckIndex = 0; // pointer into currentDeck for drawing
    //[Header("Auto-Fill Deck")]
    //[SerializeField] private bool autoFillFromResources = true;
    //[SerializeField] private string prefabsPath = "CardPrefabs"; // folder under Assets/Prefabs/<prefabsPath>
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
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
    }


    public void SelectCard(BaseCard card)
    {
        if (selectedCard != null)
        {
            DeselectCard();
        }
        selectedCard = card;
        selectedCard.transform.position += new Vector3(0, 1.1f, 0);
        Debug.Log("Card Selected: " + card.cardName);
    }

    public void DeselectCard()
    {
        Debug.Log("Card Deselected: " + selectedCard.cardName);
        selectedCard.transform.position -= new Vector3(0, 1.1f, 0);
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
    //[ContextMenu("Fill Deck From Resources")]
    public void FillDeckFromResources()
    {
        currentDeck.Clear();
        /*
        GameObject[] prefabs = Resources.LoadAll<GameObject>();
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogWarning("No prefabs found in Resources/" + prefabsPath + ". Make sure prefabs are placed there.");
            return;
        }

        foreach (var go in prefabs)
        {
            if (go == null) continue;
            BaseCard cardComp = go.GetComponent<BaseCard>();
            if (cardComp != null)
            {
                currentDeck.Add(cardComp);
            }
            else
            {
                Debug.LogWarning("Prefab '" + go.name + "' does not contain a BaseCard component and will be skipped.");
            }
        }

        maxDeckSize = currentDeck.Count;
        deckIndex = 0;
        Debug.Log("Filled deck with " + currentDeck.Count + " cards from Resources/" + prefabsPath);
        */

        //Temporary hardcoded cards for testing
        /*ScriptableCard[] allCards = Resources.LoadAll<ScriptableCard>("Cards");
        foreach (var card in allCards)
        {
            GameObject cardGO = Instantiate(cardPrefab, new Vector3(-5,-5), Quaternion.identity);
            BaseCard baseCard = cardGO.GetComponent<BaseCard>();
            if (baseCard != null)
            {
                baseCard.LoadFromScriptable(card); // Method in BaseCard
                currentDeck.Add(baseCard);
                Debug.Log("Added card to deck: " + baseCard.cardName);
            }
            else
            {
                Debug.LogWarning("cardPrefab does not have a BaseCard component.");
                Destroy(cardGO);
            }
        }*/

        for (int i = 0; i < testDeckPrefabs.Count; i++)
        {
            GameObject cardGO = Instantiate(testDeckPrefabs[i], new Vector3(-5,-5), Quaternion.identity);
            BaseCard baseCard = cardGO.GetComponent<BaseCard>();
            if (baseCard != null)
            {
                //baseCard.LoadFromScriptable(card); // Method in BaseCard
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

    // Call this to advance to the next player turn and draw a fresh hand
    public void NextTurn()
    {
        // Any per-turn cleanup/logic can go here
        DrawHand();
    }
}
