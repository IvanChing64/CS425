using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Developer: Ivan Ching
//Aggregated from multiple tutorials
public abstract class Tile : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] private GameObject highlight;
    [SerializeField] public bool isWalkable;


    public BaseUnit OccupiedUnit;
    public bool Walkable => isWalkable && OccupiedUnit == null;
    public Vector2 Position => this.transform.position;
    public List<Tile> Neighbors => GridManager.Instance.GetNeighborsOf(this);


    public virtual void Init(int x, int y)
    {
        
    }

    //Hover Highlight code
    // void OnMouseEnter()
    // {
    //     highlight.SetActive(true);
    // }

    // void OnMouseExit()
    // {
    //     highlight.SetActive(false);
    // }

    //Player movement testing
    private void OnMouseDown()
    {
        //Checks if the combat menu is open on screen
        if (combatUIManager.Instance != null && combatUIManager.Instance.IsCombatMenuOpen) return;
        //Checks if it is player's turn
        if (GameManager.Instance.gameState != GameState.PlayerTurn) return;

        //If there is something on the tile selected
        if(OccupiedUnit != null)
        {
            //Selecting players
            if (OccupiedUnit.Faction == Faction.Player)
            {
                //Selects players
                UnitManager.Instance.SetSelectedPlayer((BasePlayer)OccupiedUnit);
                //code to check if selected player is already next to an enemy
                if (IsNextToEnemy())
                {
                        Debug.Log("Player is next to an enemy!");
                        var neighbors = GridManager.Instance.GetNeighborsOf(this);

                        foreach (var n in neighbors)
                        {
                            if (n.OccupiedUnit != null && n.OccupiedUnit.Faction == Faction.Enemy)
                            {
                                BaseEnemy enemy = (BaseEnemy)n.OccupiedUnit;
                                BasePlayer player = UnitManager.Instance.SelectedPlayer;
                                combatUIManager.Instance.showCombatOption(player, enemy);
                                break;
                            }
                        }
                }
                else
                {
                    // Highlight player movement range
                    List<Tile> tilesInRange = UnitManager.Instance.SelectedPlayer.GetTilesInMoveRange();
                    foreach (Tile t in tilesInRange) t.highlight.SetActive(true);
                }
            }
            else if (UnitManager.Instance.SelectedPlayer != null) { // If not selecting a player unit then it selects a enemy
                Debug.Log("Cannot move here. Enemy Space.");
                return;
            }

        }
        // If a player is selected
        else if (UnitManager.Instance.SelectedPlayer != null)
        {
            // Simple implementation for getting tiles in range of a player
            List<Tile> tilesInRange = UnitManager.Instance.SelectedPlayer.GetTilesInMoveRange();

            /* Complex implementation to allow path reconstruction
            Dictionary<Tile, Tile> tilePathsInRange = MovementManager.Instance.GetPathsInRange(
                UnitManager.Instance.SelectedPlayer.OccupiedTile, UnitManager.Instance.SelectedPlayer.moveRange);
            List<Tile> movementPath = MovementManager.Instance.ReconstructPath(this, tilePathsInRange);
            */

            // If this tile is NOT in the selected unit's movement range, deny movement
            if (!tilesInRange.Contains(this))
            {
                Debug.Log("Cannot move here.");
                return;
            }

            // When moving to tile, if an enemy is found near the player display the attack prompt
            if (IsNextToEnemy())
            {
                    foreach (Tile t in tilesInRange) t.highlight.SetActive(false);

                    Debug.Log("Player moved next to an enemy!");
                    var neighbors = GridManager.Instance.GetNeighborsOf(this);

                    foreach (var n in neighbors)
                    {
                        if (n.OccupiedUnit != null && n.OccupiedUnit.Faction == Faction.Enemy)
                        {
                            BaseEnemy enemy = (BaseEnemy)n.OccupiedUnit;
                            BasePlayer player = UnitManager.Instance.SelectedPlayer;
                            combatUIManager.Instance.showCombatOption(player, enemy);
                            break;
                        }
                    }

            }
            // If no enemy is found near the player, hide attack prompt
            else
            {
                combatUIManager.Instance.hideCombatOption();
            }

            //places unit there
            setUnit(UnitManager.Instance.SelectedPlayer);
            //UnitManager.Instance.SetSelectedPlayer(null);
            GameManager.Instance.ChangeState(GameState.EnemyTurn);
            //Uncomment to regain control of player
            //GameManager.Instance.ChangeState(GameState.PlayerTurn);
            foreach (Tile t in tilesInRange) t.highlight.SetActive(false);
        }
    }

    //Helper function to scan direction for enemys
    public bool IsNextToEnemy()
    {
        var neighbors = GridManager.Instance.GetNeighborsOf(this);

        foreach (var n in neighbors)
        {
            if (n.OccupiedUnit != null && n.OccupiedUnit.Faction == Faction.Enemy)
            {
                return true;
            }
        }

        return false;
    }

    //General Code for Movement
    public void setUnit(BaseUnit unit)
    {
        if (unit.OccupiedTile != null) unit.OccupiedTile.OccupiedUnit = null;
        unit.transform.position = transform.position;
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
    }
}