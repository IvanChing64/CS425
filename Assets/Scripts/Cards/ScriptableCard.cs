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
    public int value;
    public Sprite cardBorder, cardIcon;
    public Color cardBorderColor;
    public SupportEffect supportEffect;
    public ControlEffect controlEffect;
    public Dictionary<Vector2, Tile> validTiles;
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
    None = -1,
    Heal = 0,
    Guard = 1
}

//Control effect types
public enum ControlEffect
{
    None = -1,
    Stun = 0,
    Poison = 1
}
