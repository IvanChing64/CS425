using UnityEngine;
using System.Collections.Generic;

//Developer: Bailey Escritor
//Derived class for movement cards
public class BaseMovementCard : BaseCard
{
    public override void SelectCard()
    {
        base.SelectCard();

        //Highlight Selectable Tiles
        BasePlayer player = CardManager.instance.selectedPlayer;

        if(player != null)
        {
            player.moveRange = range + player.moveModifier;
            
            List<Tile> tilesInRange = null;
            if (rangeType == RangeType.FloodMovementUnrestricted)
            {
                tilesInRange = player.GetTilesInUnrestrictedMoveRange();
            } else
            {
                tilesInRange = player.GetTilesInMoveRange();
            }
            
            foreach (Tile t in tilesInRange)
            {
                if (t.isWalkable)
                {
                    t.ShowHighlight(true, Tile.targetableColor);
                }              
            }
            
            UnitManager.Instance.targeting = true;
            if (player.GetComponent<HandManager>().actionPoints < cost)
            {
                player.moveRange = 0;
            }
        }
    }

    public override void DeselectCard()
    {
        base.DeselectCard();

        //Unhighlight Selectable Tiles
        BasePlayer player = CardManager.instance.selectedPlayer;
        UnitManager.Instance.targeting = false;

        if(player != null)
        {
            player.moveRange = range + player.moveModifier;
            List<Tile> tilesInRange; ;
            if (rangeType == RangeType.FloodMovementUnrestricted)
            {
                tilesInRange = player.GetTilesInUnrestrictedMoveRange();
            } else
            {
                tilesInRange = player.GetTilesInMoveRange();
            }
            foreach(Tile t in tilesInRange)
            {
                t.ShowHighlight(false, Tile.nonwalkableColor);
            }
            player.OccupiedTile.ShowHighlight(false, Tile.nonwalkableColor);

            player.moveRange = 0;
        }
    }
}
