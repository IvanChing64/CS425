using Unity.VisualScripting;
using UnityEngine;

//Developer: Bailey Escritor
//Derived class for attack cards
public class BaseAttackCard : BaseCard
{

    public override void SelectCard()
    {
        //Highlight Selectable Tiles and Targets
        BasePlayer player = CardManager.instance.selectedPlayer;

        if(player != null)
        {
            player.canAttack = true;
            player.dmg = value;
            Tile currentTile = player.OccupiedTile;

            foreach (Tile t in player.GetTilesInAttackRange())
            {
                t.ShowHighlight(true, Tile.attackableColor);
            }
            currentTile.ShowHighlight(false, Tile.nonwalkableColor);

            // if(currentTile != null && currentTile.IsNextToEnemy())
            // {
            //     Debug.Log("Player is next to an enemy!");
            //     var neighbors = GridManager.Instance.GetNeighborsOf(currentTile);

            //     foreach (var n in neighbors)
            //     {
            //         if (n.OccupiedUnit != null && n.OccupiedUnit.Faction == Faction.Enemy)
            //         {
            //             BaseEnemy enemy = (BaseEnemy)n.OccupiedUnit;
            //             combatUIManager.Instance.showCombatOption(player, enemy);
            //             break;
            //         }
            //     }
            // }
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
            Tile currentTile = player.OccupiedTile;
            foreach (Tile t in GridManager.Instance.GetNeighborsOf(currentTile))
            {
                if (t.isWalkable)t.ShowHighlight(false, Tile.nonwalkableColor);
            }
            combatUIManager.Instance.hideCombatOption();
        }
    }
}
