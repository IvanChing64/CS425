using UnityEngine;
//Card info
[CreateAssetMenu(fileName ="New Card",menuName = "Scriptable Card")]

public class ScriptableCard : ScriptableObject
{
    public string cardName;
    public Type type;
    public BaseCard CardPrefab;
    public bool isPlayed;
    public float drawChance;
}

public enum Type
{
    Attack = 0,
    Movement = 1,
    Support = 2
}
