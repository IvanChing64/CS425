using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewStageData", menuName = "ScriptableObjects/StageData")]
public class StageData : ScriptableObject
{
    [Header("Stage Type")]
    public StageType type = StageType.Battle;

    [Header("Visuals")]
    public Tile grassTilePrefab;
    public Tile mountainTilePrefab;

    [Header("Enemy Spawning")]
    public List<GameObject> enemies;
    public int enemyCount = 3;

    [Header("Stage Reward")]
    public int currency = 0;
}

public enum StageType
{
    Battle,
    Shop,
    Other
}
