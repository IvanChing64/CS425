using UnityEngine;

//Developer: Bailey Escritor
//Derived class for summon cards
public class BaseSummonCard : BaseCard
{
    //public int value;
    public string summon;

    //Copies properties from ScriptableCard including support effect
    public override void CopyScriptableCard(ScriptableCard card)
    {
        base.CopyScriptableCard(card);
    }

    //Applies control effect when the card is played
    public override void PlayCard()
    {
        //ApplyControlEffect();
        isPlayed = true;
        Debug.Log("Summon Card Played with " + summon + " summoned with lifetime of " + value);
    }
}
