using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Unit Army Slot", menuName = "Scriptable Army Slots")]

public class ScriptableArmySlot : ScriptableObject
{
    public float health;
    public int energy;
    public string description;
    public Sprite unitImage;
}
