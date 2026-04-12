using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Developer: Bailey Escritor
//Holds all available cards and manages deck creation
public class DeckManager : MonoBehaviour
{
    public static DeckManager instance {get; private set;}
    public static GameObject attackCardPrefab, movementCardPrefab, supportCardPrefab, summonCardPrefab;
    public List<ScriptableCard> allCards = new List<ScriptableCard>();
    public List<ScriptableUnit> allSummons = new List<ScriptableUnit>();
    private bool initialized = false;


    public int DEBUGTEAMSELECTOR = 0;

    // Initializes singleton instance and loads all cards from Resources
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            initialized = true;
            ScriptableCard[] cards = Resources.LoadAll<ScriptableCard>("Cards");
            allCards.AddRange(cards);
            attackCardPrefab = Resources.Load<GameObject>("Prefabs/Cards/AttackCardPrefab");
            movementCardPrefab = Resources.Load<GameObject>("Prefabs/Cards/MovementCardPrefab");
            supportCardPrefab = Resources.Load<GameObject>("Prefabs/Cards/SupportCardPrefab");
            summonCardPrefab = Resources.Load<GameObject>("Prefabs/Cards/SummonCardPrefab");
            ScriptableUnit[] summons = Resources.LoadAll<ScriptableUnit>("Summons");
            allSummons.AddRange(summons);
            DEBUGTEAMSELECTOR = 0;

            Debug.Log("DeckManager Instance Created & Initialized");
        } else
        {
            Debug.Log("DeckManager Instance already exists, destroying duplicate.");
            Destroy(gameObject);
        }
    }

    // Enforces single initialization
    void Start()
    {
        if (!initialized)
        {
            initialized = true;
            ScriptableCard[] cards = Resources.LoadAll<ScriptableCard>("Cards");
            allCards.AddRange(cards);
            attackCardPrefab = Resources.Load<GameObject>("Prefabs/Cards/AttackCardPrefab");
            movementCardPrefab = Resources.Load<GameObject>("Prefabs/Cards/MovementCardPrefab");
            supportCardPrefab = Resources.Load<GameObject>("Prefabs/Cards/SupportCardPrefab");
            summonCardPrefab = Resources.Load<GameObject>("Prefabs/Cards/SummonCardPrefab");
            ScriptableUnit[] summons = Resources.LoadAll<ScriptableUnit>("Summons");
            allSummons.AddRange(summons);
            DEBUGTEAMSELECTOR = 0;

            Debug.Log("DeckManager Initialized");
        } else
        {
            Debug.Log("DeckManager already initialized");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            instance.DEBUGTEAMSELECTOR = (instance.DEBUGTEAMSELECTOR + 1) % 6;
        }
    }

    // Returns a unit's deck based on their startingDeck list
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

    // Gets card by name from allCards list
    public ScriptableCard GetCardByName(string newCardName)
    {
        return allCards.Find(card => card.cardName == newCardName);
    }

    // Gets a summon by enum from allSummons list
    public ScriptableUnit GetSummonByName(string summonName)
    {
        return allSummons.Find(ScriptableObject => ScriptableObject.name == summonName);
    }
}
