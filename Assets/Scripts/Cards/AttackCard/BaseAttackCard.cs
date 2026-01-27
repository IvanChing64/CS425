using Unity.VisualScripting;
using UnityEngine;

//Developer: Bailey Escritor
//Aggregated from multiple tutorials
public class BaseAttackCard : BaseCard
{
    //public int attack;

    //Sets unit's dmg value and allows attack action
    public override void PlayCard()
    {
        CardManager.instance.selectedPlayer.canAttack = true;
        CardManager.instance.selectedPlayer.dmg = value;
        isPlayed = true;    
        Debug.Log("Attack Card Played with attack value: " + value);
    }
}
