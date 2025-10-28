using UnityEngine;
//Unit info
[CreateAssetMenu(fileName ="New Unit",menuName = "Scriptable Unit")]

public class ScriptableUnit : ScriptableObject
{
    public Faction Faction;
    public BaseUnit UnitPrefab;
}

public enum Faction
{
    Player = 0,
    Enemy = 1
}
    