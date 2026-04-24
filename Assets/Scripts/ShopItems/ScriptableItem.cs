using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Items/Default Item")]

public class ScriptableItem : ScriptableObject
{ 
    public virtual ItemType Type => ItemType.DefaultItem;

    //[Tooltip("Describes what the item does")]
    //public ItemType type;

    public Sprite itemSprite;
    public string itemDesc;
    public int cost;
}

/// <summary>
/// Enum for different item types
/// </summary>
public enum ItemType
{
    DefaultItem = 0,
    /// <summary>Switches out a base unit for a certain upgrade unit</summary>
    UnitUpgrade = 1,
    /// <summary>Adds a unit to the party</summary>
    NewUnit = 2,
    /// <summary> Buffs the party's stats</summary>
    PartyBuff = 3,
    /// <summary>Adds a card each of the party member's decks</summary>
    DeckAddition = 4,
}
