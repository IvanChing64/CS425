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
    public GameObject cardArea, deckCard;
    public Vector3 cardLocation;
    [SerializeField] public static int cardSelectOffsetY = 20;

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
            selectedPlayer.GetComponent<HandManager>().RemoveCard(selectedCard);
            Destroy(selectedCard.gameObject);
            DeselectCard();
            selectedPlayer.GetComponent<HandManager>().UpdateHandPositions();
            UpdateDeckCard();
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
    }

    //Draws a card from the selected player's deck into their hand
    public void DrawDeckCard()
    {
        if (selectedPlayer == null) return;
        if (GameManager.Instance.gameState != GameState.PlayerTurn) return;
        selectedPlayer.GetComponent<HandManager>().DrawCard(true);
        selectedPlayer.GetComponent<HandManager>().UpdateHandPositions();
        UpdateDeckCard();
    }

    //Shows hand of selected player and hides previous player's hand
    public void SetSelectedPlayer(BasePlayer player)
    {
        DeselectCard();
        BasePlayer previousPlayer = selectedPlayer;
        selectedPlayer = player;

        if (selectedPlayer == null)
        {
            ToggleDeckCard(false);
            Debug.Log("No player selected.");
            return;
        }

        ToggleDeckCard(true);
        UpdateDeckCard();

        if (selectedPlayer.GetComponent<HandManager>().currentDeck.Count == 0) 
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

    //Sets number of action points on deck card
    public void UpdateDeckCard()
    {
        if (selectedPlayer != null)
        {
            if (selectedPlayer.GetComponent<HandManager>().canDraw == false)
            {
                deckCard.GetComponentInChildren<Button>().interactable = false;
                deckCard.GetComponentInChildren<TextMeshProUGUI>().SetText("X");
                deckCard.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
                return;
            } else if (!deckCard.GetComponentInChildren<Button>().interactable)
            {
                deckCard.GetComponentInChildren<Button>().interactable = true;
                deckCard.GetComponentInChildren<TextMeshProUGUI>().color = new Color(140f/255f, 12f/255f, 252f/255f, 1);
            }
            deckCard.GetComponentInChildren<TextMeshProUGUI>().SetText(selectedPlayer.GetComponent<HandManager>().actionPoints.ToString());
        }
    }

    private void moveCard ()
    {
        
    }
}
