using System.Collections.Generic;
using UnityEngine;

//Developer: Bailey Escritor
//Derived class for support cards
public class BaseSupportCard : BaseCard
{
    public SupportEffect primaryEffect, secondaryEffect;
    //public int value;

    //Copies properties from ScriptableCard including support effect
    public override void CopyScriptableCard(ScriptableCard card)
    {
        base.CopyScriptableCard(card);
        primaryEffect = card.primarySupportEffect;
        secondaryEffect = card.secondarySupportEffect;
    }

    public override void SelectCard()
    {
        //Highlight Selectable Tiles and Targets
        BasePlayer player = CardManager.instance.selectedPlayer;

        if (player != null)
        {
            player.attackRange = range;
            if (!(player.GetComponent<HandManager>().actionPoints < cost))
            {
                player.canSupport = true;
            }

            List<Tile> tilesInRange = player.GetTilesInAttackRange();
            foreach (Tile t in tilesInRange)
            {
                t.ShowHighlight(true, Tile.supportableColor);
            }
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
            foreach (Tile t in player.GetTilesInAttackRange())
            {
                if (t.isWalkable)t.ShowHighlight(false, Tile.nonwalkableColor);
            }
            player.attackRange = 0;
        }
    }

    //Applies the support effect to the selected player
    public void ApplySupportEffect(BaseUnit targetPlayer)
    {
        switch (primaryEffect)
        {
            case SupportEffect.None:
                break;
            
            case SupportEffect.Heal:
                targetPlayer.Heal(value);
                break;

            case SupportEffect.Guard:
                targetPlayer.Guard(value);
                break;

            case SupportEffect.Boost:
                targetPlayer.Boost();
                break;

            case SupportEffect.Cleanse:
                targetPlayer.Cleanse();
                break;

            case SupportEffect.Energize:
                targetPlayer.Energize(value);
                break;

            case SupportEffect.Invisibility:
                targetPlayer.Invisible();
                break;

            case SupportEffect.Dodge:
                targetPlayer.Dodge();
                break;

            case SupportEffect.Defiant:
                targetPlayer.Defiant();
                break;
            
            case SupportEffect.Agility:
                targetPlayer.Agility();
                break;

            case SupportEffect.Strengthen:
                targetPlayer.Strengthen();
                break;

            case SupportEffect.Resistant:
                targetPlayer.Resistant();
                break;

            case SupportEffect.Reflect:
                targetPlayer.Reflect();
                break;

            case SupportEffect.Regeneration:
                targetPlayer.Regeneration();
                break;

            case SupportEffect.Absorb:
                targetPlayer.Absorb();
                break;

            default:
                break;
        }

        switch (secondaryEffect)
        {
            case SupportEffect.None:
                break;
            
            case SupportEffect.Heal:
                targetPlayer.Heal(value);
                break;

            case SupportEffect.Guard:
                targetPlayer.Guard(value);
                break;

            case SupportEffect.Boost:
                targetPlayer.Boost();
                break;

            case SupportEffect.Cleanse:
                targetPlayer.Cleanse();
                break;

            case SupportEffect.Energize:
                targetPlayer.Energize();
                break;
            
            case SupportEffect.Invisibility:
                targetPlayer.Invisible();
                break;

            case SupportEffect.Dodge:
                targetPlayer.Dodge();
                break;

            case SupportEffect.Defiant:
                targetPlayer.Defiant();
                break;

            case SupportEffect.Agility:
                targetPlayer.Agility();
                break;

            case SupportEffect.Strengthen:
                targetPlayer.Strengthen();
                break;

            case SupportEffect.Resistant:
                targetPlayer.Resistant();
                break;

            case SupportEffect.Reflect:
                targetPlayer.Reflect();
                break;

            case SupportEffect.Regeneration:
                targetPlayer.Regeneration();
                break;

            case SupportEffect.Absorb:
                targetPlayer.Absorb();
                break;

            default:
                break;
        }
        Debug.Log("Support Card Played with " + primaryEffect + " and " + secondaryEffect);
    }
}
