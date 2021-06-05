using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    private TerrainChunk terrainChunk;
    List<TerrainChunk> chunk = new List<TerrainChunk>();
    Noise noise;
    public Vector3 chunkPosition;
    public Material terrainMaterial;
    public Material waterMaterial;
    public AnimationCurve heightCurve;
    [Range(0,50)]
    public float waterHeight;
    [HideInInspector]
    public AnimationCurve lockedCurve;
    public bool performanceMode = false;
    public bool autoUpdate = false;
    bool generatingWorld = false;
    public void StartGenerateWorld()
    {
        if (!generatingWorld)
        {
            lockedCurve.keys = heightCurve.keys;
            for (int c = 0; c < transform.childCount; c++)
            {
                Destroy(transform.GetChild(c).gameObject);
            }
            chunk.Clear();
            StartCoroutine(GenerateWorld());
        }
    }
    public IEnumerator GenerateWorld()
    {
        generatingWorld = true;
        for (int c = 0, x = 0; x <= TerrainSettings.mapChunkSize; x++)
        {
            for (int z = 0; z <= TerrainSettings.mapChunkSize; z++)
            {
                chunk.Add(new TerrainChunk());
                chunkPosition = new Vector3(x * TerrainSettings.chunkWidth, 0, z * TerrainSettings.chunkWidth);
                chunk[c].NewChunk(chunkPosition);
                chunk[c].terrainMeshRenderer.material = terrainMaterial;
                chunk[c].waterMeshRenderer.material = waterMaterial;
                c++;
                if(performanceMode)
                    yield return new WaitForFixedUpdate();
            }
        }
        generatingWorld = false;
    }
}