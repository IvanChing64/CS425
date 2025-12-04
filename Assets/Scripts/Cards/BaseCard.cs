using UnityEngine;

public abstract class BaseCard : MonoBehaviour
{
    //public BaseCard instance;
    [SerializeField] private GameObject highlightEffect;
    public string cardName;
    public Type cardType;
    public bool isPlayed;
    public float drawChance;

    public virtual void PlayCard()
    {
        Debug.Log("Base Card Played");
    }

    public virtual void LoadFromScriptable(ScriptableCard scriptable)
{
    // Example: Copy fields from ScriptableCard to this BaseCard
    /*
    this.cardName = scriptable.cardName;
    this.cardType = scriptable.type;
    this.isPlayed = scriptable.isPlayed;
    this.drawChance = scriptable.drawChance;
    /*/
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
        else
        {
            CardManager.instance.DeselectCard();
        }
    }
}
