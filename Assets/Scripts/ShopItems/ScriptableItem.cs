using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Item")]

public class ScriptableItem : ScriptableObject
{
    [Tooltip("Describes what the item does")]
    public ItemType type;
    public Sprite itemSprite;
    public int cost;
}

/// <summary>
/// Enum for different item types
/// </summary>
public enum ItemType
{
    // Switches out a base unit for a certain upgrade unit
    UnitUpgrade = 1,
    // Adds a unit to the party
    NewUnit = 2,
    // Buffs the party's stats
    PartyBuff = 3,
    // Adds a card each of the party member's decks
    DeckAddition = 4,


}
