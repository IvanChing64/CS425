using UnityEngine;

public class BaseSupportCard : BaseCard
{
    public int health;

    public override void PlayCard()
    {
        Debug.Log("Support Card Played with health value: " + health);
    }
}
