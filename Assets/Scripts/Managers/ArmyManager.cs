using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/// <summary>
/// Script for handling the player's army across levels
/// </summary>
/// <remarks>Primary author: Liam Riel</remarks>
public class ArmyManager : MonoBehaviour
{
    public static ArmyManager Instance;
    public static Dictionary<string, ScriptableUnit> AllPlayerUnits;

    public List<ScriptableUnit> unitsInArmy;
    [SerializeField] private int currency;

    /// <summary>
    /// The maximum number of units that can be in the army
    /// </summary>
    public int ArmyCapacity { get; private set; } = 6;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        AllPlayerUnits = new Dictionary<string, ScriptableUnit>();

        ScriptableUnit[] loadedUnits = Resources.LoadAll<ScriptableUnit>("Units");
        foreach (ScriptableUnit unit in loadedUnits)
        {
            AllPlayerUnits.Add(unit.name, unit);
        }

        if (unitsInArmy.Count == 0) GenerateStartingArmy();
        
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            GainCurrency(100);
            if (ShopManager.Instance != null)
                ShopManager.Instance.UpdateCurrencyText();
        }
    }

    /// <summary>
    /// Adds a specified unit to the army if the army is under capacity.
    /// </summary>
    /// <param name="unit">The unit to add to the army.</param>
    /// <returns>If the unit was added, false when the army is at or over capacity.</returns>
    public bool AddUnit(ScriptableUnit unit)
    {
        if (unitsInArmy.Count >= ArmyCapacity) return false;
        unitsInArmy.Add(unit);
        return true;
    }

    /// <summary>
    /// Adds a unit by name to the army. The name must be associated with a unit in the AllPlayerUnits dictionary.
    /// </summary>
    /// <param name="unitName">The name of the unit to add.</param>
    /// <returns>If the unit was added, false when the army is at or over capacity.</returns>
    public bool AddUnit(string unitName) => AddUnit(AllPlayerUnits[unitName]);
    
    /// <summary>
    /// Finds if the army has a certain unit.
    /// </summary>
    /// <param name="unit">The unit to find.</param>
    /// <returns>True if the army has the specified unit.</returns>
    public bool HasUnit(ScriptableUnit unit) => unitsInArmy.Contains(unit);

    /// <summary>
    /// Finds if the army has a certain unit. The name must be associated with a unit in the AllPlayerUnits dictionary.
    /// </summary>
    /// <param name="unitName">The name of the unit to find.</param>
    /// <returns>True if the army has the specified unit.</returns>
    public bool HasUnit(string unitName) => HasUnit(AllPlayerUnits[unitName]);

    /// <summary>
    /// Removes a specified unit from the army.
    /// </summary>
    /// <param name="unit">The unit to remove from the army.</param>
    /// <returns>False if the army did not contain the unit in the first place.</returns>
    public bool RemoveUnit(ScriptableUnit unit) => unitsInArmy.Remove(unit);

    /// <summary>
    /// Removes a unit specified by name from the army. The name must be associated with a unit in the AllPlayerUnits dictionary.
    /// </summary>
    /// <param name="unitName">The name of unit to remove from the army.</param>
    /// <returns>False if the army did not contain the unit in the first place.</returns>
    public bool RemoveUnit(string unitName) => RemoveUnit(AllPlayerUnits[unitName]);

    /// <summary>
    /// Gets the player's current gold.
    /// </summary>
    /// <returns>The amount of gold the player has.</returns>
    public int GetCurrency() => currency;

    /// <summary>
    /// Adds gold to the player's currency total, capped at 9999
    /// </summary>
    /// <param name="amount">The amount of gold to add to the player's.</param>
    public void GainCurrency(int amount)
    {
        currency += amount;
        if (currency > 9999)
            currency = 999;
    }

    /// <summary>
    /// Attempts to spend a certain gold amount. No gold is lost if the player's current gold is less than the purchase amount.
    /// </summary>
    /// <param name="purchaseAmount">The gold cost of the purchase to make.</param>
    /// <returns>False if the player did not have enough gold to make the purchase, true if the purchase was made.</returns>
    public bool AttemptPurchase(int purchaseAmount)
    {
        if (purchaseAmount > currency)
            return false;
        currency -= purchaseAmount;
        return true;
    }

    /// <summary>
    /// Clears the player's army and sets their gold to 0.
    /// </summary>
    public void ResetArmy()
    {
        unitsInArmy.Clear();
        currency = 0;
        GenerateStartingArmy();
    }

    /// <summary>
    /// Adds three random base units to the player's army. One is always melee, one is always support, and one can be ranged or glass cannon.
    /// </summary>
    /// <remarks><em>Only called if the ArmyManager instance does not already have units, or when resetting the army.</em></remarks>
    private void GenerateStartingArmy()
    {
        
        // Add the team's melee unit
        int random = Random.Range(0, 3);
        switch (random)
        {
            case 0: AddUnit("Knight"); break;
            case 1: AddUnit("Pioneer"); break;
            case 2: AddUnit("Robot"); break;
            default:
                Debug.Log("Army generation out of range");
                AddUnit("Knight");
                break;
        }

        // Add the team's ranged or glass cannon unit
        random = Random.Range(0, 6);
        switch (random)
        {
            case 0: AddUnit("Archer"); break;
            case 1: AddUnit("Cowboy"); break;
            case 2: AddUnit("Soldier"); break;
            case 3: AddUnit("Mage"); break;
            case 4: AddUnit("Gunner"); break;
            case 5: AddUnit("Spy"); break;
            default:
                Debug.Log("Army generation out of range");
                AddUnit("Archer");
                break;

        }

        // Add the team's support unit
        random = Random.Range(0, 3);
        switch (random)
        {
            case 0: AddUnit("Cleric"); break;
            case 1: AddUnit("Huckster"); break;
            case 2: AddUnit("Scientist"); break;
            default:
                Debug.Log("Army generation out of range");
                AddUnit("Cleric");
                break;
        }

    }

}
