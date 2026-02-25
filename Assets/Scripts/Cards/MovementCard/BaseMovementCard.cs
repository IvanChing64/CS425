using UnityEngine;
using System.Collections.Generic;

//Developer: Bailey Escritor
//Derived class for movement cards
public class BaseMovementCard : BaseCard
{
    //public int movement;

    //Sets unit's moveRange value
    // public override void PlayCard()
    // {
    //     BasePlayer player = CardManager.instance.selectedPlayer;
    //     /* Show new movement range on grid after card is played
    //     //GridManager.Instance.GetTileAtPosition(CardManager.instance.selectedPlayer.OccupiedTile.Position).HighlightMovementRange();
    //     */

    //     if(player != null)
    //     {
    //         player.moveRange = value;

    //         List<Tile> tilesInRange = player.GetTilesInMoveRange();
    //         foreach(Tile t in tilesInRange)
    //         {
    //             t.ShowHighlight(true, Tile.walkableColor);
    //         }
    //         player.OccupiedTile.ShowHighlight(true, Tile.walkableColor);
    //         isPlayed = true;
    //         Debug.Log("Movement Card Played with movement value: " + value);
    //     }
        
    // }

    public override void SelectCard()
    {
        //Highlight Selectable Tiles
        BasePlayer player = CardManager.instance.selectedPlayer;

        if(player != null)
        {
            player.moveRange = value;

            List<Tile> tilesInRange = player.GetTilesInMoveRange();
            foreach(Tile t in tilesInRange)
            {
                t.ShowHighlight(true, Tile.walkableColor);
            }
            player.OccupiedTile.ShowHighlight(true, Tile.walkableColor);
        }
    }

    public override void DeselectCard()
    {
        //Unhighlight Selectable Tiles
        if (isPlayed == true) return;

        BasePlayer player = CardManager.instance.selectedPlayer;

        if(player != null)
        {
            List<Tile> tilesInRange = player.GetTilesInMoveRange();
            foreach(Tile t in tilesInRange)
            {
                t.ShowHighlight(false, Tile.nonwalkableColor);
            }
            player.OccupiedTile.ShowHighlight(false, Tile.nonwalkableColor);

            player.moveRange = 0;
        }
    }
}
