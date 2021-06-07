using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureGeneration : MonoBehaviour
{
    public Structure structure;
    private World world;
    private void Start()
    {
        world = FindObjectOfType<World>();
    }
    public void GenerateStructure(Vector3 pos, Transform terrainChunk)
    {
        for (int x = 0; x <= world.terrainSettings.chunkWidth; x++)
        {
            for (int z = 0; z <= world.terrainSettings.chunkWidth; z++)
            {
                float treePos = world.noiseSettings.GetTerrainGenerationFromNoise(new Vector3(x + pos.x, 0, z + pos.z), world.terrainSettings.chunkWidth, 1, world.lockedMountainCurve, world.lockedHillCurve); ;
                if (treePos < structure.scatterAmount && treePos > 0.1f)
                    if (structure.regionDensity == Random.Range(0, 50))
                        Instantiate(structure.prefabObject, new Vector3(x + pos.x, treePos * world.terrainSettings.terrainHeight, z + pos.z), Quaternion.Euler(-90, 0, 0), terrainChunk);
            }
        }
    }
}
