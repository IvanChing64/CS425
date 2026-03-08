using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : BaseUnit
{
    public List<string> startingDeck = new List<string>();
    public bool canAttack = false, canSupport = false;
    public int energy;

    //Resets a units card values at the end of their turn
    public void ResetCardValues()
    {
        moveRange = 0;
        attackRange = 0;
        dmg = 0;
        canAttack = false;
    }
}
