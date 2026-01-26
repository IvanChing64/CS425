using System.Collections.Generic;
using UnityEngine;

//Developer: Bailey Escritor
//Manages card selection and playing
public class CardManager : MonoBehaviour
{
    public static CardManager instance;
    public GameObject cardAreaBackdrop;
    [SerializeField]private int maxHandSize = 3;
    public BasePlayer selectedPlayer;
    public BaseCard selectedCard;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

        //Create Backdrop for Card Area
        for (int i = 0; i < maxHandSize; i++)
        {
            Vector3 backdropPos = transform.position + new Vector3(i * 3, 0, 0);
            Instantiate(cardAreaBackdrop, backdropPos, Quaternion.identity);
        }
    }

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
        if (selectedCard == null) return;
        Debug.Log("Card Deselected: " + selectedCard.cardName);
        selectedCard.transform.position -= new Vector3(0, 0.85f, 0);
        selectedCard = null;
    }

    public void PlaySelectedCard()
    {
        if (selectedCard != null)
        {
            selectedCard.PlayCard();
            selectedPlayer.GetComponent<HandManager>().currentHand.Remove(selectedCard.gameObject);
            Destroy(selectedCard.gameObject);
            DeselectCard();
        }
        else
        {
            Debug.Log("No card selected to play.");
        }
    }

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
    }

    public void SetSelectedPlayer(BasePlayer player)
    {
        BasePlayer previousPlayer = selectedPlayer;
        selectedPlayer = player;
        if (selectedPlayer.GetComponent<HandManager>().currentDeck.Count == 0) 
        {
            selectedPlayer.GetComponent<HandManager>().FillDeckFromResources();
            selectedPlayer.GetComponent<HandManager>().NextTurn();
        }

        selectedPlayer.GetComponent<HandManager>().ToggleHandVisibility(true);
        if (previousPlayer != null && previousPlayer != selectedPlayer)
        {
            previousPlayer.GetComponent<HandManager>().ToggleHandVisibility(false);
        }
    }
}
