using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : BaseUnit
{
    public List<string> startingDeck = new List<string>();
    public bool canAttack = false, canSupport = false;
    public int energy;

    //Resets a units card values at the end of their turn
    public void ResetValues()
    {
        moveRange = 0;
        attackRange = 0;
        dmg = 0;
        canAttack = false;
        attackBoost = 1;
        defenseBoost = 1;

        if (stunned > 0)
        {
            stunned--;
        } else
        {
            stunned = 0;
        }

        if (invisible > 0)
        {
            invisible--;
            if (invisible == 0)
            {
                Visible();
            }
            attackBoost += 0.15f;
        } else
        {
            invisible = 0;
        }


        GetComponentInParent<HandManager>().UpdateCardVisuals();
        
    }

    public override void SelectUnit()
    {

    }
}
