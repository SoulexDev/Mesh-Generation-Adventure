using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Spawnables", menuName = "Spawnable Objects/Spawnable")]
public class Spawnable : ScriptableObject
{
    public string spawnableStructureID;
    public GameObject prefabObject;
    [Range(0.1f, 1)]
    public float scatterAmount;
    [Range(0.1f, 1)]
    public float minimumScatterYValue;
    [Range(0, 100)]
    public int regionDensity;
}