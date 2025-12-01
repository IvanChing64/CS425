using UnityEngine;

public class BaseMovementCard : BaseCard
{
    public int movement;

    public override void PlayCard()
    {
        Debug.Log("Movement Card Played with movement value: " + movement);
    }
}
