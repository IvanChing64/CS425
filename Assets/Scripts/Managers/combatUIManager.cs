using UnityEngine;
using UnityEngine.UI;

//Developer: Ivan Ching
//Aggregated from multiple sources

public class combatUIManager : MonoBehaviour
{
    //temp usage until card functionality is implemented.
    public static combatUIManager Instance;

    [SerializeField] private GameObject combatPanel;
    [SerializeField] private Button combatButton;

    private BasePlayer targetPlayer;
    private BaseEnemy targetEnemy;
    public bool IsCombatMenuOpen => combatPanel.activeSelf;

    private void Awake()
    {
        Instance = this;
        combatPanel.SetActive(false);
    }

    //gets player and enemy data and prompts attack button
    public void showCombatOption(BasePlayer player, BaseEnemy enemy)
    {
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

        if (targetEnemy != null)
        {
            targetEnemy.takeDamage(targetPlayer.dmg);
        }
        hideCombatOption();
        
    }

}
