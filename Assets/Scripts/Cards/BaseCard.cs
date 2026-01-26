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

    public virtual void PlayCard()
    {
        Debug.Log("Base Card Played");
    }

    public virtual void CopyScriptableCard(ScriptableCard card)
    {
        cardName = card.cardName;
        cardType = card.type;
        value = card.value;
        Debug.Log("Card Copied: " + cardName);
    }

    void OnMouseEnter()
    {
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(true);
            Debug.Log("Mouse Entered Card Area" + highlightEffect.activeSelf);
        }
    }

    void OnMouseExit()
    {
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(false);
            Debug.Log("Mouse Entered Card Area" + highlightEffect.activeSelf);
        }
    }

    private void OnMouseDown()
    {
        if (CardManager.instance == null)
        {
            Debug.LogWarning("CardManager.instance is null. Make sure a CardManager exists in the scene.");
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
