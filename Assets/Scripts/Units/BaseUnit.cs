using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public Tile OccupiedTile;
    public Faction Faction;
    public float maxHealth;
    public float health;
    public float dmg;
    [SerializeField] healthbar healthbar;


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
