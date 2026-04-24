using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    [Header("Unit Stats")]
    [Tooltip("Unit Name")]
    public string Name;
    [Tooltip("Tile the unit is occupying")]
    public Tile OccupiedTile;
    [Tooltip("Ally or Enemy")]
    public Faction Faction;
    [Tooltip("Unit was summoned and has a lifetime")]
    public bool summoned;
    [Tooltip("Lifetime of summon")]
    public int lifetime;
    [Tooltip("Maximum Health of the unit")]
    public float maxHealth;
    [Tooltip("Current Health of the unit")]
    public float health;
    [Tooltip("Current Guard of the unit, absorbs damage before health")]
    public float guard;
    [Tooltip("Amount of damage unit can deal")]
    public float dmg;
    [Tooltip("Attack Damage Multiplier")]
    public float attackModifier = 1;
    [Tooltip("Taken Damage Multiplier")]
    public float defenseModifier = 1;
    [Tooltip("Movement Adder")]
    public int moveModifier = 0;
    [Tooltip("Movement Range")]
    public int moveRange;
    [Tooltip("Attack Range")]
    public int attackRange;


    [Header("Buff Flags")]
    [Tooltip("Increase attack & defense by 25%")]
    public EffectFlag boost = EffectFlag.None;
    [Tooltip("Increase attack by 25%")]
    public EffectFlag strengthen = EffectFlag.None;
    [Tooltip("Increase defense by 25%")]
    public EffectFlag resistant = EffectFlag.None;
    [Tooltip("Increase attack by 15% and become untargetable")]
    public EffectFlag invisible = EffectFlag.None;
    [Tooltip("Cleanse and resist control effects")]
    public EffectFlag immune = EffectFlag.None;

    [Header("Debuff Flags")]
    [Tooltip("Stop all actions")]
    public EffectFlag stunned = EffectFlag.None;
    [Tooltip("Stop all movement")]
    public EffectFlag restricted = EffectFlag.None;
    [Tooltip("Stop all actions and take 10% more damage")]
    public EffectFlag frozen = EffectFlag.None;
    [Tooltip("Lose 15% of remaing health and take 15% of max health as damage")]
    public EffectFlag flaming = EffectFlag.None;
    [Tooltip("Decrease attack by 25%")]
    public EffectFlag weaken = EffectFlag.None;
    [Tooltip("Decrease defense by 25%")]
    public EffectFlag vulnerable = EffectFlag.None;
    [Tooltip("Decrease attack & defense by 25%")]
    public EffectFlag hinder = EffectFlag.None;

    [Header("Other Buffs")]
    [Tooltip("Heal 10% of max health for each stack, decrements")]
    public int regeneration = 0;
    [Tooltip("Gain 5% chance to avoid damage for each stack, decrements")]
    public int dodge = 0;
    [Tooltip("Increase movement by 1 for each stack")]
    public int agility = 0;
    [Tooltip("Reflect an instance of damage taken back to attacker")]
    public int reflect = 0;
    [Tooltip("Convert an instance of damage into healing, at 50% conversion")]
    public int absorb = 0;
    [Tooltip("Heal to 20% of max health instead of dying")]
    public bool defiant = false;

    [Header("Other Debuffs")]
    [Tooltip("Reduce movement by 1 for each stack")]
    public int daze = 0;
    [Tooltip("Take 9% of max health as damage for each stack and deal 10% less damage, decrements")]
    public int poison = 0;

    [Header("Misc. Components")]
    public Animator UnitAnimator;
    [SerializeField] healthbar healthbar;
    [SerializeField] private AudioClip[] hurtSFX;

    //Enemy Flags: Andrew Shelton
    public enum EnemyFlag
    {
        Ranged,
        Default,
        Support

    }

    public enum MovementBehavior
    {
        Default,
        Ranged,
        Support,
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
    public List<Tile> GetTilesInAOEAttackRange(Tile rangedCenter, int range) => RangeManager.GetTilesInRange(rangedCenter, range, RangeType.FloodTargeting);
    public List<Tile> GetTilesInUnrestrictedMoveRange() => RangeManager.GetTilesInRange(OccupiedTile, moveRange, RangeType.FloodMovementUnrestricted);

    private void Awake()
    {
        UnitAnimator = GetComponent<Animator>();
        
    }

    //damage functions
    public void takeDamage(float damageAmount, bool dodgeable = true, bool pierce = false, BaseUnit attacker = null, bool poison = false)
    {
        healthbar = GetComponentInChildren<healthbar>();

        float damage = damageAmount * defenseModifier;
        float absorbAmount;

        // Check for Pierce Damage
        if (!pierce)
        {
            // Check for Reflect
            if (reflect > 0)
            {
                attacker.takeDamage(damage * UnitManager.reflectEfficiency, true, false);
                reflect--;
                return;
            }

            // Check for Absorb
            if (absorb > 0)
            {
                absorbAmount = damage * UnitManager.absorbEfficiency;
                Heal(absorbAmount);
                absorb--;
                return;
            } 
        } else
        {
            if (damage < damageAmount)
            {
                damage = damageAmount;
            }
        }

        // Check for Dodge
        if (dodgeable && dodge > 0)
        {
            float dodgeChance = dodge * 0.05f;
            if (dodgeChance > UnitManager.maxDodgeChance)
            {
                dodgeChance = UnitManager.maxDodgeChance;
            }
            if (Random.value < dodgeChance)
            {
                dodge--;
                return;
            }
        }

        if (!poison && guard > 0)
        {
            if (pierce)
            {
                if (guard >= damage * 2 * UnitManager.guardEfficiency)
                {
                    guard -= damage * 2 * UnitManager.guardEfficiency;
                } else
                {
                    damage = damage * UnitManager.guardEfficiency - guard;
                    guard = 0;
                    health -= damage;
                } 
            } else
            {
                if (guard >= damage * UnitManager.guardEfficiency)
                {
                    guard -= damage * UnitManager.guardEfficiency;
                } else
                {
                    damage = damage * UnitManager.guardEfficiency - guard;
                    guard = 0;
                    health -= damage;
                } 
            }
            
        } else
        {
            health -= damage;
        }

        if(SoundFXManager.instance != null)
        {
            SoundFXManager.instance.PlaySoundFXClip(hurtSFX, transform, 1f);
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
    public void Heal(float healthAmount = 15)
    {
        health += healthAmount;
        if (health > maxHealth) { health = maxHealth; }
        UpdateHealth();
    }

    // Give guard equal to given amount, up to max health
    public void Guard(float guardAmount = 15)
    {
        guard += guardAmount;
        if (guard > maxHealth) { guard = maxHealth; }
        UpdateHealth();
    }

    // Increase attack and defense by 25%
    public void Boost(bool reapply = false)
    {
        if (!reapply)
        {
            if ((int)boost > 0)
            {
                boost = EffectFlag.Middle;
                return;
            }
            
            boost = EffectFlag.Middle;
        }

        attackModifier += UnitManager.boostHinderValue;
        if (attackModifier > 2)
        {
            attackModifier = 2;
        }

        defenseModifier -= UnitManager.boostHinderValue;
        if (defenseModifier < 0)
        {
            defenseModifier = 0;
        }

        if (Faction == Faction.Player)
        {
            GetComponentInParent<HandManager>().UpdateCardVisuals();
        }
    }

    // Remove all debuffs
    public void Cleanse()
    {
        // Reset modifiers
        attackModifier = 1;
        defenseModifier = 1;
        moveModifier = 0;

        // Reapply Positive Effects
        if (strengthen > 0)
        {
            Strengthen(true);
        }

        if (resistant > 0)
        {
            Resistant(true);
        }

        if (boost > 0)
        {
            Boost(true);
        }

        if (invisible > 0)
        {
            attackModifier += UnitManager.invisibleAttackBoost;
        }

        if (agility > 0)
        {
            moveModifier += agility;
        }

        // Remove Debuffs
        daze = 0;
        poison = 0;
        stunned = EffectFlag.None;
        restricted = EffectFlag.None;
        frozen = EffectFlag.None;
        flaming = EffectFlag.None;
        weaken = EffectFlag.None;
        vulnerable = EffectFlag.None;
        hinder = EffectFlag.None;

        if (Faction == Faction.Player)
        {
            GetComponentInParent<HandManager>().UpdateCardVisuals();
        }
    }

    // Increase action points by given amount
    public void Energize(int energy = 1)
    {
        GetComponent<HandManager>().actionPoints += energy;
    }

    // Become untargetable, increase attack by 15%
    public void Invisible(bool visible = false)
    {
        if (visible)
        {
            // Become Visible
            invisible = 0;
            GetComponent<SpriteRenderer>().color += new Color(0, 0, 0, 0.5f);
            attackModifier -= UnitManager.invisibleAttackBoost;
            if (Faction == Faction.Player)
            {
                GetComponentInParent<HandManager>().UpdateCardVisuals();
            }
        } else
        {
            // Become Invisible
            if (invisible != 0) return;
            invisible = EffectFlag.Start;
            GetComponent<SpriteRenderer>().color -= new Color(0, 0, 0, 0.5f);
            attackModifier += UnitManager.invisibleAttackBoost;
            if (Faction == Faction.Player)
            {
                GetComponentInParent<HandManager>().UpdateCardVisuals();
            }
        }
    }

    // Apply 2 stacks of dodge, each giving a 5% chance to avoid attacks
    public void Dodge()
    {
        dodge += 3;
        if (dodge > 18)
        {
            dodge = 18;
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
        if (Faction == Faction.Player)
        {
            GetComponentInParent<HandManager>().UpdateCardVisuals();
        }
    }

    // Increase attack by 25%
    public void Strengthen(bool reapply = false)
    {
        if (!reapply)
        {
            if ((int)strengthen > 0)
            {
                strengthen = EffectFlag.Middle;
                return;
            }

            strengthen = EffectFlag.Middle;
        }

        attackModifier += UnitManager.strengthenWeakenValue;
        if (attackModifier > 2)
        {
            attackModifier = 2;
        }

        if (Faction == Faction.Player)
        {
            GetComponentInParent<HandManager>().UpdateCardVisuals();
        }
    }

    // Increase defense by 25%
    public void Resistant(bool reapply = false)
    {
        if (!reapply)
        {
            if ((int)resistant > 0)
            {
                resistant = EffectFlag.Middle;
                return;
            }

            resistant = EffectFlag.Middle;
        }

        defenseModifier -= UnitManager.resistantVulnerableValue;
        if (defenseModifier < 0)
        {
            defenseModifier = 0;
        }

        if (Faction == Faction.Player)
        {
            GetComponentInParent<HandManager>().UpdateCardVisuals();
        }
    }

    // Reflect 75% damage taken back to the attacker
    public void Reflect(int stacks = 1)
    {
        reflect += stacks;
    }

    // Heal for 10% times number of stacksof max health at the end of the turn
    public void Regeneration(int stacks = 1)
    {
        regeneration += stacks;
        if (regeneration >= UnitManager.maxRegenStacks)
        {
            regeneration = UnitManager.maxRegenStacks;
        }
    }

    // Heal 50% of damage dealt to the player
    public void Absorb(int stacks = 1)
    {
        absorb += stacks;
    }

    // Become immune to status effects
    public void Immune()
    {
        Cleanse();
        if (immune == EffectFlag.Middle) return;
        immune = EffectFlag.Middle;
    }

    // CONTROL EFFECTS
    // Reduce movement by 1
    public void Daze()
    {
        if (!(daze >= 3))
        {
            daze++;
        } else
        {
            daze = 3;
            return;   
        }

        moveModifier--;

        if (Faction == Faction.Enemy)
        {
            if (moveRange - moveModifier < 0)
            {
                moveModifier++;
            }
        }

        if (Faction == Faction.Player)
        {
            GetComponentInParent<HandManager>().UpdateCardVisuals();
        }
        
    }

    // Stun for 1 turn, preventing card use
    public void Stun()
    {
        if (immune > 0)
        {
            return;
        }
        if (stunned == EffectFlag.End) return;
        stunned = EffectFlag.End;
    }

    // Reduce movement to 0 for turn
    public void Restrict()
    {
        if (immune > 0)
        {
            return;
        }
        if (restricted == EffectFlag.End) return;
        restricted = EffectFlag.End;
        if (Faction == Faction.Player) moveModifier = -99;
    }

    // Stun for 1 turn and decrease defense by 15%
    public void Freeze(bool reapply = false)
    {
        if (immune > 0)
        {
            return;
        }
        if (!reapply) {
            if (frozen == EffectFlag.Middle ) return;
            if (frozen == EffectFlag.End)
            {
                frozen = EffectFlag.Middle;
                return;
            }
        }

        Stun();
        defenseModifier += UnitManager.frozenDefenseDown;
        if (defenseModifier > 2)
        {
            defenseModifier = 2;
        }
    }

    // Apply poison stacks, dealing 10% of remaining health times the stack count as damage at end of turn, decrease damage dealt by 10%
    public void Poison(int stacks = 1)
    {
        if (immune > 0)
        {
            return;
        }
        if (poison >= 6) 
        {
            poison = 6;
            return;
        }
        if (poison == 0)
        {
            attackModifier -= UnitManager.poisonAttackDown;
        }
        poison += stacks;
    }
    
    // Take flaming damage for 2 turns, equal to 15% of remaining health and 20% of max health each turn
    public void Flaming()
    {
        if (immune > 0)
        {
            return;
        }
        flaming = EffectFlag.Start;
    }

    // Reduce attack by 25% for 2 turns
    public void Weaken(bool reapply = false)
    {
        if (immune > 0)
        {
            return;
        }
        if (!reapply)
        {
            if ((int)weaken > 0)
            {
                weaken = EffectFlag.Middle;
                return;
            }

            weaken = EffectFlag.Middle;
        }

        attackModifier -= UnitManager.strengthenWeakenValue;
        if (attackModifier < 0)
        {
            attackModifier = 0;
        }

        if (Faction == Faction.Player)
        {
            GetComponentInParent<HandManager>().UpdateCardVisuals();
        }
    }

    // Reduce defense by 25% for 2 turns
    public void Vulnerable(bool reapply = false)
    {
        if (immune > 0)
        {
            return;
        }
        if (!reapply)
        {
            if ((int)vulnerable > 0)
            {
                vulnerable = EffectFlag.Middle;
                return;
            }

            vulnerable = EffectFlag.Middle;
        }

        defenseModifier += UnitManager.resistantVulnerableValue;
        if (defenseModifier > 2)
        {
            defenseModifier = 2;
        }

        if (Faction == Faction.Player)
        {
            GetComponentInParent<HandManager>().UpdateCardVisuals();
        }
    }

    // Reduce attack and defense by 25% for 2 turns
    public void Hinder(bool reapply = false)
    {
        if (immune > 0)
        {
            return;
        }
        if (!reapply)
        {
            if ((int)hinder > 0)
            {
                hinder = EffectFlag.Middle;
                return;
            }
            
            hinder = EffectFlag.Middle;
        }

        attackModifier -= UnitManager.boostHinderValue;
        if (attackModifier < 0)
        {
            attackModifier = 0;
        }

        defenseModifier += UnitManager.boostHinderValue;
        if (defenseModifier > 2)
        {
            defenseModifier = 2;
        }

        if (Faction == Faction.Player)
        {
            GetComponentInParent<HandManager>().UpdateCardVisuals();
        }
    }

    // Remove all buffs
    public void Expose()
    {
        if (immune > 0)
        {
            return;
        }
        // Reset modifiers
        attackModifier = 1;
        defenseModifier = 1;
        moveModifier = 0;

        // Reapply Negative Effects
        if (weaken > 0)
        {
            Weaken(true);
        }

        if (vulnerable > 0)
        {
            Vulnerable(true);
        }

        if (hinder > 0)
        {
            Hinder(true);
        }

        if (frozen > 0)
        {
            Freeze(true);
        }

        if (daze > 0)
        {
            moveModifier -= daze;
        }

        if (poison > 0)
        {
            attackModifier -= UnitManager.poisonAttackDown;
        }

        // Remove Buffs
        dodge = 0;
        agility = 0;
        regeneration = 0;
        reflect = 0;
        absorb = 0;
        defiant = false;
        boost = EffectFlag.None;
        strengthen = EffectFlag.None;
        resistant = EffectFlag.None;
        invisible = EffectFlag.None;

        if (Faction == Faction.Player)
        {
            GetComponentInParent<HandManager>().UpdateCardVisuals();
        }
    }

    // Reset all values and decrement all effects at the end of the turn
    public virtual void ResetValues()
    {
        // if (summoned)
        // {
        //     lifetime--;
        //     if (lifetime == 0)
        //     {
        //         Die();
        //     }
        // }

        // attackModifier = 1;
        // defenseModifier = 1;
        // moveModifier = 0;
        reflect = 0;
        absorb = 0;
        defiant = false;
        // agility = 0;

        // if (immune > 0)
        // {
        //     immune--;
        // }

        // if (invisible > 0)
        // {
        //     invisible--;
        //     attackModifier += UnitManager.invisibleAttackBoost;
        //     if (invisible == 0)
        //     {
        //         Invisible(true);
        //     }
        // }

        // if (daze > 0)
        // {
        //     daze--;
        //     moveModifier -= daze;
        // }

        // if (strengthen > 0)
        // {
        //     strengthen--;
        //     if (strengthen > 0)
        //     {
        //         Strengthen(true);
        //     }
        // }

        // if (stunned > 0)
        // {
        //     stunned--;
        // }

        // if (frozen > 0)
        // {
        //     frozen--;
        //     if (frozen > 0)
        //     {
        //         Freeze(true);
        //     }
        // }

        // if (weaken > 0)
        // {
        //     weaken--;
        //     if (weaken > 0)
        //     {
        //         Weaken(true);
        //     }
        // }

        // if (regeneration > 0)
        // {
        //     Heal(maxHealth * 0.1f * regeneration);
        //     regeneration--;
        // }

        // if (poison > 0)
        // {
        //     takeDamage(health * 0.1f * poison, false, false);
        //     poison--;
        //     if (poison == 0)
        //     {
        //         attackModifier += UnitManager.poisonAttackDown;
        //     }
        // }

        // if (flaming > 0)
        // {
        //     takeDamage(health * 0.2f, false, false);
        //     takeDamage(maxHealth * 0.15f, false, false);
        //     flaming--;
        // }

        // if (boost > 0)
        // {
        //     boost--;
        //     if (boost > 0)
        //     {
        //         Boost(true);
        //     }
        // }

        // if (hinder > 0)
        // {
        //     hinder--;
        //     if (hinder > 0)
        //     {
        //         Hinder(true);
        //     }
        // }

        // if (resistant > 0)
        // {
        //     resistant--;
        //     if (resistant > 0)
        //     {
        //         Resistant(true);
        //     }
        // }

        // if (vulnerable > 0)
        // {
        //     vulnerable--;
        //     if (vulnerable > 0)
        //     {
        //         Vulnerable(true);
        //     }
        // }

        // if (restricted > 0)
        // {
        //     restricted--;
        // }

        if (dodge > 0)
        {
            dodge--;
        }

    }

    public void ApplyEndTurnEffects()
    {
        if (summoned)
        {
            lifetime--;
            if (lifetime == 0)
            {
                Die();
            }
        }

        attackModifier = 1;
        defenseModifier = 1;
        moveModifier = 0;
        //reflect = 0;
        //absorb = 0;
        //defiant = false;
        agility = 0;

        if (immune > 0)
        {
            immune--;
        }

        if (invisible > 0)
        {
            invisible--;
            attackModifier += UnitManager.invisibleAttackBoost;
            if (invisible == 0)
            {
                Invisible(true);
            }
        }

        if (daze > 0)
        {
            daze--;
            moveModifier -= daze;
        }

        if (strengthen > 0)
        {
            strengthen--;
            if (strengthen > 0)
            {
                Strengthen(true);
            }
        }

        if (stunned > 0)
        {
            stunned--;
        }

        if (frozen > 0)
        {
            frozen--;
            if (frozen > 0)
            {
                Freeze(true);
            }
        }

        if (weaken > 0)
        {
            weaken--;
            if (weaken > 0)
            {
                Weaken(true);
            }
        }

        if (regeneration > 0)
        {
            for (int i = 1; i <= regeneration; i++)
            {
                if (i <= 3)
                {
                    Heal(maxHealth * 0.15f);
                } else if (i == 4)
                {
                    Heal(maxHealth * 0.1f);
                } else if (i == UnitManager.maxRegenStacks)
                {
                    Heal(maxHealth * 0.05f);
                }
            }
            //Heal(maxHealth * 0.1f * regeneration);
            regeneration--;
        }

        if (poison > 0)
        {
            takeDamage(health * 0.1f * poison, false, false, null, true);
            poison--;
            if (poison == 0)
            {
                attackModifier += UnitManager.poisonAttackDown;
            }
        }

        if (flaming > 0)
        {
            takeDamage(health * 0.2f, false, true);
            takeDamage(maxHealth * 0.15f, false, true);
            flaming--;
        }

        if (boost > 0)
        {
            boost--;
            if (boost > 0)
            {
                Boost(true);
            }
        }

        if (hinder > 0)
        {
            hinder--;
            if (hinder > 0)
            {
                Hinder(true);
            }
        }

        if (resistant > 0)
        {
            resistant--;
            if (resistant > 0)
            {
                Resistant(true);
            }
        }

        if (vulnerable > 0)
        {
            vulnerable--;
            if (vulnerable > 0)
            {
                Vulnerable(true);
            }
        }

        if (restricted > 0)
        {
            restricted--;
        }

        // if (dodge > 0)
        // {
        //     dodge--;
        // }
    }


    public void UpdateHealth()
    {
        healthbar.UpdateHealthBar(health, maxHealth);
        healthbar.UpdateGuardBar(guard, maxHealth);
    }

    void Die()
    {
        Debug.Log($"{name} has died.");
        if (Faction == Faction.Player)
        { 
            // If this is a player unit
            UnitManager.Instance.playerUnitCount -= 1; // Decrease the player unit count
            UnitManager.Instance.playersSpawned.Remove((BasePlayer)this);
        }
        else  
        {
            // Otherwise this is an enemy unit
            UnitManager.Instance.enemyUnitCount -= 1; // Decrease the enemy unit count
            UnitManager.Instance.enemiesSpawned.Remove((BaseEnemy)this);
        }
        Destroy(gameObject);
    }

    public virtual void SelectUnit()
    {
        
    }
}
