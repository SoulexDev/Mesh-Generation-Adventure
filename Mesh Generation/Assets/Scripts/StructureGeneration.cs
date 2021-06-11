using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StructureGeneration : MonoBehaviour
{
    private World world;
    private void Start()
    {
        world = FindObjectOfType<World>();
    }
    public void GenerateStructure(Vector3 pos, Transform terrainChunk, int biomeID, int spawnableID)
    {
        if (world != null)
        {
            for (int x = 0; x <= world.terrainSettings.chunkWidth; x++)
            {
                for (int z = 0; z <= world.terrainSettings.chunkWidth; z++)
                {
                    float spawnableLocation = world.noiseSettings.GetTerrainGenerationFromNoise(new Vector3(x + pos.x, 0, z + pos.z), world.terrainSettings.chunkWidth, 1, world.lockedMountainCurve, world.lockedHillCurve, world.lockedHillCurve, world.lockedHillCurve, world.lockedHillCurve, world.lockedHillCurve, 0);
                    float spawnablePool = world.noiseSettings.GetTerrainGenerationFromNoise(new Vector3(x + pos.x, 0, z + pos.z), world.terrainSettings.chunkWidth, 1, world.lockedMountainCurve, world.lockedHillCurve, world.lockedHillCurve, world.lockedHillCurve, world.lockedHillCurve, world.lockedHillCurve, 1);
                    if (spawnableLocation < world.biomes[biomeID].spawnables[spawnableID].scatterAmount && spawnableLocation > world.biomes[biomeID].spawnables[spawnableID].minimumScatterYValue && spawnablePool > 0.3f)
                        if (world.biomes[biomeID].spawnables[spawnableID].regionDensity == (int)Random.Range(world.biomes[biomeID].spawnables[spawnableID].regionDensity, 100))
                            Instantiate(world.biomes[biomeID].spawnables[spawnableID].prefabObject, new Vector3(x + pos.x, spawnableLocation * world.terrainSettings.terrainHeight, z + pos.z), Quaternion.Euler(-90, 0, 0), terrainChunk);
                }
            }
        }
    }
}