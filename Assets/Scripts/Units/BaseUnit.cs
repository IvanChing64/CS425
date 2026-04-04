using Codice.Client.Common.GameUI;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;





public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;
    public Faction Faction;
    public float maxHealth;
    public float health;
    public float guard;
    public float dmg;
    public float attackBoost = 1;
    public float defenseBoost = 1;
    public int stunned = 0;
    public int invisible = 0;
    public int moveRange;
    public int attackRange;
    public Animator UnitAnimator;
    [SerializeField] healthbar healthbar;
    [SerializeField] private AudioClip[] hurtSFX;

    //Enemy Flags: Andrew Shelton
    public enum EnemyFlag
    {
        Ranged,
        None,
        Support

    }

    public virtual void ApplyFlags()
    {
        //Does nothing in the base class, overridden
    }

    private void Start()
    {
        ApplyFlags();
    }
    //End of new stuff.
    public List<Tile> GetTilesInMoveRange() => RangeManager.GetTilesInRange(OccupiedTile, moveRange, RangeType.FloodMovement);
    public List<Tile> GetTilesInAttackRange() => RangeManager.GetTilesInRange(OccupiedTile, attackRange, RangeType.FloodTargeting);

    private void Awake()
    {
        UnitAnimator = GetComponent<Animator>();
    }
    //damage functions
    public void takeDamage(float damageAmount)
    {
        if(SoundFXManager.instance != null)
        {
            SoundFXManager.instance.PlaySoundFXClip(hurtSFX, transform, 1f);
        }
        

        healthbar = GetComponentInChildren<healthbar>();
        float damage = damageAmount * defenseBoost;

        if (guard > 0)
        {
            if (guard >= damage * 0.8f)
            {
                guard -= damage * 0.8f;
            } else
            {
                damage = damage * 0.8f - guard;
                guard = 0;
                health -= damage;
            }
        } else
        {
            health -= damage;
        }

        Debug.Log($"{name} took {damageAmount} damage. Health now: {health}");
        UpdateHealth();

        if (health <= 0)
        {
            Die();
        }
    }

    public void Heal(float healthAmount)
    {
        health += healthAmount;
        if (health > maxHealth) { health = maxHealth; }
        Debug.Log("Healed for " + healthAmount + ". Current Health: " + health);
        UpdateHealth();
    }

    public void Guard(float guardAmount)
    {
        guard += guardAmount;
        if (guard > maxHealth) { guard = maxHealth; }
        Debug.Log("Guard increased by " + guardAmount + ". Current Guard: " + guard);
        UpdateHealth();
    }

    public void Boost()
    {
        attackBoost += 0.25f;
        if (attackBoost > 2)
        {
            attackBoost = 2;
        } else if (attackBoost < 0)
        {
            attackBoost = 0;
        }

        defenseBoost -= 0.25f;
        if (defenseBoost < 0)
        {
            defenseBoost = 0;
        } else if (defenseBoost > 2)
        {
            defenseBoost = 2;
        }

        GetComponentInParent<HandManager>().UpdateCardVisuals();
        Debug.Log("Blessed: Attack Boost = " + attackBoost + ", Defense Boost = " + defenseBoost);
    }

    public void Cleanse()
    {
        //Clear Debuffs
        //Temporary Implementation
        if (attackBoost < 1)
        {
            attackBoost = 1;
        }
        if (defenseBoost > 1)
        {
            defenseBoost = 1;
        }
        if (stunned > 0)
        {
            stunned = 0;
        }
        stunned = 0;
        GetComponentInParent<HandManager>().UpdateCardVisuals();
    }

    public void Energize(int energy)
    {
        GetComponent<HandManager>().actionPoints += energy;
    }

    public void Daze()
    {
        //Reduce Movement by 1
        //Temporary enemy only implementation
        if (Faction == Faction.Enemy)
        {
            if (moveRange > 0)
            {
                moveRange--;
            }
            
        }
    }

    public void Stun()
    {
        if (stunned >= 2) return;
        stunned += 2;
    }

    public void Restrict()
    {
        //Reduce Movement to 0
        //Temporary enemy only implementation
        if (Faction == Faction.Enemy)
        {
            moveRange = 0;
        }
    }

    public void Freeze()
    {
        Stun();
        defenseBoost += 0.15f;
        if (defenseBoost < 0)
        {
            defenseBoost = 0;
        } else if (defenseBoost > 2)
        {
            defenseBoost = 2;
        }
    }

    public void Invisible()
    {
        if (invisible != 0) return;
        invisible = 2;
        GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.5f);
        attackBoost += 0.15f;
        GetComponentInParent<HandManager>().UpdateCardVisuals();
    }

    public void Visible()
    {
        if (invisible == 0) return;
        invisible = 0;
        GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 0.5f);
        attackBoost -= 0.15f;
        GetComponentInParent<HandManager>().UpdateCardVisuals();
    }



    public void UpdateHealth()
    {
        healthbar.UpdateHealthBar(health, maxHealth);
        healthbar.UpdateGuardBar(guard, maxHealth);
    }

    void Die()
    {
        Debug.Log($"{name} has died.");
        if (Faction == Faction.Player) // If this is a player unit
            UnitManager.Instance.playerUnitCount -= 1; // Decrease the player unit count
        else // Otherwise this is an enemy unit
            UnitManager.Instance.enemyUnitCount -= 1; // Decrease the enemy unit count
        Destroy(gameObject);
    }

    public virtual void SelectUnit()
    {
        
    }
}
