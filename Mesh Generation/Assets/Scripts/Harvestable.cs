using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HarvestableItems", menuName = "Harvestable/Harvestable Item")]
public class Harvestable : ScriptableObject
{
    public float health;
    public GameObject[] droppedItems;
    public ParticleSystem mineParticles;
    public enum HarvestingTool { Rock, AxePick }
    public HarvestingTool harvestingTool;
}
