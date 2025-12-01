using UnityEngine;

public abstract class BaseCard : MonoBehaviour
{
    [SerializeField] private GameObject highlightEffect;
    public bool isPlayed;

    public virtual void PlayCard()
    {
        Debug.Log("Base Card Played");
    }

    void OnMouseEnter()
    {
        highlightEffect.SetActive(true);
        Debug.Log("Mouse Entered Card Area" + highlightEffect.activeSelf);
    }

    void OnMouseExit()
    {
        highlightEffect.SetActive(false);
        Debug.Log("Mouse Entered Card Area" + highlightEffect.activeSelf);    }

    private void OnMouseDown()
    {
        if (!isPlayed)
        {
            PlayCard();
            isPlayed = true;
        }
    }


}
