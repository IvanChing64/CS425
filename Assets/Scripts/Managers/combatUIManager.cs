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
        combatButton.onClick.RemoveAllListeners();
        combatButton.onClick.AddListener(() => ExecuteCombat());
    }
    
    //hides button when not in use
    public void hideCombatOption()
    {
        combatPanel.SetActive(false);
    }

    //damage step
    public void ExecuteCombat()
    {
        //SoundFXManager.instance.PlaySoundFXClip(damageSoundClips, transform, 1f);
        if (targetEnemy != null)
        {
            targetEnemy.takeDamage(targetPlayer.dmg);
            targetPlayer.canAttack = false;
            targetPlayer.dmg = 0;
            foreach (Tile t in GridManager.Instance.GetNeighborsOf(targetPlayer.OccupiedTile))
            {
                if (t.isWalkable)t.ShowHighlight(false, Tile.nonWalkableColor);
            }
        }
        hideCombatOption();
        
    }

    public void ShowEndTurnOption()
    {
        endTurnPanel.SetActive(true);
        endTurnButton.onClick.RemoveAllListeners();
        endTurnButton.onClick.AddListener(() => ExecuteEndTurn());
    }

    public void hideEndTurnOption()
    {
        endTurnPanel.SetActive(false);
    }

    public void ExecuteEndTurn()
    {
        hideEndTurnOption();
        hideCombatOption();
        
        CardManager.instance.DeselectCard();
        if (CardManager.instance.selectedPlayer != null)
        {
            CardManager.instance.selectedPlayer.GetTilesInMoveRange().ForEach(t => t.ShowHighlight(false, Tile.nonWalkableColor));
            GridManager.Instance.GetNeighborsOf(CardManager.instance.selectedPlayer.OccupiedTile).ForEach(t => t.ShowHighlight(false, Tile.nonWalkableColor));
            CardManager.instance.selectedPlayer.OccupiedTile.ShowHighlight(false, Tile.nonWalkableColor);
        }
        GameManager.Instance.ChangeState(GameState.EnemyTurn);
    }

    public void ToggleBlocker(bool isActive)
    {
        blocker.SetActive(isActive);
    }

}
