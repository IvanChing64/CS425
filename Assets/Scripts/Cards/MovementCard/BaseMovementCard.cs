using UnityEngine;

//Developer: Bailey Escritor
//Aggregated from multiple tutorials
public class BaseMovementCard : BaseCard
{
    //public int movement;

    public override void PlayCard()
    {
        Debug.Log("Movement Card Played with movement value: " + value);
    }
}
