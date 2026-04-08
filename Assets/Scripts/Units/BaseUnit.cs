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
    public float attackModifier = 1;
    public float defenseModifier = 1;
    public int moveModifier = 0;
    public EffectFlag boost = 0;
    public EffectFlag dmgUp = 0;
    public EffectFlag defUp = 0;
    public bool defiant = false;
    public EffectFlag stunned = 0;
    public int invisible = 0;
    public int dodge = 0;
    public int poison = 0;
    public int agility = 0;
    public int moveRange;
    public int attackRange;
    public Animator UnitAnimator;
    [SerializeField] healthbar healthbar;
    [SerializeField] private AudioClip[] hurtSFX;

    public List<Tile> GetTilesInMoveRange() => RangeManager.GetTilesInRange(OccupiedTile, moveRange, RangeType.FloodMovement);
    public List<Tile> GetTilesInAttackRange() => RangeManager.GetTilesInRange(OccupiedTile, attackRange, RangeType.FloodTargeting);

    private void Awake()
    {
        UnitAnimator = GetComponent<Animator>();
    }
    //damage functions
    public void takeDamage(float damageAmount, bool dodgeable = true)
    {
        // Check for Dodge
        if (dodgeable && dodge > 0)
        {
            float dodgeChance = dodge * 0.05f;
            if (dodgeChance > 0.85f)
            {
                dodgeChance = 0.85f;
            }
            if (Random.value < dodgeChance)
            {
                return;
            }
        }

        if(SoundFXManager.instance != null)
        {
            SoundFXManager.instance.PlaySoundFXClip(hurtSFX, transform, 1f);
        }
        

        healthbar = GetComponentInChildren<healthbar>();
        float damage = damageAmount * defenseModifier;

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

        UpdateHealth();

        if (health <= 0)
        {
            if (defiant)
            {
                health = maxHealth * 0.2f;
                UpdateHealth();
                return;
            }
            Die();
        }
    }


    // SUPPORT EFFECTS
    // Heal given amount, up to max health
    public void Heal(float healthAmount)
    {
        health += healthAmount;
        if (health > maxHealth) { health = maxHealth; }
        UpdateHealth();
    }

    // Give guard equal to given amount, up to max health
    public void Guard(float guardAmount)
    {
        guard += guardAmount;
        if (guard > maxHealth) { guard = maxHealth; }
        UpdateHealth();
    }

    // Increase attack by 25%
    public void DamageUp()
    {
        attackModifier += 0.25f;
        if (attackModifier > 2)
        {
            attackModifier = 2;
        }
    }

    //Increase defense by 25%
    public void DefenseUp()
    {
        defenseModifier -= 0.25f;
        if (defenseModifier < 0)
        {
            defenseModifier = 0;
        }
    }

    // Increase attack and defense by 25%
    public void Boost()
    {
        attackModifier += 0.25f;
        if (attackModifier > 2)
        {
            attackModifier = 2;
        }

        defenseModifier -= 0.25f;
        if (defenseModifier < 0)
        {
            defenseModifier = 0;
        }

        GetComponentInParent<HandManager>().UpdateCardVisuals();
    }

    // Remove all debuffs
    public void Cleanse()
    {
        // Reset modifiers
        attackModifier = 1;
        defenseModifier = 1;
        moveModifier = 0;

        // Reapply Positive Effects
        if (dmgUp > 0)
        {
            DamageUp();
        }

        if (defUp > 0)
        {
            DefenseUp();
        }

        if (boost > 0)
        {
            Boost();
        }

        if (invisible > 0)
        {
            attackModifier += 0.15f;
        }

        if (agility > 0)
        {
            moveModifier += agility;
        }

        // Remove Debuffs
        stunned = 0;
        poison = 0;

        GetComponentInParent<HandManager>().UpdateCardVisuals();
    }

    // Increase action points by given amount
    public void Energize(int energy)
    {
        GetComponent<HandManager>().actionPoints += energy;
    }

    // Become untargetable for 2 turns, increase attack by 15%
    public void Invisible()
    {
        if (invisible != 0) return;
        invisible = 3;
        GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.5f);
        attackModifier += 0.15f;
        GetComponentInParent<HandManager>().UpdateCardVisuals();
    }

    // Remove invisibility and attack bonus
    public void Visible()
    {
        invisible = 0;
        GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 0.5f);
        attackModifier -= 0.15f;
        GetComponentInParent<HandManager>().UpdateCardVisuals();
    }

    // Apply 2 stacks of dodge, each giving a 5% chance to avoid attacks
    public void Dodge()
    {
        dodge += 2;
        if (dodge > 17)
        {
            dodge = 17;
        }
    }

    // Resist fatal damage, heal back to 20% max health
    public void Defiant()
    {
        defiant = true;
    }

    // Increase movement by 1
    public void Agility()
    {
        if (!(agility >= 3))
        {
            agility++;
        } else
        {
            agility = 3;
            return;   
        }

        moveModifier++;
        GetComponentInParent<HandManager>().UpdateCardVisuals();
    }

    // CONTROL EFFECTS
    // Reduce movement by 1
    public void Daze()
    {
        if (Faction == Faction.Enemy)
        {
            if (moveRange > 0)
            {
                moveRange--;
            }
            
        } else if (Faction == Faction.Player)
        {
            if (!(moveModifier <= -3))
            {
                moveModifier--;
            }
        }
    }

    // Stun for 1 turn, preventing card use
    public void Stun()
    {
        if ((int)stunned == 2) return;
        stunned = EffectFlag.Start;
    }

    // Reduce movement to 0 for turn
    public void Restrict()
    {
        //Reduce Movement to 0
        if (Faction == Faction.Enemy)
        {
            moveRange = 0;
        } else if (Faction == Faction.Player)
        {
            moveModifier = -99;
        }
    }

    // Stun for 1 turn and decrease defense by 15%
    public void Freeze()
    {
        Stun();
        defenseModifier += 0.10f;
        if (defenseModifier < 0)
        {
            defenseModifier = 0;
        } else if (defenseModifier > 2)
        {
            defenseModifier = 2;
        }
    }

    // Apply 1 poison stack, dealing 8% max health times the stack count as damage at end of turn, decrease damage dealt by 10%
    public void Poison()
    {
        if (poison >= 4) return;
        if (poison == 0)
        {
            attackModifier -= 0.1f;
        }
        poison++;

    }


    public virtual void ResetValues()
    {
        attackModifier = 1;
        defenseModifier = 1;
        moveModifier = 0;
        defiant = false;
        agility = 0;

        if (stunned > 0)
        {
            stunned--;
        } else
        {
            stunned = 0;
        }

        if (poison > 0)
        {
            takeDamage(maxHealth * 0.08f * poison, false);
            poison--;
            if (poison == 0)
            {
                attackModifier += 0.1f;
            }
        }

        if (dmgUp > 0)
        {
            dmgUp--;
            if (dmgUp > 0)
            {
                DamageUp();
            }
        }

        if (defUp > 0)
        {
            defUp--;
            if (defUp > 0)
            {
                DefenseUp();
            }
        }

        if (boost > 0)
        {
            boost--;
            if (boost > 0)
            {
                Boost();
            }
        }

        if (invisible > 0)
        {
            invisible--;
            if (invisible == 0)
            {
                Visible();
            }
            attackModifier += 0.15f;
        } else
        {
            invisible = 0;
        }

        if (dodge > 0)
        {
            dodge--;
        }
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
