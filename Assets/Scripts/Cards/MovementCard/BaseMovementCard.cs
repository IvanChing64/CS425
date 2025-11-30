using UnityEngine;

public class BaseMovementCard : BaseCard
{
    public int movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void PlayCard()
    {
        Debug.Log("Movement Card Played with movement value: " + movement);
    }
}
