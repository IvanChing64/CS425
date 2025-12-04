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
        Destroy(gameObject);
    }
}
