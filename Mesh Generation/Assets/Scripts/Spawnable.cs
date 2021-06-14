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
    public float minimumHeight;
    [Range(0, 100)]
    public int regionDensity;
    [Header("Structure Mask Settings")]
    public int structureMaskOctaves = 3;
    public float structureMaskLacunarity = 2;
    public float structureMaskFrequency = 0.01f;
    public float structureMaskWeightedStrength = 1;
    [Header("Harvestable")]
    public Harvestable harvestable;
}