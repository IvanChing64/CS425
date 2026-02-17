using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;

public class EnemyTargetingManager : MonoBehaviour
{
    private BaseUnit npcUnit;
    public BasePlayer CurrentTarget { get; private set; }

    private void Awake() {        
        npcUnit = GetComponent<BaseUnit>();
    }

    public void SelectTarget(){

        var players = UnitManager.Instance.PlayerUnits;

        if (players == null || players.Count == 0) {
            CurrentTarget = null;
            return;
        }


        BasePlayer bestPlayer = null;
        float bestDistance = Mathf.Infinity;
        
        foreach (var p in players)
        {
            float dist = Vector2.Distance(transform.position, p.transform.position);

            if (dist < bestDistance)
            {
                bestDistance = dist;
                bestPlayer = p;
            }
        }

        CurrentTarget = bestPlayer;

    }
    
}
