using JetBrains.Annotations;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit Upgrade Item", menuName = "Scriptable Items/Unit Upgrade Item")]
public class UnitUpgradeItem : ScriptableItem
{
    public override ItemType Type => ItemType.UnitUpgrade;

    public ScriptableUnit originalUnit, upgradedUnit;
}
