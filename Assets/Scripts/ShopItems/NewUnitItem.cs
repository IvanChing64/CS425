using UnityEngine;

[CreateAssetMenu(fileName = "New New Unit Item", menuName = "Scriptable Items/New Unit Item")]

public class NewUnitItem : ScriptableItem
{
    public override ItemType Type => ItemType.NewUnit;

    public ScriptableUnit newUnit;
}
