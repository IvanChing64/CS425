using UnityEngine;

//Developer: Bailey Escritor
//Aggregated from multiple tutorials
public class BaseMovementCard : BaseCard
{
    //public int movement;

    //Sets unit's moveRange value
    public override void PlayCard()
    {
        CardManager.instance.selectedPlayer.moveRange = value;
        /* Show new movement range on grid after card is played
        //GridManager.Instance.GetTileAtPosition(CardManager.instance.selectedPlayer.OccupiedTile.Position).HighlightMovementRange();
        */
        isPlayed = true;
        Debug.Log("Movement Card Played with movement value: " + value);
    }
}
