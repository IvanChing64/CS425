using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Multiplayer.Center.Common;
using UnityEngine;
using UnityEngine.EventSystems;

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

    public static Color walkableColor = new Color(44f/255f, 153f/255f, 1, 120f/255f);
    public static Color nonwalkableColor = new Color(0, 0, 0, 0);
    public static Color targetableColor = new Color(1, 1, 1, 120f/255f);
    public static Color attackableColor = new Color(1, 18f/255f, 0, 159f/255f);
    public static Color supportableColor = new Color(3f/255f, 1, 0, 100f/255f);
    public static Color summonableColor = new Color(167f/255f, 65f/255f, 1, 155f/255f);

    public virtual void Init(int x, int y)
    {
        
    }

    //Player movement testing
    private void OnMouseDown()
    {
        //Checks if the combat menu is open on screen
        //if (combatUIManager.Instance != null && combatUIManager.Instance.IsCombatMenuOpen) return;

        //Check if it is the player's turn and no units are currently moving
        if (GameManager.Instance.gameState != GameState.PlayerTurn) return;
        if (GameManager.Instance.unitMoving == true) return;

        if (EventSystem.current.IsPointerOverGameObject()) return;

        // If a unit and card are selected and this tile is in effect range, attempt to apply card effect
        if (UnitManager.Instance.SelectedPlayer != null && CardManager.instance.selectedCard != null && highlight.activeInHierarchy)
        {
            BasePlayer player = UnitManager.Instance.SelectedPlayer;
            BaseCard usedCard = CardManager.instance.selectedCard;

            // If the card applies a support effect and an ally is in range, apply it
            if (player.canSupport && OccupiedUnit != null && OccupiedUnit.Faction == Faction.Player)
            {
                Animator playerAnim = UnitManager.Instance.SelectedPlayer.GetComponent<Animator>();
                UnitManager.Instance.SelectedPlayer.GetComponent<Animator>();
                if (playerAnim != null)
                {
                    playerAnim.SetTrigger("support");
                }

                if (usedCard.AoE == AreaOfEffectType.None)
                {
                    ((BaseSupportCard)CardManager.instance.selectedCard).ApplySupportEffect(OccupiedUnit);
                } else if (usedCard.AoE == AreaOfEffectType.SupportInclusive)
                {
                    foreach (Tile t in player.GetTilesInAttackRange())
                    {
                        if (t.OccupiedUnit != null && t.OccupiedUnit.Faction == Faction.Player)
                        {
                            ((BaseSupportCard)CardManager.instance.selectedCard).ApplySupportEffect(t.OccupiedUnit);
                        }
                    }
                } else if (usedCard.AoE == AreaOfEffectType.SupportExclusive)
                {
                    foreach (Tile t in UnitManager.Instance.SelectedPlayer.GetTilesInAttackRange())
                    {
                        if (t.OccupiedUnit != null && t.OccupiedUnit != player && t.OccupiedUnit.Faction == Faction.Player)
                        {
                            ((BaseSupportCard)CardManager.instance.selectedCard).ApplySupportEffect(t.OccupiedUnit);
                        }
                    }
                }
                
                UnitInfo.Instance.UpdatePanel();
                CardManager.instance.PlaySelectedCard();
                return;
            }

            // If the card is an attack card and an enemy is in range, 
            if (player.canAttack && OccupiedUnit != null && OccupiedUnit.Faction == Faction.Enemy)
            {
                if (usedCard.AoE != AreaOfEffectType.AttackRangedCenter)
                {
                    if (usedCard.AoE == AreaOfEffectType.None)
                    {
                        // Attack the enemy
                        combatUIManager.Instance.Attack(UnitManager.Instance.SelectedPlayer, OccupiedUnit);

                        // Apply control effects
                        ((BaseAttackCard)CardManager.instance.selectedCard).ApplyControlEffect(OccupiedUnit);
                    } else if (usedCard.AoE == AreaOfEffectType.AttackSelfCenter)
                    {
                        foreach (Tile t in UnitManager.Instance.SelectedPlayer.GetTilesInAttackRange())
                        {
                            if (t.OccupiedUnit != null && t.OccupiedUnit.Faction == Faction.Enemy)
                            {
                                // Attack the enemy
                                combatUIManager.Instance.Attack(UnitManager.Instance.SelectedPlayer, t.OccupiedUnit, true);

                                // Apply control effects
                                ((BaseAttackCard)CardManager.instance.selectedCard).ApplyControlEffect(t.OccupiedUnit);
                            }   
                        }

                        UnitManager.Instance.SelectedPlayer.canAttack = false;
                        UnitManager.Instance.SelectedPlayer.dmg = 0;
                        foreach (Tile t in GridManager.Instance.GetNeighborsOf(UnitManager.Instance.SelectedPlayer.OccupiedTile))
                        {
                            if (t.isWalkable)t.ShowHighlight(false, Tile.nonwalkableColor);
                        }
                    }

                    // Remove invisibility if player attacks
                    if (UnitManager.Instance.SelectedPlayer.invisible > 0 && ((BaseAttackCard)usedCard).damaging) UnitManager.Instance.SelectedPlayer.Invisible(true);
                    
                    CardManager.instance.PlaySelectedCard();
                    return;
                }
                
            }

            if (player.canAttack && usedCard.AoE == AreaOfEffectType.AttackRangedCenter)
            {
                bool attacked = false;
                bool attackable = false;
                foreach (Tile t in UnitManager.Instance.SelectedPlayer.GetTilesInAttackRange())
                {
                    if (this == t)
                    {
                        attackable = true;
                        break;
                    }
                }
                if (attackable)
                {
                    if (OccupiedUnit != null && OccupiedUnit.Faction == Faction.Enemy && highlight.activeInHierarchy)
                    {
                        attacked = true;
                        // Attack the enemy
                        combatUIManager.Instance.Attack(UnitManager.Instance.SelectedPlayer, OccupiedUnit, true);

                        // Apply control effects
                        ((BaseAttackCard)CardManager.instance.selectedCard).ApplyControlEffect(OccupiedUnit);
                    }

                    foreach (Tile t in UnitManager.Instance.SelectedPlayer.GetTilesInAOEAttackRange(this, usedCard.areaRange))
                    {
                    if (t.OccupiedUnit != null && t.OccupiedUnit.Faction == Faction.Enemy && highlight.activeInHierarchy)
                        {
                            attacked = true;
                            // Attack the enemy
                            combatUIManager.Instance.Attack(UnitManager.Instance.SelectedPlayer, t.OccupiedUnit, true);

                            // Apply control effects
                            ((BaseAttackCard)CardManager.instance.selectedCard).ApplyControlEffect(t.OccupiedUnit);
                        } 
                    }

                    if (attacked)
                    {
                        UnitManager.Instance.SelectedPlayer.canAttack = false;
                        UnitManager.Instance.SelectedPlayer.dmg = 0;
                        foreach (Tile t in GridManager.Instance.GetNeighborsOf(UnitManager.Instance.SelectedPlayer.OccupiedTile))
                        {
                            if (t.isWalkable)t.ShowHighlight(false, Tile.nonwalkableColor);
                        }

                        // Remove invisibility if player attacks
                        if (UnitManager.Instance.SelectedPlayer.invisible > 0 && ((BaseAttackCard)usedCard).damaging) UnitManager.Instance.SelectedPlayer.Invisible(true);
                        
                        CardManager.instance.PlaySelectedCard();
                        return;
                    }
                }                
            }

            //If the card is a summon card, and tile is not occupied
            if (UnitManager.Instance.SelectedPlayer.canSummon && OccupiedUnit == null)
            {
                BasePlayer summonUnit = ((BaseSummonCard)CardManager.instance.selectedCard).SummonUnit(this);
                ShowHighlight(false, Tile.nonwalkableColor);
                CardManager.instance.PlaySelectedCard();
                summonUnit.GetComponent<HandManager>().NextTurn();
                UnitManager.Instance.SetSelectedPlayer(summonUnit);
                return;
            }

            // If the card is a movement card, move to the tile
            /* TODO: This part is still gross, some of this functionality should be extracted */
            List<Tile> tilesInRange = player.GetTilesInMoveRange();
            if (tilesInRange.Contains(this))
            {
                //BasePlayer playerPath = UnitManager.Instance.SelectedPlayer;
                List<Tile> path = AStarManager.Instance.GeneratePath(player.OccupiedTile, this);
                UnitManager.Instance.SelectedPlayer.OccupiedTile.highlight.SetActive(false);
                UnitManager.Instance.selector.Hide();
                combatUIManager.Instance.ToggleBlocker(true);
                CardManager.instance.PlaySelectedCard();
                CardManager.instance.ToggleCardArea(false);
                if (path != null && path.Count > 0)
                {
                    StartCoroutine(MoveUnitPath(player, path));
                }

                foreach (Tile t in tilesInRange) t.highlight.SetActive(false);
                return;
            }
        }

        // If there is a unit occupying this tile, select it
        if (OccupiedUnit != null)
        {
            // If the unit is a player, select it as if it's a player
            if (OccupiedUnit.Faction == Faction.Player)
            {
                UnitManager.Instance.SetSelectedPlayer((BasePlayer)OccupiedUnit);
            }

            // Otherwise, select it as if it's an enemy
            else
            {
                UnitManager.Instance.SetSelectedEnemy((BaseEnemy)OccupiedUnit);
            }

            return;
        }

        /* OLD CODE
        
        //If there is something on the tile selected
        if(OccupiedUnit != null)
        {
            //Selecting players or Supports PLayer
            if (OccupiedUnit.Faction == Faction.Player)
            {
                // Supports PLayer
                if (UnitManager.Instance.SelectedPlayer != null && UnitManager.Instance.SelectedPlayer.canSupport)
                {
                    if (OccupiedUnit != null && OccupiedUnit.Faction == Faction.Player)
                    {
                        if (highlight.activeInHierarchy)
                        {
                            BaseSupportCard tempSupport = (BaseSupportCard)CardManager.instance.selectedCard;
                            tempSupport.ApplySupportEffect(OccupiedUnit);
                            CardManager.instance.PlaySelectedCard();
                            return;
                        }
                    }
                }

                //Selects players
                UnitManager.Instance.SetSelectedPlayer((BasePlayer)OccupiedUnit);
                CardManager.instance.SetSelectedPlayer((BasePlayer)OccupiedUnit);
                //code to check if selected player is already next to an enemy
            }
            else if (UnitManager.Instance.SelectedPlayer != null) { // If not selecting a player unit then it selects a enemy
                Debug.Log("Cannot move here. Enemy Space.");
                //return;
            }

            // Attacks enemy
            if (UnitManager.Instance.SelectedPlayer.canAttack)
            {
                if (OccupiedUnit != null && OccupiedUnit.Faction == Faction.Enemy)
                {
                    if (highlight.activeInHierarchy)
                    {
                        combatUIManager.Instance.Attack(UnitManager.Instance.SelectedPlayer, OccupiedUnit);
                    }
                }
            }

        }
        // If a player is selected
        else if (UnitManager.Instance.SelectedPlayer != null)
        {
            // Simple implementation for getting tiles in range of a player
            List<Tile> tilesInRange = UnitManager.Instance.SelectedPlayer.GetTilesInMoveRange();

            //Complex implementation to allow path reconstruction
            //Dictionary<Tile, Tile> tilePathsInRange = RangeManager.GetPathsInRange(
            //    UnitManager.Instance.SelectedPlayer.OccupiedTile, UnitManager.Instance.SelectedPlayer.moveRange, RangeType.FloodMovement);
            //List<Tile> movementPath = RangeManager.ReconstructPath(this, tilePathsInRange);
            

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
                        //combatUIManager.Instance.showCombatOption(player, enemy);
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
            //setUnit(UnitManager.Instance.SelectedPlayer);
            BasePlayer playerPath = UnitManager.Instance.SelectedPlayer;
            List<Tile> path = AStarManager.Instance.GeneratePath(playerPath.OccupiedTile, this);
            UnitManager.Instance.SelectedPlayer.OccupiedTile.highlight.SetActive(false);
            combatUIManager.Instance.ToggleBlocker(true);
            CardManager.instance.PlaySelectedCard();
            CardManager.instance.ToggleCardArea(false);
            if(path != null && path.Count > 0)
            {
                StartCoroutine(MoveUnitPath(playerPath, path));
            }

            //UnitManager.Instance.SetSelectedPlayer(null);
            //GameManager.Instance.ChangeState(GameState.EnemyTurn);
            //Uncomment to regain control of player
            //GameManager.Instance.ChangeState(GameState.PlayerTurn);
            foreach (Tile t in tilesInRange) t.highlight.SetActive(false);
        }

        */
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
        if(unit == null)
        {
            OccupiedUnit = null;
            return;
        }

        if (unit.OccupiedTile != null) unit.OccupiedTile.OccupiedUnit = null;
        unit.transform.position = transform.position;
        OccupiedUnit = unit;
        unit.OccupiedTile = this;
        if (unit.Faction == Faction.Player)
        {
            unit.moveRange = 0; //Reset move range after moving
        }
        
    }

    private IEnumerator MoveUnitPath(BaseUnit unit, List<Tile> path)
    {
        GameManager.Instance.unitMoving = true;
        Animator UnitAnimator = unit.GetComponent<Animator>();
        if(UnitAnimator != null)
        {
            UnitAnimator.SetBool("isMoving", true);
        }
        if (unit.OccupiedTile != null)
        {
            unit.OccupiedTile.OccupiedUnit = null;
            unit.OccupiedTile = null;
        }
            
        foreach(Tile tile in path)
        {
            Vector3 startPos = unit.transform.position;
            Vector3 endPos = tile.transform.position;
            float travelTime = 0.2f;
            float elapsedTime = 0;

            while(elapsedTime < travelTime)
            {
                unit.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / travelTime);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            unit.transform.position = endPos;
        }
        if (UnitAnimator != null)
        {
            UnitAnimator.SetBool("isMoving", false);
        }

        combatUIManager.Instance.ToggleBlocker(false);
        CardManager.instance.ToggleCardArea(true);
        UnitManager.Instance.selector.PlaceOnUnit(UnitManager.Instance.SelectedUnit);

        OccupiedUnit = unit;
        unit.OccupiedTile = this;
        if (unit.Faction == Faction.Player)
        {
            unit.moveRange = 0;
        }
        
        GameManager.Instance.unitMoving = false;
    }

    public void ShowHighlight(bool state, Color color)
    {
        if(highlight != null)
        {
            highlight.SetActive(state);
            highlight.GetComponent<SpriteRenderer>().color = color;
        }
    }
}