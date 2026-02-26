using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.ComponentModel;

public class EnemyTargetingManager : MonoBehaviour
{
    private BaseUnit npcUnit;
    public BasePlayer CurrentTarget { get; private set; }

    public static Dictionary<BasePlayer, int> TargetCounts = new Dictionary<BasePlayer, int>();

    private void Awake() {        
        npcUnit = GetComponent<BaseUnit>();
    }

    public void SelectTarget(){

        var players = UnitManager.Instance.playersSpawned;

        if (players == null || players.Count == 0) {
            CurrentTarget = null;
            return;
        }


        BasePlayer bestPlayer = null;
        float bestDistance = Mathf.Infinity;

        

        foreach (var p in players)
        {
            if (p == null || p.gameObject == null) continue;

            float dist = Vector2.Distance(transform.position, p.transform.position);

            //Section to ask how many npc's are targetting one unit.

            int load = EnemyTargetingManager.TargetCounts.ContainsKey(p)
                ? EnemyTargetingManager.TargetCounts[p]
                : 0;

            if (dist < bestDistance)
            {
                bestDistance = dist;
                bestPlayer = p;
            }
        }

        CurrentTarget = bestPlayer;

        if (!EnemyTargetingManager.TargetCounts.ContainsKey(bestPlayer))
        {
            EnemyTargetingManager.TargetCounts[bestPlayer] = 0;

            EnemyTargetingManager.TargetCounts[bestPlayer]++;
        }

    }
    
}
