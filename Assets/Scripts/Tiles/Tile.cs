using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] private GameObject highlight;
    [SerializeField] private bool isWalkable;

    public BaseUnit OccupiedUnit;

    public bool Walkable => isWalkable && OccupiedUnit == null;
    public Vector2 Position => new Vector2(this.transform.position.x, this.transform.position.y);
    public List<Tile> Neighbors => GridManager.Instance.GetNeighborsOf(this);

    public virtual void Init(int x, int y)
    {
        
    }

    //Hover Highlight code
    private void OnMouseEnter()
    {
        highlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        highlight.SetActive(false);
    }

    //Player movement testing
    private void OnMouseDown()
    {
        //Checks if the combat menu is open on screen
        if (CombatUIManager.Instance != null && CombatUIManager.Instance.IsCombatMenuOpen) return;
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
                                CombatUIManager.Instance.showCombatOption(player, enemy);
                                break;
                            }
                        }
                }
            }
            else if (UnitManager.Instance.SelectedPlayer != null) { // If not selecting a player unit then it selects a enemy
                Debug.Log("Cannot move here. Enemy Space.");
                return;
            }

        } else if (UnitManager.Instance.SelectedPlayer != null) {
            //checks is terrain is walkable
            if (!isWalkable)
            {
                Debug.Log("Cannot move here.");
                return;
            }
            //when moving to tile, scan for enemy and prompt attack button
            if (IsNextToEnemy())
            {
                    Debug.Log("Player moved next to an enemy!");
                    var neighbors = GridManager.Instance.GetNeighborsOf(this);

                    foreach (var n in neighbors)
                    {
                        if (n.OccupiedUnit != null && n.OccupiedUnit.Faction == Faction.Enemy)
                        {
                            BaseEnemy enemy = (BaseEnemy)n.OccupiedUnit;
                            BasePlayer player = UnitManager.Instance.SelectedPlayer;
                            CombatUIManager.Instance.showCombatOption(player, enemy);
                            break;
                        }
                    }

            }
            //when moved away from enemy, hide attack prompt
            if (!IsNextToEnemy())
            {
                CombatUIManager.Instance.hideCombatOption();
            }

            //places unit there
            setUnit(UnitManager.Instance.SelectedPlayer);
            UnitManager.Instance.SetSelectedPlayer(null);
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

    /// <summary>
    /// <p>Gets all tiles and paths to them within a certain range of a unit</p>
    /// </summary>
    /// <param name="range">The maximum range to search for tiles away from this tile</param>
    /// <returns>A dictionary with all found tiles in range and their preceding tile</returns>
    public Dictionary<Tile, Tile> GetPathsInRange(int range)
    {
        var tilesPaths = new Dictionary<Tile, Tile>();
        var visitedList = new List<Tile>();
        var visitQueue = new Queue<Tile>();

        visitQueue.Enqueue(this);
        
        while (visitQueue.TryDequeue(out Tile tile))
        {
            foreach (Tile neighbor in Neighbors.Where(t => !visitedList.Contains(t) && t.Walkable && this.DistanceTo(t) <= range))
            {
                tilesPaths.Add(neighbor, tile);
                visitQueue.Enqueue(neighbor);
            }
            
            visitedList.Add(tile);
        }

        return tilesPaths;
    }

    /// <summary>
    /// <p>Finds the distance to another tile in terms of the number of tile moves it would take to reach it</p>
    /// </summary>
    /// <param name="other">The tile to find the distance to</param>
    /// <returns>The number of tile moves it would take to reach 'other' from this tile</returns>
    public int DistanceTo(Tile other)
    {
        float thisTotal = this.Position.x + this.Position.y;
        float otherTotal = other.Position.x + other.Position.y;

        return (int)Math.Abs(thisTotal - otherTotal);
    }

}