using UnityEngine;

//Developer: Bailey Escritor
//Aggregated from multiple tutorials
public class BaseSupportCard : BaseCard
{
    public SupportEffect effect;
    //public int value;

    public override void PlayCard()
    {
        Debug.Log("Support Card Played with " + effect + " of value: " + value);
    }
}
