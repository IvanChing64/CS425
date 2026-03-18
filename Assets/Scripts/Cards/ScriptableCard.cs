using UnityEngine;
using System.Collections.Generic;

//Developer: Bailey Escritor
//Aggregated from multiple tutorials, Mainly Sinuous Deckbuilding Card Game Tutorials
//Holds card data to be copied to BaseCard instances
[CreateAssetMenu(fileName ="New Card",menuName = "Scriptable Card")]

public class ScriptableCard : ScriptableObject
{
    public string cardName, cardDescription;
    public Type type;
    public int range, value, cost;
    public RangeType rangeType;
    public Sprite cardIcon, cardCorner;
    public Color cardBorderColor;
    public SupportEffect primarySupportEffect, secondarySupportEffect;
    public ControlEffect controlEffect;
}

//Card types
public enum Type
{
    Attack = 0,
    Movement = 1,
    Support = 2,
    Control = 3
}

//Support effect types
public enum SupportEffect
{
    None = 0,
    Heal = 1,
    Guard = 2,
    Bless = 3,
    Cleanse = 4,
    Energize = 5
    
}

//Control effect types
public enum ControlEffect
{
    None = 0,
    Daze = 1,
    Stun = 2,
    Restrict = 3,
    Freeze = 4,
}
