using System.Collections.Generic;
using UnityEngine;

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
        AddUnit("Knight");
        AddUnit("Archer");
        AddUnit("Mage");
        AddUnit("Cleric");
    }

}
