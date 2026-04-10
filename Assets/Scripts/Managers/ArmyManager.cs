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

    public void AddUnitToArmy(ScriptableUnit unit) => unitsInArmy.Add(unit);
    public void AddUnitToArmy(string unitName) => AddUnitToArmy(AllPlayerUnits[unitName]);
    
    public bool HasUnitInArmy(ScriptableUnit unit) => unitsInArmy.Contains(unit);
    public bool HasUnitInArmy(string unitName) => HasUnitInArmy(AllPlayerUnits[unitName]);

    public bool RemoveUnitFromArmy(ScriptableUnit unit) => unitsInArmy.Remove(unit);
    public bool RemoveUnitFromArmy(string unitName) => RemoveUnitFromArmy(AllPlayerUnits[unitName]);

    /// <remarks><em>Only generated if ArmyManager instance does not already have units</em></remarks>
    public void GenerateStartingArmy()
    {
        AddUnitToArmy("Knight");
        AddUnitToArmy("Archer");
        AddUnitToArmy("Mage");
        AddUnitToArmy("Cleric");
    }

}
