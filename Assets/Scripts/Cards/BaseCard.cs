using UnityEngine;

//Developer: Bailey Escritor
//Aggregated from multiple tutorials
public abstract class BaseCard : MonoBehaviour
{
    //public BaseCard instance;
    [SerializeField] private GameObject highlightEffect;
    public string cardName;
    public Type cardType;
    public int value;
    public bool isPlayed;
    //public float drawChance;

    //Play the card's effect, overridden in derived classes
    public virtual void PlayCard()
    {
        Debug.Log("Base Card Played");
    }

    //Copies properties from ScriptableCard
    public virtual void CopyScriptableCard(ScriptableCard card)
    {
        cardName = card.cardName;
        cardType = card.type;
        value = card.value;
        Debug.Log("Card Copied: " + cardName);
    }

    //Highlight card on mouse hover during player's turn
    void OnMouseEnter()
    {
        if (GameManager.Instance.gameState != GameState.PlayerTurn) return;
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(true);
            Debug.Log("Mouse Entered Card Area" + highlightEffect.activeSelf);
        }
    }

    //Remove highlight when mouse exits card area during player's turn
    void OnMouseExit()
    {
        if (GameManager.Instance.gameState != GameState.PlayerTurn) return;
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(false);
            Debug.Log("Mouse Entered Card Area" + highlightEffect.activeSelf);
        }
    }

    //Selects or plays the card on mouse click during player's turn
    private void OnMouseDown()
    {
        if (GameManager.Instance.gameState != GameState.PlayerTurn) return;
        if (CardManager.instance == null)
        {
            Debug.LogWarning("CardManager.instance is null.");
            return;
        }

        if (CardManager.instance.selectedCard != this)
        {
            CardManager.instance.SelectCard(this);
        }
        else if (CardManager.instance.selectedCard == this)
        {
            CardManager.instance.PlaySelectedCard();
        }
    }
}
