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
    public int mountainOctaves;
    public float mountainLacunarity;
    public float mountainFrequency;
    public float mountainFractalGain;
    public float mountainWeightedStrength;
    //Hills
    [Header("Hills")]
    public int hillOctaves;
    public float hillLacunarity;
    public float hillFrequency;
    public float hillFractalGain;
    public float hillWeightedStrength;
    //Cliffs
    [Header("Cliffs")]
    public int cliffOctaves;
    public float cliffLacunarity;
    public float cliffFrequency;
    public float cliffFractalGain;
    public float cliffWeightedStrength;
    //Islands
    [Header("Islands")]
    public int islandsOctaves;
    public float islandsLacunarity;
    public float islandsFrequency;
    public float islandsFractalGain;
    public float islandsWeightedStrength;
    //Spores
    [Header("Spores")]
    public int sporesOctaves;
    public float sporesLacunarity;
    public float sporesFrequency;
    public float sporesFractalGain;
    public float sporesWeightedStrength;
    //Ridges
    [Header("Ridges")]
    public int ridgesOctaves;
    public float ridgesLacunarity;
    public float ridgesFrequency;
    public float ridgesFractalGain;
    public float ridgesWeightedStrength;
}