using UnityEngine;
using System.Collections.Generic;

//Developer: Bailey Escritor
//Aggregated from multiple tutorials, Mainly Sinuous Deckbuilding Card Game Tutorials
//Holds card data to be copied to BaseCard instances
[CreateAssetMenu(fileName ="New Card",menuName = "Scriptable Card")]

public class ScriptableCard : ScriptableObject
{
    public string cardName, cardDescription;
    [Tooltip("Attack, Movement, Support, or Summon")]
    public Type type;
    public int range, value, cost;
    [Tooltip("If and How it targets multiple units")]
    public AreaOfEffectType areaEffect;
    public RangeType rangeType;
    public Sprite cardIcon, cardCorner;
    public Color cardBorderColor;
    public SupportEffect primarySupportEffect, secondarySupportEffect;
    public ControlEffect primaryControlEffect, secondaryControlEffect;
}

/// <summary>
/// Enum for different card types
/// </summary>
public enum Type
{
    Attack = 0,
    Movement = 1,
    Support = 2,
    Summon = 3
}

/// <summary>
/// Enum for different Support Effects
/// </summary>
public enum SupportEffect
{
    None = 0,
    Heal = 1,
    Guard = 2,
    Boost = 3,
    Cleanse = 4,
    Energize = 5,
    Invisibility = 6,
    Dodge = 7,
    Defiant = 8,
    Agility = 9,
    Strengthen = 10,
    Resistant = 11,
    Reflect = 12, 
    Regeneration = 13,
    Absorb = 14
}

/// <summary>
/// Enum for all different Control Effects
/// </summary>
public enum ControlEffect
{
    None = 0,
    Daze = 1,
    Stun = 2,
    Restrict = 3,
    Freeze = 4,
    Poison = 5, 
    Flaming = 6,
    Weaken = 7,
    Vulnerable = 8, 
    Hinder = 9,
    Expose = 10
}

/// <summary>
/// Enum for different ways a card can affect multiple units at once
/// </summary>
public enum AreaOfEffectType
{
    None = 0,
    Inclusive = 1,
    Exclusive = 2,
}

/// <summary>
/// Enum for different available summons
/// </summary>
public enum Summons
{
    None = 0,
    Pawn = 1,
    Raven = 2,
    Snake = 3, 
    Turret = 4, 
    Generator = 5,
    Drone = 6, 
}
