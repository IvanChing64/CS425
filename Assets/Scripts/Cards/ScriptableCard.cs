using UnityEngine;
//Card info
[CreateAssetMenu(fileName ="New Card",menuName = "Scriptable Card")]

public class ScriptableCard : ScriptableObject
{
    public Type type;
    public BaseCard CardPrefab;
    public bool isPlayed;
}

public enum Type
{
    Attack = 0,
    Movement = 1,
    Support = 2
}
