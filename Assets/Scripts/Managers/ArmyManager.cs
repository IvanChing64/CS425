using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public int currency;

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

    public void AddUnit(ScriptableUnit unit) => unitsInArmy.Add(unit);
    public void AddUnit(string unitName) => AddUnit(AllPlayerUnits[unitName]);
    
    public bool HasUnit(ScriptableUnit unit) => unitsInArmy.Contains(unit);
    public bool HasUnit(string unitName) => HasUnit(AllPlayerUnits[unitName]);

    public bool RemoveUnit(ScriptableUnit unit) => unitsInArmy.Remove(unit);
    public bool RemoveUnit(string unitName) => RemoveUnit(AllPlayerUnits[unitName]);

    /// <remarks><em>Only generated if ArmyManager instance does not already have units</em></remarks>
    public void GenerateStartingArmy()
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

        // Add the team's ranged unit
        random = Random.Range(0, 3);
        switch (random)
        {
            case 0: AddUnit("Archer"); break;
            case 1: AddUnit("Cowboy"); break;
            case 2: AddUnit("Soldier"); break;
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

        // Add the team's glass cannon unit
        random = Random.Range(0, 3);
        switch (random)
        {
            case 0: AddUnit("Mage"); break;
            case 1: AddUnit("Gunner"); break;
            case 2: AddUnit("Spy"); break;
            default:
                Debug.Log("Army generation out of range");
                AddUnit("Mage");
                break;
        }
    }

}
