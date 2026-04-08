using UnityEngine;
using UnityEngine.UI;

//Developer: Ivan Ching
//Aggregated from multiple sources

public class combatUIManager : MonoBehaviour
{
    //temp usage until card functionality is implemented.
    public static combatUIManager Instance;

    [SerializeField] private AudioClip[] damageSoundClips;
    [SerializeField] private GameObject combatPanel, endTurnPanel;
    [SerializeField] private Button combatButton, endTurnButton;
    [SerializeField] private GameObject blocker;

    private BasePlayer targetPlayer;
    private BaseEnemy targetEnemy;
    Animator playerAnim;
    public bool IsCombatMenuOpen => combatPanel.activeSelf;
    public bool IsEndTurnMenuOpen => endTurnPanel.activeSelf;

    private void Awake()
    {
        Instance = this;
        combatPanel.SetActive(false);
        endTurnPanel.SetActive(false);
    }

    //gets player and enemy data and prompts attack button
    public void showCombatOption(BasePlayer player, BaseEnemy enemy)
    {
        if (player.canAttack == false) return;
        Debug.Log("showing combat menu");
        targetPlayer = player;
        targetEnemy = enemy;

        combatPanel.SetActive(true);
    }
    
    //hides button when not in use
    public void hideCombatOption()
    {
        combatPanel.SetActive(false);
    }

    //damage step
    public void ExecuteCombat()
    {
        if (targetEnemy != null)
        {
            Animator playerAnim = targetPlayer.GetComponent<Animator>();
            if(playerAnim != null)
            {
                playerAnim.SetTrigger("attack");
            }
            targetEnemy.takeDamage(targetPlayer.dmg * targetPlayer.attackModifier);
            targetPlayer.canAttack = false;
            targetPlayer.dmg = 0;
            foreach (Tile t in GridManager.Instance.GetNeighborsOf(targetPlayer.OccupiedTile))
            {
                if (t.isWalkable)t.ShowHighlight(false, Tile.nonwalkableColor);
            }
        }
        hideCombatOption();
        CardManager.instance.PlaySelectedCard();
        
    }

    //ADDED FOR CLICK ON TILE COMBAT
    public void Attack(BasePlayer attacker, BaseUnit defender)
    {
        Animator playerAnim = attacker.GetComponent<Animator>();
        if (defender != null && attacker != null)
        {
            attacker.GetComponent<Animator>();
            if (playerAnim != null)
            {
                playerAnim.SetTrigger("attack");
            }
            defender.takeDamage(attacker.dmg * attacker.attackModifier);
            attacker.canAttack = false;
            attacker.dmg = 0;
            foreach (Tile t in GridManager.Instance.GetNeighborsOf(attacker.OccupiedTile))
            {
                if (t.isWalkable)t.ShowHighlight(false, Tile.nonwalkableColor);
            }
        }
    }

    //End Turn button
    public void ShowEndTurnOption()
    {
        endTurnPanel.SetActive(true);
    }

    public void hideEndTurnOption()
    {
        endTurnPanel.SetActive(false);
    }


    //end turn functionality
    public void ExecuteEndTurn()
    {
        hideEndTurnOption();
        hideCombatOption();
        
        CardManager.instance.DeselectCard();
        UnitManager.Instance.SetSelectedPlayer(null);

        if (CardManager.instance.selectedPlayer != null)
        {
            CardManager.instance.selectedPlayer.GetTilesInMoveRange().ForEach(t => t.ShowHighlight(false, Tile.nonwalkableColor));
            GridManager.Instance.GetNeighborsOf(CardManager.instance.selectedPlayer.OccupiedTile).ForEach(t => t.ShowHighlight(false, Tile.nonwalkableColor));
            CardManager.instance.selectedPlayer.OccupiedTile.ShowHighlight(false, Tile.nonwalkableColor);
        }
        GameManager.Instance.ChangeState(GameState.EnemyTurn);
    }

    public void ToggleBlocker(bool isActive)
    {
        blocker.SetActive(isActive);
    }
}
