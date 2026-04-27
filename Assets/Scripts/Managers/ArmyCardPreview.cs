using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyCardPanel : MonoBehaviour
{
    [SerializeField] private RectTransform cardPanel;
    [SerializeField] private Coroutine moveRoutine;
    [SerializeField] private List<BaseCard> previewCards;
    [SerializeField] private int selectedSlot; 
    public static ArmyCardPanel instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        //cardPanel.gameObject.SetActive(false);
        //ShowCardPanel(false);
    }
    
    // Move card panel on-screen or off-screen
    public void ShowCardPanel(bool show)
    {
        cardPanel.gameObject.SetActive(true);

        Vector3 showPosit = new Vector3(0f, 0f, 0f);
        Vector3 hidePosit = new Vector3(0f, -1000f, 0f);
        Vector3 posit;

        if (show)
        {
            posit = showPosit;
            if (moveRoutine != null)
            {
                StopCoroutine(MoveCardPanel(hidePosit));
            }
        } else
        {
            posit = hidePosit;
            if (moveRoutine != null)
            {
                StopCoroutine(MoveCardPanel(showPosit));
            }
        }

        moveRoutine = StartCoroutine(MoveCardPanel(posit));
    }

    // Move card panel to designated position
    private IEnumerator MoveCardPanel(Vector3 position)
    {
        Vector3 startPos = cardPanel.anchoredPosition;
        float duration = 0.1f;
        float time = 0;
        while(time < duration)
        {
            cardPanel.anchoredPosition = Vector2.Lerp(startPos, position, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        cardPanel.anchoredPosition = position;
        moveRoutine = null;
    }

    // Show the selected unit's cards
    public void UpdateCardPanel(int index)
    {
        BasePlayer unit = (BasePlayer)ArmyManager.Instance.unitsInArmy[index].UnitPrefab;
        List<ScriptableCard> cards = new List<ScriptableCard>();

        foreach (string cardName in unit.startingDeck)
        {
            cards.Add(DeckManager.instance.GetCardByName(cardName));
        }

        foreach (ScriptableCard card in cards)
        {
            GameObject newCard = null;

            //Instantiate card based on its type
            switch (card.type)
            {
                case Type.Support:
                    newCard = Instantiate(DeckManager.supportCardPrefab, Vector3.zero, Quaternion.identity);
                    break;
                case Type.Attack:
                    newCard = Instantiate(DeckManager.attackCardPrefab, Vector3.zero, Quaternion.identity);
                    break;
                case Type.Movement:
                    newCard = Instantiate(DeckManager.movementCardPrefab, Vector3.zero, Quaternion.identity);
                    break;

                case Type.Summon:
                    newCard = Instantiate(DeckManager.summonCardPrefab, Vector3.zero, Quaternion.identity);
                    break;

                default:
                    Debug.LogWarning("Unknown card type: " + card.type);
                    return;
            }   

            previewCards.Add(newCard.GetComponent<BaseCard>());
            newCard.GetComponent<CardDisplay>().cardData = card;
            moveCards();
            ShowCardPanel(true);
        }   
    }

    // Clear panel
    public void clearCards()
    {
        for (int i = 0; i < previewCards.Count; i++)
        {
            if (previewCards[i] != null)
            {
                Destroy(previewCards[i].gameObject);
            }
        }
        previewCards.Clear();

        ShowCardPanel(false);
    }

    // Move cards to position based on number of cards
    public void moveCards()
    {
        int index1 = 0;
        int index2 = 0;
        foreach (BaseCard card in previewCards)
        {
            if (index1 < previewCards.Count / 2)
            {
                if ((index1 + 1) % 2 == 1)
                {
                    card.cardHolder.transform.position = new Vector3(((index1 - (index1 / 2)) * HandManager.cardPositionOffsetX * 3) + 500, 670, 0);
                } else
                {
                    card.cardHolder.transform.position = new Vector3(((index1 - 1 - (index1 / 2 - 0.5f)) * HandManager.cardPositionOffsetX * 3) + 500, 670, 0);
                }
                index1++;
            } else
            {
                if ((index2 + 1) % 2 == 1)
                {
                    card.cardHolder.transform.position = new Vector3(((index2 - (index2 / 2)) * HandManager.cardPositionOffsetX * 3) + 500, 390, 0);
                } else
                {
                    card.cardHolder.transform.position = new Vector3(((index2 - 1 - (index2 / 2 - 0.5f)) * HandManager.cardPositionOffsetX * 3) + 500, 390, 0);
                }
                index2++;
            }
        }
    }

}
