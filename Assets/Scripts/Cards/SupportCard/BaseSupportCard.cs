using UnityEngine;

public class BaseSupportCard : BaseCard
{
    public int health;

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
        Debug.Log("Support Card Played with health value: " + health);
    }
}
