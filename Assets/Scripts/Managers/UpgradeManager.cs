using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance {get; private set;}
    private bool initialized = false;

    Dictionary<int, ScriptableCard> CardUpgrades;
    Dictionary<int, ScriptableUnit> UnitUpgrades;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            initialized = true;
            Debug.Log("UpgradeManager Instance Created & Initialized");

        } else
        {
            Destroy(gameObject);
            Debug.Log("UpgradeManager Instance Created & Initialized");
        }
    } 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!initialized)
        {
            initialized = true;

            Debug.Log("UpgradeManager Initialized");
        } else
        {
            Debug.Log("UpgradeManager already initialized");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum UpgradeType
    {
        Card = 0,
        Unit = 1,
    }
}
