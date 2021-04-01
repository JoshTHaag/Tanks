using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] List<Transform> spawnPositions;

    public Dictionary<Transform, Tank> SpawnPositions { get; private set; }

    public void Awake()
    {
        SpawnPositions = new Dictionary<Transform, Tank>(spawnPositions.Count);
        foreach (var pos in spawnPositions)
            SpawnPositions.Add(pos, null);
    }
}
