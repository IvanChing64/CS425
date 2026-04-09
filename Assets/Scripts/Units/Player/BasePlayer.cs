using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : BaseUnit
{
    [Tooltip("Card Set for Combat")]
    public List<string> startingDeck = new List<string>();
    public bool canAttack = false, canSupport = false;
    [Tooltip("Used for Drawing or Playing")]
    public int energy;

    //Resets a units card values at the end of their turn
    public override void ResetValues()
    {
        moveRange = 0;
        attackRange = 0;
        dmg = 0;
        canAttack = false;
        canSupport = false;
        
        base.ResetValues();

        GetComponentInParent<HandManager>().UpdateCardVisuals();
    }

    public override void SelectUnit()
    {

    }
}
