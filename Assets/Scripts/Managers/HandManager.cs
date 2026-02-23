using System.Collections.Generic;
using Unity.VisualScripting;

//using System.Numerics;
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

    [SerializeField] private GameObject attackCardPrefab, movementCardPrefab, supportCardPrefab, controlCardPrefab;
    public List<ScriptableCard> currentDeck = new List<ScriptableCard>();//MinMaxSize: 6, MaxMaxSize: 9
    public List<BaseCard> currentHand = new List<BaseCard>();//Always Size: 3
    // [SerializeField] public List<int> deckCardIDs = new List<int>();
    // [SerializeField] public List<int> handCardIDs = new List<int>();
    public int drawNum = 3; //At least 3
    [SerializeField] private int deckIndex = 0; // pointer into currentDeck for drawing
    public bool handDrawn = false;
    public bool handSelected = false;
    //public int currentHandSize;

    //Initalizes instance and fills deck
    void Awake()
    {
        //if (instance == null)
        //{
        //    instance = this;
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
    
        if (currentDeck == null || currentDeck.Count == 0)
        {
            FillDeckFromResources();
        }

        ShuffleDeck();
        //DrawHand();
    }

    //Shows card info in UI
    void ShowInfo(BaseCard card)
    {
        // Implementation for displaying card information
    }
    
    //Shuffle cards based on Fisher-Yates Shuffle
    void ShuffleDeck()
    {
        for (int i = currentDeck.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            ScriptableCard temp = currentDeck[i];
            currentDeck[i] = currentDeck[randomIndex];
            currentDeck[randomIndex] = temp;

            // int tempID = deckCardIDs[i];
            // deckCardIDs[i] = deckCardIDs[randomIndex];
            // deckCardIDs[randomIndex] = tempID;
            
        }
        Debug.Log("Deck Shuffled");
    }

    //Draw cards up to maxHandSize from currentDeck
    public void DrawHand()
    {
        //If hand already drawn or hand is full, do not draw again
        if (handDrawn)
        {
            Debug.Log("Hand already drawn for this turn.");
            return;
        }

        //Deselect any selected card
        if (CardManager.instance.selectedCard != null) 
        {
            CardManager.instance.DeselectCard();
        }

        //Clear current hand (Subject to change based on game design)
        
        for (int i = 0; i < currentHand.Count; i++)
        {
            if (currentHand[i] != null)
            {
                Destroy(currentHand[i].gameObject);
                Debug.Log("Destroyed card: " + currentHand[i].name);
            }
        }
        currentHand.Clear();
        

        //If deck is not filled, fill deck from resources
        if (currentDeck == null)
        {
            FillDeckFromResources();
            Debug.Log("Current deck is empty - Filling deck from resources.");
        }

        // Draw up to DrawNum cards from the deck starting at deckIndex
        int cardsToDraw = drawNum;

        if (currentHand.Count == currentDeck.Count)
        {
            Debug.Log("Hand is full, cannot draw more cards.");
            return;
        }

        for (int i = 0; i < cardsToDraw; i++)
        {
            Debug.Log("Drawing card " + (i + 1) + " of " + cardsToDraw);
            if (currentDeck.Count == 0) break;

            // Reset deckIndex, if it exceeds deck size, to the top of the deck and reshuffle
            if (deckIndex >= currentDeck.Count)
            {
                deckIndex = 0;
                ShuffleDeck();
            }

            ScriptableCard drawnCard = currentDeck[deckIndex];
            GameObject newCard = null;
            Vector3 spawnPos = new Vector3(currentHand.Count * 250 - 250, -500, 0);

            //Check if drawn card is already in hand, if so, skip and draw next card
            /*
            bool notInHand = false;
            while (!notInHand)
            {
                notInHand = true;
                for (int j = 0; j < currentHand.Count; j++)
                {
                    if (handCardIDs.Count > j && handCardIDs[j] == deckCardIDs[deckIndex])
                    {
                        Debug.Log("Card already in hand: " + drawnCard.cardName);
                        deckIndex++;
                        if (deckIndex >= currentDeck.Count)
                        {
                            deckIndex = 0;
                            ShuffleDeck();
                        }
                        drawnCard = currentDeck[deckIndex];
                        notInHand = false;
                        break;
                    }
                }
            }
            */

            //Instantiate card based on its type
            switch (drawnCard.type)
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

                case Type.Control:
                    newCard = Instantiate(controlCardPrefab, spawnPos, Quaternion.identity);
                    break;

                default:
                    Debug.LogWarning("Unknown card type: " + drawnCard.type);
                    continue;
            }

            //Add new card to current hand and copy properties from scriptable card
            currentHand.Add(newCard.GetComponent<BaseCard>());
            newCard.GetComponent<BaseCard>().cardHolder.transform.localPosition = spawnPos;
            //handCardIDs.Add(deckCardIDs[deckIndex]);
            newCard.GetComponent<CardDisplay>().cardData = drawnCard;
            newCard.GetComponent<BaseCard>().CopyScriptableCard(drawnCard);
            deckIndex++;
        }

        handDrawn = true;
        Debug.Log("Hand Drawn with " + currentHand.Count + " cards.");
    }

    // Populate `currentDeck` from Player Deck Data
    public void FillDeckFromResources()
    {
        currentDeck.Clear();
        DeckManager.instance.GetDeck(this);
        
        // int index = 0;
        // foreach (ScriptableCard card in currentDeck)
        // {
        //     deckCardIDs.Add(index);
        //     index++;
        // }
        
        Debug.Log("Deck filled from resources with " + currentDeck.Count + " cards.");
    }

    // Call this to reset for next turn
    public void NextTurn()
    {
        handDrawn = false;
        this.GetComponentInParent<BasePlayer>().ResetCardValues();
        UpdateHandPositions();
        //ShuffleDeck();
        DrawHand();
    }

    //Toggles hand visibility based on input
    public void ToggleHandVisibility(bool show)
    {
        if (currentHand.Count == 0)
        {
            Debug.Log("No cards in hand to show.");
            return;
        }

        if (show)
        {
            foreach (BaseCard card in currentHand)
            {
                card.GameObject().SetActive(true);
            }
            handSelected = true;
            Debug.Log("Hand is now shown.");
        } else
        {
            foreach (BaseCard card in currentHand)
            {
                card.GameObject().SetActive(false);
            }
            handSelected = false;
            Debug.Log("Hand is now hidden.");
        }
    }

    //Move cards to the right in hand
    //Move cards dynamically based on hand size and card index to keep them centered
    public void UpdateHandPositions()
    {
        // for (int i = 0; i < currentHand.Count; i++)
        // {
        //     if (currentHand[i] != null)
        //     {
        //         Vector3 targetPos = CardManager.instance.cardLocation + new Vector3(i * 3, 0, 0);
        //         currentHand[i].transform.position = targetPos;
        //     }
        // }

        if (currentHand.Count == 0)
        {
            //Debug.Log("No cards in hand to position.");
            return;
        }

        if (currentHand.Count % 2 == 0)
        {
            for (int i = 0; i < currentHand.Count; i++)
            {
                if (currentHand[i] != null)
                {
                    Vector3 targetPos = new Vector3((i - (currentHand.Count / 2 - 0.5f)) * 250, -500, 0);
                    currentHand[i].cardHolder.transform.localPosition = targetPos;
                    if (currentHand[i] == CardManager.instance.selectedCard)
                    {
                        currentHand[i].cardHolder.transform.localPosition += new Vector3(0, 85, 0);
                    }
                }
            }
            //Debug.Log("Current hand size is even, centering cards between two middle cards.");
        } else if (currentHand.Count % 2 == 1) 
        {
                for (int i = 0; i < currentHand.Count; i++)
                {
                    if (currentHand[i] != null)
                    {   
                        Vector3 targetPos = new Vector3((i - (currentHand.Count / 2)) * 250, -500, 0);
                        currentHand[i].cardHolder.transform.localPosition = targetPos;
                        if (currentHand[i] == CardManager.instance.selectedCard)
                        {
                            currentHand[i].cardHolder.transform.localPosition += new Vector3(0, 85, 0);
                        }
                    }
                }

            //Debug.Log("Current hand size is odd, centering cards.");
        }
    }
}