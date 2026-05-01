using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is used to store and update values for item slots in the shop scene
/// </summary>
public class ShopItemSlot : MonoBehaviour
{
    public Text itemDescription;
    public Text costText;
    public Image itemImage;
    public Image itemButtonImage;

    // Item color modifiers
    public static Color NewUnitItemColor => Color.lightGreen;
    public static Color UnitUpgradeItemColor => Color.lightGoldenRod;
}
