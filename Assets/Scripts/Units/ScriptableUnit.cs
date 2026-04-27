using UnityEngine;
//Unit info
[CreateAssetMenu(fileName ="New Unit",menuName = "Scriptable Unit")]

public class ScriptableUnit : ScriptableObject
{
    public Faction Faction;
    public BaseUnit UnitPrefab;
    public float health;
    public float dmg;
    public int energy;
}

public enum Faction
{
    Player = 0,
    Enemy = 1
}

/// <summary>
/// Describes how many turns an effect is active
/// </summary>
public enum EffectFlag : byte
{
    Start = 3,
    Middle = 2,
    End = 1,
    None = 0
}
    