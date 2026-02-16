using System.Collections.Generic;
using UnityEngine;

//Developer: Bailey Escritor
//Manages card selection and playing
public class CardManager : MonoBehaviour
{
    public static CardManager instance;
    public GameObject cardAreaBackdrop;
    public BasePlayer selectedPlayer;
    public BaseCard selectedCard;
    public Vector3 cardLocation;


    // Initializes instance and calls backdrop creation
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            cardLocation = transform.position;
        }
        else
        {
            Destroy(gameObject);
        }

        //Upddates card location based on grid size
        CenterCardArea();

        //Create Backdrop for Card Area
        //CreateCardAreaBackdrops(maxHandSize);
    }

    //Selects a card and raises its position
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

    //Deselects the currently selected card and lowers its position
    public void DeselectCard()
    {
        if (selectedCard == null) return;
        Debug.Log("Card Deselected: " + selectedCard.cardName);
        selectedCard.transform.position -= new Vector3(0, 0.85f, 0);
        selectedCard = null;
    }

    //Plays the selected card and removes it from the player's hand
    public void PlaySelectedCard()
    {
        if (selectedCard != null)
        {
            HandManager selectedHand = selectedPlayer.GetComponent<HandManager>();
            selectedCard.PlayCard();
            //selectedPlayer.GetComponent<HandManager>().handCardIDs.Remove(selectedHand.handCardIDs[selectedHand.currentHand.IndexOf(selectedCard)]);
            selectedPlayer.GetComponent<HandManager>().currentHand.Remove(selectedCard);
            Destroy(selectedCard.gameObject);
            DeselectCard();
        }
        else
        {
            Debug.Log("No card selected to play.");
        }
    }

    //Hide unselected players' hands and show selected player's hand at start of turn
    public void NextTurn()
    {
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

    //Creates backdrops for card area (May be irrelevant with later changes)
    private void CreateCardAreaBackdrops(int handSize)
    {
        for (int i = 0; i < handSize; i++)
        {
            Vector3 backdropPos = cardLocation + new Vector3(i * 3, 0, 0);
            Instantiate(cardAreaBackdrop, backdropPos, Quaternion.identity);
        }
    }

    //Ensures cards are in the middle of the card area
    public void CenterCardArea()
    {
        float centerX = (float)GridManager.width / 2f - 3.5f;
        float centerY = (float)GridManager.height / 2f - 5.25f - (0.25f * GridManager.height);
        //float i = 7.75f;


        cardLocation = new Vector3(centerX, centerY, 0);
    }

    //Toggles card area visibility
    public void ToggleCardArea(bool show)
    {
        cardLocation = transform.position;
        if (show)
        {
            transform.position = cardLocation + new Vector3(0, 100, 0);
            Debug.Log("Card area shown.");
        }
        else
        {
            transform.position = cardLocation + new Vector3(0, -100, 0);
            Debug.Log("Card area hidden.");
        }
    }
}
