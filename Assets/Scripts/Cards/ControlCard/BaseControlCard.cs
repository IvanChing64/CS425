using UnityEngine;

//Developer: Bailey Escritor
//Derived class for control cards
public class BaseControlCard : BaseCard
{
    public ControlEffect effect;
    //public int value;

    //Copies properties from ScriptableCard including support effect
    public override void CopyScriptableCard(ScriptableCard card)
    {
        base.CopyScriptableCard(card);
        effect = card.controlEffect;
    }

    //Applies control effect when the card is played
    public override void PlayCard()
    {
        ApplyControlEffect();
        isPlayed = true;
        Debug.Log("Control Card Played with " + effect + " of value: " + value);
    }

    //Applies the control effect to the selected enemy
    public void ApplyControlEffect()
    {
        //BasePlayer player = CardManager.instance.currentPlayer;

        switch (effect)
        {
            case ControlEffect.Stun:
                //target.Stun(value);
                break;

            case ControlEffect.Poison:
                //target.Poison(value);
                break;

            default:
                Debug.LogWarning("Unknown control effect: " + effect);
                break;
        }
    }
}
