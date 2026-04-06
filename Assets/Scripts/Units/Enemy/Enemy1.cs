using UnityEngine;

public class Enemy1 : BaseEnemy
{
    //New stuff: Enemy Flags - Andrew Shelton
    public EnemyFlag enemyFlag;
    public MovementBehavior movementBehavior = MovementBehavior.Default;

    public override void ApplyFlags()
    {
        switch (enemyFlag)
        {
            case EnemyFlag.Ranged:
                attackRange += 3;
                maxHealth -= 20;
                health -= 20;
                movementBehavior = MovementBehavior.Ranged;
                break;

            case EnemyFlag.Support:
                //attackBoost += 0.5f;
                defenseBoost += 0.5f;
                movementBehavior = MovementBehavior.Support;
                break;

            case EnemyFlag.Default:
                // No changes for default
                break;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ApplyFlags();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
