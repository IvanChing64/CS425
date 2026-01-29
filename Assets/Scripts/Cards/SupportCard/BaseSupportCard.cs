using UnityEngine;

//Developer: Bailey Escritor
//Aggregated from multiple tutorials
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
                player.takeDamage(-value); // Negative damage to heal
                Debug.Log("Healed for " + value + ". Current Health: " + player.health);
                break;

            case SupportEffect.Guard:
                /* Guard is currently not implemented
                player.guard += value;
                if (player.guard > player.maxHealth)
                {
                    player.guard = player.maxHealth; // Cap guard at maxHealth
                }
                Debug.Log("Guard increased by " + value + ". Current Guard: " + player.guard);
                */
                break;

            default:
                Debug.LogWarning("Unknown support effect: " + effect);
                break;
        }
    }
}
