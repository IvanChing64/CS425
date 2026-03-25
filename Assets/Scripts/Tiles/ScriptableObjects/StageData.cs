using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewStageData", menuName = "ScriptableObjects/StageData")]
public class StageData : ScriptableObject
{
    [Header("Visuals")]
    public Tile grassTilePrefab;
    public Tile mountainTilePrefab;

    [Header("Enemy Spawning")]
    public List<GameObject> enemyPrefabs;
    public int enemyCount = 3;

}
