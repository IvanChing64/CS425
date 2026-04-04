using UnityEngine;

public class BaseEnemy : BaseUnit
{
    //New stuff: Enemy Flags - Andrew Shelton
    public EnemyFlag enemyFlag;

    public override void ApplyFlags()
    {
        switch (enemyFlag)
        {
            case EnemyFlag.Ranged:
                attackRange += 3;
                maxHealth -= 95;
                break;

            case EnemyFlag.Support:
                attackBoost += 0.5f;
                defenseBoost += 0.5f;
                break;
               
        }
    }
    public override void SelectUnit()
    {

    }
}
