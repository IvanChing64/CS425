using Unity.VisualScripting;
using UnityEngine;

//Developer: Bailey Escritor
//Derived class for attack cards
public class BaseAttackCard : BaseCard
{
    public ControlEffect effect;

    public override void CopyScriptableCard(ScriptableCard card)
    {
        base.CopyScriptableCard(card);
        effect = card.controlEffect;
    }

    public override void SelectCard()
    {
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

            foreach (Tile t in player.GetTilesInAttackRange())
            {
                t.ShowHighlight(true, Tile.attackableColor);
            }
            currentTile.ShowHighlight(false, Tile.nonwalkableColor);
        }
    }

    public override void DeselectCard()
    {
        //Unhighlight Selectable Tiles and Targets
        BasePlayer player = CardManager.instance.selectedPlayer;

        if(player != null)
        {
            player.canAttack = false;
            player.dmg = 0;
            foreach (Tile t in player.GetTilesInAttackRange())
            {
                if (t.isWalkable)t.ShowHighlight(false, Tile.nonwalkableColor);
            }
            player.attackRange = 0;
            //combatUIManager.Instance.hideCombatOption();
        }
    }

    public void ApplyControlEffect(BaseUnit targetEnemy)
    {
        switch (effect)
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
        }
    }
}
