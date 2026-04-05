using UnityEngine;

public class Enemy1 : BaseEnemy
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
