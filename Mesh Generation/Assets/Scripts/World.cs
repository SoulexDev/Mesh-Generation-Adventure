using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [HideInInspector]
    public Vector3 playerSpawnPosition;
    int renderDistanceInChunks = 32;
    public float unitRenderDistance;
    public float LODradiusOne, LODradiusTwo, LODradiusThree;
    private PlayerMovement player;
    private TerrainChunk terrainChunk;
    List<TerrainChunk> chunk = new List<TerrainChunk>();
    public Vector3 chunkPosition;
    public Material terrainMaterial;
    public Material waterMaterial;
    public AnimationCurve mountainHeightCurve;
    public AnimationCurve hillHeightCurve;
    [Range(0,50)]
    public float waterHeight;
    [HideInInspector]
    public AnimationCurve lockedMountainCurve;
    [HideInInspector]
    public AnimationCurve lockedHillCurve;
    public bool performanceMode = false;
    public bool autoUpdate = false;
    public static bool generatingWorld = false;
    public LODS Lod;
    public TerrainSettings terrainSettings;
    [HideInInspector]
    public NoiseSettings noiseSettings;
    public Spawnable[] spawnables;
    public Biome[] biomes;
    public StructureGeneration structureGeneration;
    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        structureGeneration = FindObjectOfType<StructureGeneration>();
        unitRenderDistance = renderDistanceInChunks * terrainSettings.chunkWidth * 100;
        LODradiusOne = unitRenderDistance / 3;
        LODradiusTwo = LODradiusOne * 2;
        LODradiusThree = unitRenderDistance;
        Debug.Log($"RadiusOne{LODradiusOne}RadiusTwo{LODradiusTwo}RadiusThree{LODradiusThree}");
    }
    private void Update()
    {
        UpdateChunks();
    }
    public void StartGenerateWorld()
    {
        if (!generatingWorld)
        {
            terrainSettings.mapCenter = new Vector3((terrainSettings.mapChunkSize * terrainSettings.chunkWidth) / 2, terrainSettings.terrainHeight, (terrainSettings.mapChunkSize* terrainSettings.chunkWidth) / 2);
            playerSpawnPosition = terrainSettings.mapCenter;
            lockedHillCurve.keys = hillHeightCurve.keys;
            lockedMountainCurve.keys = mountainHeightCurve.keys;
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
        for (int c = 0, x = 0; x <= terrainSettings.mapChunkSize; x++)
        {
            for (int z = 0; z <= terrainSettings.mapChunkSize; z++)
            {
                chunk.Add(new TerrainChunk());
                chunkPosition = new Vector3(x * terrainSettings.chunkWidth, 0, z * terrainSettings.chunkWidth);
                chunk[c].Init(chunkPosition);
                chunk[c].terrainMeshRenderer.material = terrainMaterial;
                chunk[c].waterMeshRenderer.material = waterMaterial;
                c++;
                if(performanceMode)
                    yield return new WaitForFixedUpdate();
            }
        }
        generatingWorld = false;
        player.gameObject.transform.position = playerSpawnPosition;
    }
    void UpdateChunks()
    {
        for (int i = 0; i < chunk.Count; i++)
        {
            if (chunk[i].ChunkDistanceFromPlayer(player.transform.position) > unitRenderDistance)
            {
                if (chunk[i].terrainChunkObject.activeSelf)
                    chunk[i].terrainChunkObject.SetActive(false);
            }
            else if(!chunk[i].terrainChunkObject.activeSelf)
                chunk[i].terrainChunkObject.SetActive(true);

            if (chunk[i].terrainChunkObject.transform.position.sqrMagnitude <= chunk[i].ChunkLODRadiusFromPlayer(player.transform.position, LODradiusOne) && chunk[i].LODIndex != 1)
                chunk[i].GenerateTerrainChunk(chunk[i].terrainChunkObject.transform.position, 1);
            else if (chunk[i].terrainChunkObject.transform.position.sqrMagnitude <= chunk[i].ChunkLODRadiusFromPlayer(player.transform.position, LODradiusTwo) && chunk[i].terrainChunkObject.transform.position.sqrMagnitude >= chunk[i].ChunkLODRadiusFromPlayer(player.transform.position, LODradiusOne) && chunk[i].LODIndex != 2)
                chunk[i].GenerateTerrainChunk(chunk[i].terrainChunkObject.transform.position, 2);
            else if (chunk[i].terrainChunkObject.transform.position.sqrMagnitude <= chunk[i].ChunkLODRadiusFromPlayer(player.transform.position, LODradiusThree) && chunk[i].terrainChunkObject.transform.position.sqrMagnitude >= chunk[i].ChunkLODRadiusFromPlayer(player.transform.position, LODradiusTwo) && chunk[i].LODIndex != 4)
                chunk[i].GenerateTerrainChunk(chunk[i].terrainChunkObject.transform.position, 4);
        }
    }
}
[System.Serializable]
public class NoiseSettings
{
    [Header("Noise Types: Mountains and Hills(0), Structure Noise Mask(1)")]
    public int noiseType;
    [Header("Mountain Settings")]
    public float mountainLacunarity = 1.6f;
    public float mountainOctaves = 5;
    public float mountainGain = 0.4f;
    public float mountainFrequency = 0.1f;
    [Header("Hill Settings")]
    public float hillLacunarity = 1.6f;
    public float hillOctaves = 9;
    public float hillFrequency = 0.4f;
    public float GetTerrainGenerationFromNoise(Vector3 pos, int chunkWidth, float scale, AnimationCurve mountainCurve, AnimationCurve hillCurve, int noiseType)
    {
        FastNoiseLite mountains = new FastNoiseLite();
        FastNoiseLite hills = new FastNoiseLite();
        FastNoiseLite structureMask = new FastNoiseLite();
        float worldNoiseX = ((pos.x + 0.1f) / chunkWidth * scale);
        float worldNoiseZ = ((pos.z + 0.1f) / chunkWidth * scale);
        SetNoiseValues(mountains, hills, structureMask);
        if (noiseType == 0)
            return mountainCurve.Evaluate(mountains.GetNoise(worldNoiseX, worldNoiseZ)) + hillCurve.Evaluate(hills.GetNoise(worldNoiseX, worldNoiseZ));
        else if (noiseType == 1)
            return structureMask.GetNoise(worldNoiseX, worldNoiseZ);
        else
            return 0;
    }
    public void SetNoiseValues(FastNoiseLite mountains, FastNoiseLite hills, FastNoiseLite structureMask)
    {
        //Hills
        hills.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
        hills.SetFractalType(FastNoiseLite.FractalType.FBm);
        hills.SetFractalLacunarity(1.6f);
        hills.SetFractalOctaves(9);
        hills.SetFrequency(0.4f);
        //Mountains
        mountains.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
        mountains.SetFractalType(FastNoiseLite.FractalType.Ridged);
        mountains.SetFractalOctaves(5);
        mountains.SetFractalLacunarity(1.8f);
        mountains.SetFractalGain(0.4f);
        mountains.SetFrequency(0.1f);
        //Structure Mask
        structureMask.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        structureMask.SetFractalType(FastNoiseLite.FractalType.FBm);
        structureMask.SetFractalOctaves(3);
        structureMask.SetFractalWeightedStrength(1);
        structureMask.SetFractalLacunarity(2);
        structureMask.SetFrequency(0.005f);
    }
}
[System.Serializable]
public class LODS
{
    public enum LODs { LOD1 = 1, LOD2 = 2, LOD3 = 4}
    public LODs levelsOfDetail;
}
[System.Serializable]
public class TerrainSettings
{
    [SerializeField]
    public int chunkWidth = 16;
    [SerializeField]
    public int mapChunkSize = 16;
    [SerializeField]
    public float terrainHeight = 20;
    public Vector3 mapCenter;
}
[System.Serializable]
public class Spawnable
{
    public string spawnableStructureID;
    public GameObject prefabObject;
    [Range(0.1f,1)]
    public float scatterAmount;
    [Range(0.1f, 1)]
    public float minimumScatterYValue;
    [Range(0, 100)]
    public int regionDensity;
}
[System.Serializable]
public class Biome
{
    public string biomeName;
    public Spawnable spawnables;
    public enum NoiseTypes { Mountains, Hills, Cliffs, Islands, Spores, Ridges }
    public NoiseTypes[] noiseTypes;
    public enum TreeTypes { None, Oak, Birch, Jungle, Shrub, Crimson, Charcoaled }
    public TreeTypes[] treeTypes;
    public enum BuildingTypes { None, Dungeon, Colleseum, Village, StoneHedge, AtlantisVillage, Temple, House, AbandonedStructure }
    public BuildingTypes[] buildingTypes;
    public enum LootBoxTypes { None, SmallChest, LargeChest, Crate, AlienChest, HomeOwnersStorageBox }
    public LootBoxTypes[] lootBoxTypes;
}