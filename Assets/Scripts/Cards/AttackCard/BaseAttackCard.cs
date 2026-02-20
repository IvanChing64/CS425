using Unity.VisualScripting;
using UnityEngine;

//Developer: Bailey Escritor
//Derived class for attack cards
public class BaseAttackCard : BaseCard
{
    //public int attack;

    //Sets unit's dmg value and allows attack action
    public override void PlayCard()
    {
        BasePlayer player = CardManager.instance.selectedPlayer;
        if(player != null)
        {
            player.canAttack = true;
            player.dmg = value;
            Tile currentTile = player.OccupiedTile;
            foreach (Tile t in GridManager.Instance.GetNeighborsOf(currentTile))
            {
                if (t.isWalkable)t.ShowHighlight(true, Tile.attackableColor);
            }
            if(currentTile != null && currentTile.IsNextToEnemy())
            {
                Debug.Log("Player is next to an enemy!");
                        var neighbors = GridManager.Instance.GetNeighborsOf(currentTile);

                        foreach (var n in neighbors)
                        {
                            if (n.OccupiedUnit != null && n.OccupiedUnit.Faction == Faction.Enemy)
                            {
                                BaseEnemy enemy = (BaseEnemy)n.OccupiedUnit;
                                combatUIManager.Instance.showCombatOption(player, enemy);
                                break;
                            }
                        }
            }
        }
        //CardManager.instance.selectedPlayer.canAttack = true;
        //CardManager.instance.selectedPlayer.dmg = value;
        isPlayed = true;    
        Debug.Log("Attack Card Played with attack value: " + value);


    }
}
