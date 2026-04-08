using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for handling the player's army across levels
/// </summary>
/// <remarks>Primary author: Liam Riel</remarks>
public class ArmyManager : MonoBehaviour
{
    public static ArmyManager Instance;

    public List<ScriptableUnit> UnitsInArmy { get; private set; }
    public int currency;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddUnitToArmy(ScriptableUnit unit) => UnitsInArmy.Add(unit);
    
    public bool HasUnitInArmy(ScriptableUnit unit) => UnitsInArmy.Contains(unit);

    public bool RemoveUnitFromArmy(ScriptableUnit unit) => UnitsInArmy.Remove(unit);

    public void RandomizeStartingArmy()
    {
        const int size = 3;

        
    }

}
