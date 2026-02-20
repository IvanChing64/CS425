using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Developer: Bailey Escritor
//Manages card selection and playing
public class CardManager : MonoBehaviour
{
    public static CardManager instance;
    public BasePlayer selectedPlayer;
    public BaseCard selectedCard;
    public GameObject cardArea;
    [SerializeField] public static int cardSelectOffsetY = 35;

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
        Debug.Log("Card Selected: " + card.cardName);
    }

    //Deselects the currently selected card and lowers its position
    public void DeselectCard()
    {
        if (selectedCard == null) return;
        Debug.Log("Card Deselected: " + selectedCard.cardName);
        selectedCard.cardHolder.transform.localPosition -= new Vector3(0, cardSelectOffsetY, 0);
        selectedCard = null;
    }

    //Plays the selected card and removes it from the player's hand
    public void PlaySelectedCard(BaseCard card)
    {
        if (selectedCard != null)
        {
            if (selectedCard != card)
            {
                DeselectCard();
                SelectCard(card);
                return;
            }
            HandManager selectedHand = selectedPlayer.GetComponent<HandManager>();
            selectedCard.PlayCard();
            //selectedPlayer.GetComponent<HandManager>().handCardIDs.Remove(selectedHand.handCardIDs[selectedHand.currentHand.IndexOf(selectedCard)]);
            selectedPlayer.GetComponent<HandManager>().currentHand.Remove(selectedCard);
            Destroy(selectedCard.gameObject);
            DeselectCard();
            selectedPlayer.GetComponent<HandManager>().UpdateHandPositions();
        }
        else
        {
            SelectCard(card);
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

        //ToggleCardArea(true);
    }
    

    //Shows hand of selected player and hides previous player's hand
    public void SetSelectedPlayer(BasePlayer player)
    {
        BasePlayer previousPlayer = selectedPlayer;
        selectedPlayer = player;

        if (selectedPlayer == null)
        {
            Debug.Log("No player selected.");
            return;
        }

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
            }
        }
        else {
            cardArea.SetActive(false);
            if (selectedPlayer != null)
            {
                selectedPlayer.GetComponent<HandManager>().ToggleHandVisibility(false);
            }
        }
    }
}
