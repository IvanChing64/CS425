using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;
    public Faction Faction;
    public float maxHealth;
    public float health;
    public float dmg;
    public int moveRange;
    [SerializeField] healthbar healthbar;

    public List<Tile> GetTilesInMoveRange() => MovementManager.Instance.GetTilesInRange(OccupiedTile, moveRange);

    //damage functions
    public void takeDamage(float damageAmount)
    {
        healthbar = GetComponentInChildren<healthbar>();
        health -= damageAmount;
        Debug.Log($"{name} took {damageAmount} damage. Health now: {health}");
        healthbar.UpdateHealthBar(health, maxHealth);
        if (health <= 0)
        {
            Die();
        }
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
