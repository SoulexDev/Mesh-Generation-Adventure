using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NoiseAttributes", menuName = "Noise Options/Noise Preset")]
[System.Serializable]
public class NoiseTypesOptions : ScriptableObject
{
    public bool mountains;
    public bool hills;
    public bool cliffs;
    public bool islands;
    public bool spores;
    public bool ridges;
    //Mountains
    [Header("Mountains")]
    public int mountainOctaves = 5;
    public float mountainLacunarity = 1.8f;
    public float mountainFrequency = 0.1f;
    public float mountainFractalGain = 0.4f;
    //Hills
    [Header("Hills")]
    public int hillOctaves = 9;
    public float hillLacunarity = 1.6f;
    public float hillFrequency = 0.4f;
    //Cliffs
    [Header("Cliffs")]
    public float cliffFrequency = 0.02f;
    //Islands
    [Header("Islands")]
    public int islandsOctaves = 5;
    public float islandsLacunarity = 1.6f;
    public float islandsFractalGain = 0.2f;
    public float islandsWeightedStrength = 0.1f;
    //Spores
    [Header("Spores")]
    public int sporesOctaves = 5;
    public float sporesLacunarity = 2;
    public float sporesFrequency = 0.04f;
    public float sporesFractalGain = 0.1f;
    public float sporesWeightedStrength = 1;
    //Ridges
    [Header("Ridges")]
    public int ridgesOctaves = 5;
    public float ridgesLacunarity = 2;
    public float ridgesFrequency = 0.04f;
    public float ridgesFractalGain = 0.3f;
    public float ridgesWeightedStrength = 0;
}