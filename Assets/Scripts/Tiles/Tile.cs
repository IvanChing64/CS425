using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] private GameObject highlight;
    [SerializeField] private bool isWalkable;


    public BaseUnit OccupiedUnit;
    public bool Walkable => isWalkable && OccupiedUnit == null;


    public virtual void Init(int x, int y)
    {
        
    }

    //Hover Highlight code
    void OnMouseEnter()
    {
        highlight.SetActive(true);
    }

    void OnMouseExit()
    {
        highlight.SetActive(false);
    }

    //Player movement testing
    private void OnMouseDown()
    {
        if (GameManager.Instance.gameState != GameState.PlayerTurn) return; //Checks if it is player's turn

        //If there is something on the tile selected
        if(OccupiedUnit != null)
        {
            //Selecting players
            if (OccupiedUnit.Faction == Faction.Player)
            {
                UnitManager.Instance.SetSelectedPlayer((BasePlayer)OccupiedUnit);
            }
            else if (UnitManager.Instance.SelectedPlayer != null) { // If not selecting a player unit then it selects a enemy
              Debug.Log("Cannot move here. Enemy Space.");
              return;
                
                //var enemy = (BaseEnemy)OccupiedUnit;
              //Destroy(enemy.gameObject);
              //UnitManager.Instance.SetSelectedPlayer(null);
            }

            } else if (UnitManager.Instance.SelectedPlayer != null) {
            //checks is terrain is walkable
                if (!isWalkable)
                {
                    Debug.Log("Cannot move here.");
                    return;
                }
                //places unit there
                setUnit(UnitManager.Instance.SelectedPlayer);
                if (IsNextToEnemy())
                    {
                        Debug.Log("Player moved next to an enemy!");
                        // You can trigger combat, highlight the enemy, etc.
                    }
                UnitManager.Instance.SetSelectedPlayer(null);
            }
    }

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