using JetBrains.Annotations;
using UnityEngine;

public class UnitUpgradeItem : ScriptableItem
{
    public override ItemType Type => ItemType.UnitUpgrade;

    public ScriptableUnit originalUnit, upgradedUnit;
}
