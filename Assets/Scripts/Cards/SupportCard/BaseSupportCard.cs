using UnityEngine;

//Developer: Bailey Escritor
//Derived class for support cards
public class BaseSupportCard : BaseCard
{
    public SupportEffect effect;
    //public int value;

    //Copies properties from ScriptableCard including support effect
    public override void CopyScriptableCard(ScriptableCard card)
    {
        base.CopyScriptableCard(card);
        effect = card.supportEffect;
    }

    //Applies support effect when the card is played
    public override void PlayCard()
    {
        ApplySupportEffect();
        isPlayed = true;
        Debug.Log("Support Card Played with " + effect + " of value: " + value);
    }

    //Applies the support effect to the selected player
    public void ApplySupportEffect()
    {
        BasePlayer player = CardManager.instance.selectedPlayer;

        switch (effect)
        {
            case SupportEffect.Heal:
                player.Heal(value);
                break;

            case SupportEffect.Guard:
                player.Guard(value);
                break;

            default:
                Debug.LogWarning("Unknown support effect: " + effect);
                break;
        }
    }
}
