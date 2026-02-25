using UnityEngine;

//Developer: Bailey Escritor
//Derived class for support cards
public class BaseSupportCard : BaseCard
{
    public SupportEffect effect;
    //public int value;

    //Copies properties from ScriptableCard including support effect
    public override void CopyScriptableCard(ScriptableCard card)
    {
        base.CopyScriptableCard(card);
        effect = card.supportEffect;
    }

    public override void SelectCard()
    {
        //Highlight Selectable Tiles and Targets
        BasePlayer player = CardManager.instance.selectedPlayer;

        if (player != null)
        {
            player.canSupport = true;
            player.OccupiedTile.ShowHighlight(true, Tile.supportableColor);
        }
    }

    public override void DeselectCard()
    {
        //Unhighlight Selectable Tiles and Targets
        BasePlayer player = CardManager.instance.selectedPlayer;

        if (player != null)
        {
            player.canSupport = false;
            player.OccupiedTile.ShowHighlight(false, Tile.nonwalkableColor);
        }
    }

    //Applies the support effect to the selected player
    public void ApplySupportEffect(BaseUnit targetPlayer)
    {
        switch (effect)
        {
            case SupportEffect.Heal:
                targetPlayer.Heal(value);
                break;

            case SupportEffect.Guard:
                targetPlayer.Guard(value);
                break;

            default:
                Debug.LogWarning("Unknown support effect: " + effect);
                break;
        }
        Debug.Log("Support Card Played with " + effect + " of value: " + value);
    }
}
