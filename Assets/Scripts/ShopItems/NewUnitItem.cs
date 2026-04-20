using UnityEngine;

public class NewUnitItem : ScriptableItem
{
    public override ItemType Type => ItemType.NewUnit;

    public ScriptableUnit newUnit;
}
