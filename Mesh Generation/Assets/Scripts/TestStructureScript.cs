using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStructureScript : MonoBehaviour
{
    public Structure structure;
    private World world;
    private void Start()
    {
        world = FindObjectOfType<World>();
    }
    public void GenerateTrees()
    {
        for (int x = 0; x < world.terrainSettings.mapChunkSize * structure.regionDensity; x++)
        {
            for (int z = 0; z < world.terrainSettings.mapChunkSize * structure.regionDensity; z++)
            {
                float treePos = world.noiseSettings.GetTerrainGenerationFromNoise(new Vector3(x, 0, z), world.terrainSettings.chunkWidth, 1, world.lockedMountainCurve, world.lockedHillCurve);
                if (treePos > structure.scatterAmount)
                    Instantiate(structure.prefabObject, new Vector3(x * world.terrainSettings.chunkWidth, 20, z * world.terrainSettings.chunkWidth), Quaternion.identity);
            }
        }
    }
}
