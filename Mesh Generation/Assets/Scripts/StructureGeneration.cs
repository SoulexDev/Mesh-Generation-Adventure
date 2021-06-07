using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class StructureGeneration : MonoBehaviour
{
    private World world;
    private void Start()
    {
        world = FindObjectOfType<World>();
    }
    public void GenerateStructure(Vector3 pos, Transform terrainChunk, int structureID)
    {
        if (world != null)
        {
            for (int x = 0; x <= world.terrainSettings.chunkWidth; x++)
            {
                for (int z = 0; z <= world.terrainSettings.chunkWidth; z++)
                {
                    float structureSpawnRate = world.noiseSettings.GetTerrainGenerationFromNoise(new Vector3(x + pos.x, 0, z + pos.z), world.terrainSettings.chunkWidth, 1, world.lockedMountainCurve, world.lockedHillCurve);
                    if (structureSpawnRate < world.structure[structureID].scatterAmount && structureSpawnRate > 0.1f)
                        if (1 == Random.Range(0, world.structure[structureID].regionDensity))
                            Instantiate(world.structure[structureID].prefabObject, new Vector3(x + pos.x, structureSpawnRate * world.terrainSettings.terrainHeight, z + pos.z), Quaternion.Euler(-90, 0, 0), terrainChunk);
                }
            }
        }
    }
}
