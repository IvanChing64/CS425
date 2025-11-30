using UnityEngine;

public class BaseAttackCard : BaseCard
{
    public int attack;

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
        Debug.Log("Attack Card Played with attack value: " + attack);
    }
}
