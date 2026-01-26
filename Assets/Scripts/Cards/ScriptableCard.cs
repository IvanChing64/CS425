using UnityEngine;

//Developer: Bailey Escritor
//Aggregated from multiple tutorials
//Card info
[CreateAssetMenu(fileName ="New Card",menuName = "Scriptable Card")]

public class ScriptableCard : ScriptableObject
{
    public string cardName, cardDescription;
    public Type type;
    public int value;
    public Sprite cardBorder, cardIcon;
}

public enum Type
{
    Attack = 0,
    Movement = 1,
    Support = 2
}

public enum SupportEffect
{
    Heal = 0,
    Guard = 1,
}
