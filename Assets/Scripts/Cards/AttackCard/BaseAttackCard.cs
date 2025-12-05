using Unity.VisualScripting;
using UnityEngine;

//Developer: Bailey Escritor
//Aggregated from multiple tutorials
public class BaseAttackCard : BaseCard
{
    public int attack;

    public override void PlayCard()
    {
        Debug.Log("Attack Card Played with attack value: " + attack);
    }
}
