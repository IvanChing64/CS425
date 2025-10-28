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
        if (GameManager.Instance.gameState != GameState.PlayerTurn) return;

        if(OccupiedUnit != null)
        {
            if (OccupiedUnit.Faction == Faction.Player)
            {
                UnitManager.Instance.SetSelectedPlayer((BasePlayer)OccupiedUnit);
            } else if(UnitManager.Instance.SelectedPlayer != null) {
                var enemy = (BaseEnemy)OccupiedUnit;
                Destroy(enemy.gameObject);
                UnitManager.Instance.SetSelectedPlayer(null);
            }

        } else if (UnitManager.Instance.SelectedPlayer != null)
        {
            setUnit(UnitManager.Instance.SelectedPlayer);
            UnitManager.Instance.SetSelectedPlayer(null);
        }
        {

        }
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