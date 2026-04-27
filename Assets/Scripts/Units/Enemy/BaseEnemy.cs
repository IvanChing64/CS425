using UnityEngine;

public class BaseEnemy : BaseUnit
{

    public override void ResetValues()
    {
        base.ResetValues();
    }
    public override void SelectUnit()
    {

    }
    public virtual void InitializeUnit()
    {
        ApplyFlags(); // if you use flags

        baseMoveRange = moveRange;
        baseAttackRange = attackRange;

        ApplyConquestStats(); // ensures correct state on spawn
    }
}
