using System.Collections.Generic;
using UnityEngine;

//Developer: Bailey Escritor
//Holds all available cards and manages deck creation
public class DeckManager : MonoBehaviour
{
    public static DeckManager instance {get; private set;}
    public List<ScriptableCard> allCards = new List<ScriptableCard>();
    private bool initialized = false;

    //Initializes singleton instance and loads all cards from Resources
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            initialized = true;
            ScriptableCard[] cards = Resources.LoadAll<ScriptableCard>("Cards");
            allCards.AddRange(cards);

            Debug.Log("DeckManager Instance Created & Initialized");
        } else
        {
            Debug.Log("DeckManager Instance already exists, destroying duplicate.");
            Destroy(gameObject);
        }
    }

    //Enforces single initialization
    void Start()
    {
        if (!initialized)
        {
            initialized = true;
            ScriptableCard[] cards = Resources.LoadAll<ScriptableCard>("Cards");
            allCards.AddRange(cards);

            Debug.Log("DeckManager Initialized");
        } else
        {
            Debug.Log("DeckManager already initialized");
        }
    }

    //Returns a unit's deck based on their startingDeck list
    public void GetDeck(HandManager handManager)
    {
        BasePlayer unit = handManager.GetComponent<BasePlayer>();
        ScriptableCard addedCard;

        Debug.Log("Filling deck for unit with " + unit.startingDeck.Count + " cards.");

        foreach (string newCardName in unit.startingDeck)
        {
            Debug.Log("Adding " + newCardName + " to deck.");
            addedCard = GetCardByName(newCardName);
            if (addedCard != null)
            {
                handManager.currentDeck.Add(addedCard);
                Debug.Log(newCardName + " added to deck.");
            } else
            {
                Debug.LogWarning(newCardName + " not found in allCards.");
            }
        }
    }

    //Gets card by name from allCards list
    public ScriptableCard GetCardByName(string newCardName)
    {
        return allCards.Find(card => card.cardName == newCardName);
    }
}
