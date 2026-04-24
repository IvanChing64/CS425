using Unity.VisualScripting;
using UnityEngine;

//Developer: Bailey Escritor
//Derived class for attack cards
public class BaseAttackCard : BaseCard
{
    public bool damaging, piercing;
    public ControlEffect primaryEffect, secondaryEffect;

    public override void CopyScriptableCard(ScriptableCard card)
    {
        base.CopyScriptableCard(card);
        primaryEffect = card.primaryControlEffect;
        secondaryEffect = card.secondaryControlEffect;
        damaging = card.damaging;
        pierce = card.piercing;
    }

    public override void SelectCard()
    {
        base.SelectCard();

        //Highlight Selectable Tiles and Targets
        BasePlayer player = CardManager.instance.selectedPlayer;

        if(player != null)
        {
            if (!(player.GetComponent<HandManager>().actionPoints < cost))
            {
                player.canAttack = true;
            }
            player.dmg = value;
            player.attackRange = range;
            Tile currentTile = player.OccupiedTile;

            if (AoE == AreaOfEffectType.AttackSelfCenter)
            {
                foreach (Tile t in player.GetTilesInAttackRange())
                {
                    if (t.OccupiedUnit == null || t.OccupiedUnit.Faction == Faction.Enemy)t.ShowHighlight(true, Tile.attackableColor);
                }
                currentTile.ShowHighlight(false, Tile.nonwalkableColor);
                //UnitManager.Instance.targetting = true;
            } else if (AoE == AreaOfEffectType.None || AoE == AreaOfEffectType.AttackRangedCenter)
            {
                foreach (Tile t in player.GetTilesInAttackRange())
                {
                    if (t.OccupiedUnit == null || t.OccupiedUnit.Faction == Faction.Enemy)t.ShowHighlight(true, Tile.targetableColor);
                }
                currentTile.ShowHighlight(false, Tile.nonwalkableColor);
                UnitManager.Instance.targeting = true;
            }
            
        }
    }

    public override void DeselectCard()
    {
        base.DeselectCard();
        
        //Unhighlight Selectable Tiles and Targets
        BasePlayer player = CardManager.instance.selectedPlayer;
        UnitManager.Instance.targeting = false;

        if(player != null)
        {
            player.canAttack = false;
            player.dmg = 0;
            if (AoE == AreaOfEffectType.None || AoE == AreaOfEffectType.AttackSelfCenter)
            {
                foreach (Tile t in player.GetTilesInAttackRange())
                {
                    if (t.isWalkable)t.ShowHighlight(false, Tile.nonwalkableColor);
                }
            } else if (AoE == AreaOfEffectType.AttackRangedCenter)
            {
                foreach (Tile t in player.GetTilesInAttackRange())
                {
                    foreach (Tile p in player.GetTilesInAOEAttackRange(t, areaRange))
                    {
                        if (p.isWalkable)p.ShowHighlight(false, Tile.nonwalkableColor);
                    }
                    if (t.isWalkable)t.ShowHighlight(false, Tile.nonwalkableColor);
                }
            }
            
            player.attackRange = 0;
        }
    }

    public void ApplyControlEffect(BaseUnit targetEnemy)
    {
        switch (primaryEffect)
        {
            case ControlEffect.None:
                break;
            
            case ControlEffect.Daze:
                targetEnemy.Daze();
                break;

            case ControlEffect.Stun:
                targetEnemy.Stun();
                break;

            case ControlEffect.Restrict:
                targetEnemy.Restrict();
                break;

            case ControlEffect.Freeze:
                targetEnemy.Freeze();
                break;

            case ControlEffect.Poison:
                targetEnemy.Poison(value);
                break; 

            case ControlEffect.Flaming:
                targetEnemy.Flaming();
                break;

            case ControlEffect.Weaken:
                targetEnemy.Weaken();
                break;

            case ControlEffect.Vulnerable:
                targetEnemy.Vulnerable();
                break;

            case ControlEffect.Hinder:
                targetEnemy.Hinder();
                break;

            case ControlEffect.Expose:
                targetEnemy.Expose();
                break;
        }

        switch (secondaryEffect)
        {
            case ControlEffect.None:
                break;
            
            case ControlEffect.Daze:
                targetEnemy.Daze();
                break;

            case ControlEffect.Stun:
                targetEnemy.Stun();
                break;

            case ControlEffect.Restrict:
                targetEnemy.Restrict();
                break;

            case ControlEffect.Freeze:
                targetEnemy.Freeze();
                break;
            
            case ControlEffect.Poison:
                targetEnemy.Poison();
                break;
            
            case ControlEffect.Flaming:
                targetEnemy.Flaming();
                break;

            case ControlEffect.Weaken:
                targetEnemy.Weaken();
                break;

            case ControlEffect.Vulnerable:
                targetEnemy.Vulnerable();
                break;

            case ControlEffect.Hinder:
                targetEnemy.Hinder();
                break;

            case ControlEffect.Expose:
                targetEnemy.Expose();
                break;
        }
    }
}
