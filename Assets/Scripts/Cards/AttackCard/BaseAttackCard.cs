using Unity.VisualScripting;
using UnityEngine;

public class BaseAttackCard : BaseCard
{
    public int attack;

    public override void PlayCard()
    {
        Debug.Log("Attack Card Played with attack value: " + attack);
    }
}
