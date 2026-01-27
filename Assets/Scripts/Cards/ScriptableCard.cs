using UnityEngine;

//Developer: Bailey Escritor
//Aggregated from multiple tutorials
//Holds card data to be copied to BaseCard instances
[CreateAssetMenu(fileName ="New Card",menuName = "Scriptable Card")]

public class ScriptableCard : ScriptableObject
{
    public string cardName, cardDescription;
    public Type type;
    public int value;
    public Sprite cardBorder, cardIcon;
    public SupportEffect supportEffect;
}

//Card types
public enum Type
{
    Attack = 0,
    Movement = 1,
    Support = 2
}

//Support effect types
public enum SupportEffect
{
    None = -1,
    Heal = 0,
    Guard = 1,
}
