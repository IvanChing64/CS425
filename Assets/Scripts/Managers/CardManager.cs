using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Unity.VisualScripting;
using TMPro;

//Developer: Bailey Escritor
//Manages card selection and playing
public class CardManager : MonoBehaviour
{
    public static CardManager instance;
    public BasePlayer selectedPlayer;
    public BaseCard selectedCard;
    public GameObject cardArea, deckCard, actionPointCounter;
    public Vector3 cardLocation;
    [SerializeField] public static int cardSelectOffsetY = 18;

    // Initializes instance and calls backdrop creation
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

    //Selects a card and raises its position
    public void SelectCard(BaseCard card)
    {
        if (selectedCard != null)
        {
            DeselectCard();
        }
        selectedCard = card;

        selectedCard.cardHolder.transform.localPosition += new Vector3(0, cardSelectOffsetY, 0);
        selectedPlayer.GetComponent<HandManager>().selectedCard = selectedCard;

        //StartCoroutine(WaitForCardAnimations(selected: true));
        Debug.Log("Card Selected: " + card.cardName);
    }

    //Deselects the currently selected card and lowers its position
    public void DeselectCard()
    {
        if (selectedCard == null) return;
        Debug.Log("Card Deselected: " + selectedCard.cardName);
        //StartCoroutine(WaitForCardAnimations(selected: false));

        selectedCard.cardHolder.transform.localPosition -= new Vector3(0, cardSelectOffsetY, 0);

        selectedCard.DeselectCard();

        selectedCard = null;
    }

    //Plays the selected card and removes it from the player's hand
    public void PlaySelectedCard()
    {
        if (selectedCard != null)
        {
            selectedCard.PlayCard();
            selectedPlayer.GetComponent<HandManager>().RemoveCard(selectedCard);
            Destroy(selectedCard.gameObject);
            DeselectCard();
            selectedPlayer.GetComponent<HandManager>().UpdateHandPositions();
            UpdateDeckCard();
            UpdateAPCounter();
        }
    }

    //Hide unselected players' hands and show selected player's hand at start of turn
    public void NextTurn()
    {
        ToggleCardArea(true);
        HandManager[] handManagers = FindObjectsByType<HandManager>(FindObjectsSortMode.None);
        foreach (HandManager handManager in handManagers)
        {
            handManager.NextTurn();
            if (handManager.GetComponentInParent<BasePlayer>() == selectedPlayer)
            {
                handManager.ToggleHandVisibility(true);
            }
            else
            {
                handManager.ToggleHandVisibility(false);
            }
        }

        UpdateDeckCard();
        UpdateAPCounter();
    }

    //Draws a card from the selected player's deck into their hand
    public void DrawDeckCard()
    {
        if (selectedPlayer == null) return;
        if (GameManager.Instance.gameState != GameState.PlayerTurn) return;
        selectedPlayer.GetComponent<HandManager>().DrawCard(true);
        selectedPlayer.GetComponent<HandManager>().UpdateHandPositions();
        UpdateDeckCard();
        UpdateAPCounter();
    }

    //Shows hand of selected player and hides previous player's hand
    public void SetSelectedPlayer(BasePlayer player)
    {
        DeselectCard();
        BasePlayer previousPlayer = selectedPlayer;
        selectedPlayer = player;
        HandManager selectedHand;

        if (selectedPlayer == null)
        {
            Debug.Log("No player selected.");

            ToggleDeckCard(false);
            if (previousPlayer != null)
                previousPlayer.GetComponent<HandManager>().ToggleHandVisibility(false);

            UpdateAPCounter();
            return;
        }

        selectedHand = selectedPlayer.GetComponent<HandManager>();

        ToggleDeckCard(true);
        if (selectedHand.actionPoints > 0 && selectedHand.deckIndex != -1 && selectedHand.deckIndex < selectedHand.currentDeck.Count)
        {
            if (selectedHand.currentHand.Count < 5 && selectedPlayer.stunned == 0)
            {
                selectedPlayer.GetComponent<HandManager>().canDraw = true;
            } else
            {
                selectedPlayer.GetComponent<HandManager>().canDraw = false;
            }
        } else
        {
            selectedPlayer.GetComponent<HandManager>().canDraw = false;
        }

        if (!selectedHand.handDrawn)
        {
            selectedPlayer.GetComponent<HandManager>().DrawHand();
        }

        UpdateDeckCard();
        UpdateAPCounter();

        if (selectedHand.currentDeck.Count == 0) 
        {
            selectedPlayer.GetComponent<HandManager>().FillDeckFromResources();
            selectedPlayer.GetComponent<HandManager>().NextTurn();
        }

        //selectedPlayer.GetComponent<HandManager>().ToggleHandVisibility(true);
        if (previousPlayer != null && previousPlayer != selectedPlayer)
        {
            previousPlayer.GetComponent<HandManager>().ToggleHandVisibility(false);
        }

        if (selectedPlayer != previousPlayer)
        {
            selectedPlayer.GetComponent<HandManager>().ToggleHandVisibility(true);

        }
    }

    //Toggles card area visibility
    public void ToggleCardArea(bool show)
    {
        if (show) { 
            cardArea.SetActive(true);
            if (selectedPlayer != null)
            {
                selectedPlayer.GetComponent<HandManager>().ToggleHandVisibility(true);
                ToggleDeckCard(true);
            }
        }
        else {
            cardArea.SetActive(false);
            if (selectedPlayer != null)
            {
                selectedPlayer.GetComponent<HandManager>().ToggleHandVisibility(false);
                ToggleDeckCard(false);
            }
        }
    }

    //Toggles deck card visibility
    public void ToggleDeckCard(bool show)
    {
        if (show) { 
            deckCard.SetActive(true);
        }
        else {
            deckCard.SetActive(false);
        }
    }

    //Sets number of cards left in deck on deck card
    public void UpdateDeckCard()
    {
        if (selectedPlayer != null)
        {
            HandManager selected = selectedPlayer.GetComponent<HandManager>();

            if (selected.actionPoints == 0)
            {
                selectedPlayer.GetComponent<HandManager>().canDraw = false;
            }

            if (selected.canDraw == false)
            {
                deckCard.GetComponentInChildren<Button>().interactable = false;
                deckCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
            } else if (!deckCard.GetComponentInChildren<Button>().interactable)
            {
                deckCard.GetComponentInChildren<Button>().interactable = true;
                deckCard.GetComponentInChildren<TextMeshProUGUI>().color = new Color(140f/255f, 12f/255f, 252f/255f, 1);
            }
            if (selected.deckIndex == -1)
            {
                deckCard.GetComponentInChildren<TextMeshProUGUI>().SetText("0");
            } else
            {
                deckCard.GetComponentInChildren<TextMeshProUGUI>().SetText((selected.currentDeck.Count - selected.deckIndex).ToString());
            }
        }
    }

    //Sets Action Point Counter

    private void UpdateAPCounter()
    {
        if (selectedPlayer != null)
        {
            HandManager selected = selectedPlayer.GetComponent<HandManager>();
            int ap = selected.actionPoints;
            if (ap > 5)
            {
                actionPointCounter.GetComponentInChildren<TextMeshProUGUI>().color = Color.softYellow;
            }else if (ap > 0)
            {
                actionPointCounter.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            } else
            {
                actionPointCounter.GetComponentInChildren<TextMeshProUGUI>().color = Color.darkGray;
            }

            actionPointCounter.GetComponentInChildren<TextMeshProUGUI>().SetText(ap.ToString());
        } else
        {
            actionPointCounter.GetComponentInChildren<TextMeshProUGUI>().SetText("");
        }

    }
}
