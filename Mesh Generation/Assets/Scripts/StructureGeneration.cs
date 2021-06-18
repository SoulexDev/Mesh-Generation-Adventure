using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class StructureGeneration : MonoBehaviour
{
    private World world;
    private int seed;
    private void Start()
    {
        world = FindObjectOfType<World>();
        Random.InitState(seed);
    }
    public void GenerateStructure(Vector3 pos, Transform terrainChunk, int biomeID, int spawnableID)
    {
        if (world != null)
        {
            for (int x = 0; x <= world.terrainSettings.chunkWidth; x+= 2)
            {
                for (int z = 0; z <= world.terrainSettings.chunkWidth; z+= 2)
                {
                    float spawnableLocation = world.noiseSettings.GetTerrainGenerationFromNoise(new Vector3(x + pos.x, 0, z + pos.z), world.terrainSettings.chunkWidth, 1, biomeID, world.seed);
                    float spawnablePool = world.noiseSettings.GetStructureMask(new Vector3(x + pos.x, 0, z + pos.z), world.terrainSettings.chunkWidth, 1, biomeID, spawnableID, seed);
                    if (spawnableLocation > world.biomes[biomeID].spawnables[spawnableID].minimumHeight && spawnablePool > 0.3f)
                        if (world.biomes[biomeID].spawnables[spawnableID].regionDensity == (int)Random.Range(world.biomes[biomeID].spawnables[spawnableID].regionDensity, 100))
                            Instantiate(world.biomes[biomeID].spawnables[spawnableID].prefabObject, new Vector3(x + pos.x, spawnableLocation * world.terrainSettings.terrainHeight, z + pos.z), Quaternion.Euler(0, Random.Range(0, 360), 0), terrainChunk);
                }
            }
        }
    }
}