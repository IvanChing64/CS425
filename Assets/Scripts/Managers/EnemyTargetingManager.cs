using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
public class EnemyTargetingManager : MonoBehaviour
{
    public BasePlayer CurrentTarget { get; private set; }

    public void SelectTarget(List<BasePlayer> players) {

        float bestDist = Mathf.Infinity;
        BasePlayer bestPlayer = null;

        foreach (var player in players)
        {
            float dist = Vector2.Distance(transform.position, player.transform.position);
            if (dist < bestDist)
            {
                bestDist = dist;
                bestPlayer = player;
            }
        }

        CurrentTarget = bestPlayer;
    
    }
    
}
