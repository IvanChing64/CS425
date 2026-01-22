using UnityEngine;

//Developer: Bailey Escritor
//Aggregated from multiple tutorials
//Card info
[CreateAssetMenu(fileName ="New Card",menuName = "Scriptable Card")]

public class ScriptableCard : ScriptableObject
{
    public string cardName {get;}
    public Type type {get;}
    public BaseCard CardPrefab {get;}
    public bool isPlayed{get;}
    //public float drawChance;
}

public enum Type
{
    Attack = 0,
    Movement = 1,
    Support = 2
}
