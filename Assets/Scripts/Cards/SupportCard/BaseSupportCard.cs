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
                if (!(targetPlayer.boost > 0))
                {
                    targetPlayer.boost = EffectFlag.Start;
                    targetPlayer.Boost();
                }
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

            case SupportEffect.DamageUp:
                if (!(targetPlayer.dmgUp > 0))
                {
                    targetPlayer.dmgUp = EffectFlag.Start;
                    targetPlayer.DamageUp();
                }
                break;

            case SupportEffect.DefenseUp:
                if (!(targetPlayer.defUp > 0))
                {
                    targetPlayer.defUp = EffectFlag.Start;
                    targetPlayer.DefenseUp();
                }
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
                if (!(targetPlayer.boost > 0))
                {
                    targetPlayer.boost = EffectFlag.Start;
                    targetPlayer.Boost();
                }
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

            case SupportEffect.DamageUp:
                if (!(targetPlayer.dmgUp > 0))
                {
                    targetPlayer.dmgUp = EffectFlag.Start;
                    targetPlayer.DamageUp();
                }
                break;

            case SupportEffect.DefenseUp:
                if (!(targetPlayer.defUp > 0))
                {
                    targetPlayer.defUp = EffectFlag.Start;
                    targetPlayer.DefenseUp();
                }
                break;

            default:
                break;
        }
        Debug.Log("Support Card Played with " + primaryEffect + " and " + secondaryEffect);
    }
}
