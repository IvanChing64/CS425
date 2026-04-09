using Codice.Client.Common.GameUI;
using System.Collections.Generic;
using Unity.Plastic.Antlr3.Runtime.Tree;
using UnityEditor.EditorTools;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;





public class BaseUnit : MonoBehaviour
{
    [Header("Unit Stats")]
    [Tooltip("Tile the unit is occupying")]
    public Tile OccupiedTile;
    [Tooltip("Ally or Enemy")]
    public Faction Faction;
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
    [Tooltip("Heal 25% of damage taken")]
    public EffectFlag absorb = EffectFlag.None;

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
    [Tooltip("Reflect 25% of damage taken back to attacker")]
    public bool reflect = false;
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

    private void Awake()
    {
        UnitAnimator = GetComponent<Animator>();
    }
    //damage functions
    public void takeDamage(float damageAmount, bool dodgeable = true, BaseUnit attacker = null)
    {
        // Check for Dodge
        if (dodgeable && dodge > 0)
        {
            float dodgeChance = dodge * 0.05f;
            if (dodgeChance > UnitManager.maxDodgeChance)
            {
                dodgeChance = 0.85f;
            }
            if (Random.value < UnitManager.maxDodgeChance)
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

        if (reflect && attacker != null)
        {
            attacker.takeDamage(damage * 0.25f);
        }

        if (guard > 0)
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
        } else
        {
            health -= damage;
        }

        if (absorb > 0)
        {
            Heal(damage * 0.25f);
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
            invisible = EffectFlag.Middle;
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

    // Reflect 25% of damage taken back to the attacker
    public void Reflect()
    {
        reflect = true;
    }

    // Heal for 10% times number of stacksof max health at the end of the turn
    public void Regeneration()
    {
        if (regeneration >= 8) return;
        regeneration++;
    }

    // Heal 20% of damage dealt to the player
    public void Absorb()
    {
        absorb = EffectFlag.Middle;
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
        if (Faction == Faction.Player)
        {
            GetComponentInParent<HandManager>().UpdateCardVisuals();
        }
        
    }

    // Stun for 1 turn, preventing card use
    public void Stun()
    {
        if (stunned == EffectFlag.Middle) return;
        stunned = EffectFlag.Middle;
    }

    // Reduce movement to 0 for turn
    public void Restrict()
    {
        //Reduce Movement to 0
        if (restricted == EffectFlag.Middle) return;
        restricted = EffectFlag.Middle;
    }

    // Stun for 1 turn and decrease defense by 15%
    public void Freeze(bool reapply = false)
    {
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

    // Apply 1 poison stack, dealing 8% max health times the stack count as damage at end of turn, decrease damage dealt by 10%
    public void Poison()
    {
        if (poison >= 8) return;
        if (poison == 0)
        {
            attackModifier -= UnitManager.poisonAttackDown;
        }
        poison++;
    }
    
    // Take flaming damage for 2 turns, equal to 15% of remaining health and 15% of max health each turn
    public void Flaming()
    {
        flaming = EffectFlag.Start;
    }

    // Reduce attack by 25% for 2 turns
    public void Weaken(bool reapply = false)
    {
        if (!reapply)
        {
            if ((int)weaken > 0)
            {
                weaken = EffectFlag.Middle;
                return;
            }
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
        if (!reapply)
        {
            if ((int)vulnerable > 0)
            {
                vulnerable = EffectFlag.Middle;
                return;
            }
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
        defiant = false;
        reflect = false;
        boost = EffectFlag.None;
        strengthen = EffectFlag.None;
        resistant = EffectFlag.None;
        invisible = EffectFlag.None;
        absorb = EffectFlag.None;

        if (Faction == Faction.Player)
        {
            GetComponentInParent<HandManager>().UpdateCardVisuals();
        }
    }

    // Reset all values and decrement all effects at the end of the turn
    public virtual void ResetValues()
    {
        attackModifier = 1;
        defenseModifier = 1;
        moveModifier = 0;
        defiant = false;
        reflect = false;
        agility = 0;
        daze = 0;

        if (invisible > 0)
        {
            invisible--;
            attackModifier += UnitManager.invisibleAttackBoost;
            if (invisible == 0)
            {
                Invisible(true);
            }
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
            Heal(maxHealth * 0.1f * regeneration);
            regeneration--;
        }

        if (flaming > 0)
        {
            takeDamage(health * 0.15f, false);
            takeDamage(maxHealth * 0.15f, false);
            flaming--;
        }

        if (poison > 0)
        {
            takeDamage(maxHealth * 0.09f * poison, false);
            poison--;
            if (poison == 0)
            {
                attackModifier += UnitManager.poisonAttackDown;
            }
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

        if (absorb > 0)
        {
            absorb--;
        }

        if (restricted > 0)
        {
            restricted--;
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
