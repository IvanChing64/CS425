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
    public int moveRange;
    public int attackRange;
    [SerializeField] healthbar healthbar;
    [SerializeField] private AudioClip[] hurtSFX;

    public List<Tile> GetTilesInMoveRange() => RangeManager.GetTilesInRange(OccupiedTile, moveRange, RangeType.FloodMovement);
    public List<Tile> GetTilesInAttackRange() => RangeManager.GetTilesInRange(OccupiedTile, attackRange, RangeType.FloodTargeting);


    //damage functions
    public void takeDamage(float damageAmount)
    {
        SoundFXManager.instance.PlaySoundFXClip(hurtSFX, transform, 1f);

        healthbar = GetComponentInChildren<healthbar>();
        float damage = damageAmount;

        if (guard > 0)
        {
            if (guard >= damage * 0.75f)
            {
                guard -= damage * 0.75f;
            } else
            {
                damage = damage * 0.75f - guard;
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
}
