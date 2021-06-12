using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [HideInInspector]
    public Vector3 playerSpawnPosition;
    int renderDistanceInChunks = 32;
    public float unitRenderDistance;
    [HideInInspector]
    public float LODradiusOne, LODradiusTwo, LODradiusThree;
    private PlayerMovement player;
    private TerrainChunk terrainChunk;
    List<TerrainChunk> chunk = new List<TerrainChunk>();
    [HideInInspector]
    public Vector3 chunkPosition;
    public Material[] terrainMaterial;
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
    public TerrainSettings terrainSettings;
    [HideInInspector]
    public NoiseSettings noiseSettings;
    [HideInInspector]
    public Spawnable[] spawnables;
    public Biome[] biomes;
    public StructureGeneration structureGeneration;
    private void Start()
    {
        noiseSettings.world = this;
        player = FindObjectOfType<PlayerMovement>();
        structureGeneration = FindObjectOfType<StructureGeneration>();
        unitRenderDistance = renderDistanceInChunks * terrainSettings.chunkWidth * 100;
        LODradiusOne = unitRenderDistance / 4;
        LODradiusTwo = LODradiusOne * 2;
        LODradiusThree = unitRenderDistance;
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
                int biome = chunk[c].GetBiomeIndex(chunkPosition);
                chunk[c].terrainMeshRenderer.material = terrainMaterial[biome];
                chunk[c].waterMeshRenderer.material = waterMaterial;
                c++;
                if(performanceMode)
                    yield return new WaitForEndOfFrame();
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
            if (chunk[i].ChunkDistanceFromPlayer(player.transform.position) <= LODradiusOne && chunk[i].LODIndex != 1)
                chunk[i].GenerateTerrainChunk(chunk[i].terrainChunkObject.transform.position, 1);
            else if (chunk[i].ChunkDistanceFromPlayer(player.transform.position) <= LODradiusTwo && chunk[i].ChunkDistanceFromPlayer(player.transform.position) >= LODradiusOne && chunk[i].LODIndex != 2)
                chunk[i].GenerateTerrainChunk(chunk[i].terrainChunkObject.transform.position, 2);
            else if (chunk[i].ChunkDistanceFromPlayer(player.transform.position) <= LODradiusThree && chunk[i].ChunkDistanceFromPlayer(player.transform.position) >= LODradiusTwo && chunk[i].LODIndex != 4)
                chunk[i].GenerateTerrainChunk(chunk[i].terrainChunkObject.transform.position, 4);
        }
    }
}
[System.Serializable]
public class NoiseSettings
{
    public World world;
    public float GetTerrainGenerationFromNoise(Vector3 pos, int chunkWidth, float scale, int biomeID)
    {
        float noiseTypeOne;
        float noiseTypeTwo;
        float noiseTypeThree;
        float noiseTypeFour;
        float noiseTypeFive;
        float noiseTypeSix;
        FastNoiseLite mountains = new FastNoiseLite();
        FastNoiseLite hills = new FastNoiseLite();
        FastNoiseLite cliffs = new FastNoiseLite();
        FastNoiseLite islands = new FastNoiseLite();
        FastNoiseLite spores = new FastNoiseLite();
        FastNoiseLite ridges = new FastNoiseLite();
        float worldNoiseX = ((pos.x + 0.1f) / chunkWidth * scale);
        float worldNoiseZ = ((pos.z + 0.1f) / chunkWidth * scale);
        SetNoiseValues(mountains, hills, cliffs, islands, spores, ridges, biomeID);
        if (world.biomes[biomeID].noiseOptions.mountains)
            noiseTypeOne = world.biomes[biomeID].noiseOptions.mountainCurve.Evaluate(mountains.GetNoise(worldNoiseX, worldNoiseZ));
        else
            noiseTypeOne = 0;
        if (world.biomes[biomeID].noiseOptions.hills)
            noiseTypeTwo = world.biomes[biomeID].noiseOptions.hillCurve.Evaluate(hills.GetNoise(worldNoiseX, worldNoiseZ));
        else
            noiseTypeTwo = 0;
        if (world.biomes[biomeID].noiseOptions.cliffs)
            noiseTypeThree = world.biomes[biomeID].noiseOptions.cliffCurve.Evaluate(cliffs.GetNoise(worldNoiseX, worldNoiseZ));
        else
            noiseTypeThree = 0;
        if (world.biomes[biomeID].noiseOptions.islands)
            noiseTypeFour = world.biomes[biomeID].noiseOptions.islandCurve.Evaluate(islands.GetNoise(worldNoiseX, worldNoiseZ));
        else
            noiseTypeFour = 0;
        if (world.biomes[biomeID].noiseOptions.spores)
            noiseTypeFive = world.biomes[biomeID].noiseOptions.sporeCurve.Evaluate(spores.GetNoise(worldNoiseX, worldNoiseZ));
        else
            noiseTypeFive = 0;
        if (world.biomes[biomeID].noiseOptions.ridges)
            noiseTypeSix = world.biomes[biomeID].noiseOptions.ridgesCurve.Evaluate(ridges.GetNoise(worldNoiseX, worldNoiseZ));
        else
            noiseTypeSix = 0;
        return noiseTypeOne + noiseTypeTwo + noiseTypeThree + noiseTypeFour + noiseTypeFive + noiseTypeSix;
    }
    public float GetStructureMask(Vector3 pos, int chunkWidth, float scale, int biomeID, int spawnableID)
    {
        FastNoiseLite structureMask = new FastNoiseLite();
        float worldNoiseX = ((pos.x + 0.1f) / chunkWidth * scale);
        float worldNoiseZ = ((pos.z + 0.1f) / chunkWidth * scale);
        SetStructureMaskValues(structureMask, biomeID, spawnableID);
        return structureMask.GetNoise(worldNoiseX, worldNoiseZ);
    }
    public float GetBiomes(Vector3 pos, float scale, bool biomeSettingsType)
    {
        FastNoiseLite biomeTemperature = new FastNoiseLite();
        FastNoiseLite biomeHeight = new FastNoiseLite();
        float biomeNoiseX = ((pos.x + 0.1f) / world.terrainSettings.mapChunkSize * scale);
        float biomeNoiseZ = ((pos.z + 0.1f) / world.terrainSettings.mapChunkSize * scale);
        SetBiomeNoise(biomeTemperature, biomeHeight);
        if (biomeSettingsType)
            return biomeTemperature.GetNoise(biomeNoiseX, biomeNoiseZ);
        else
            return biomeHeight.GetNoise(biomeNoiseX, biomeNoiseZ);
    }
    public void SetBiomeNoise(FastNoiseLite biomeTemperature, FastNoiseLite biomeHeight)
    {
        //Temperature
        biomeTemperature.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
        biomeTemperature.SetFractalType(FastNoiseLite.FractalType.FBm);
        biomeTemperature.SetFractalOctaves(2);
        biomeTemperature.SetFractalLacunarity(1);
        biomeTemperature.SetFrequency(0.05f);
        //Height
        biomeHeight.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        biomeHeight.SetFractalType(FastNoiseLite.FractalType.None);
        biomeHeight.SetFractalOctaves(2);
        biomeHeight.SetFractalLacunarity(1);
        biomeHeight.SetFrequency(0.05f);
    }
    public void SetStructureMaskValues(FastNoiseLite structureMask, int biomeID, int spawnableID)
    {
        //Structure Mask
        structureMask.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        structureMask.SetFractalType(FastNoiseLite.FractalType.FBm);
        structureMask.SetFractalOctaves(world.biomes[biomeID].spawnables[spawnableID].structureMaskOctaves);
        structureMask.SetFractalWeightedStrength(world.biomes[biomeID].spawnables[spawnableID].structureMaskWeightedStrength);
        structureMask.SetFractalLacunarity(world.biomes[biomeID].spawnables[spawnableID].structureMaskLacunarity);
        structureMask.SetFrequency(world.biomes[biomeID].spawnables[spawnableID].structureMaskFrequency);
    }
    public void SetNoiseValues(FastNoiseLite mountains, FastNoiseLite hills, FastNoiseLite cliffs, FastNoiseLite islands, FastNoiseLite spores, FastNoiseLite ridges, int biomeID)
    {
        //Mountains
        if (world.biomes[biomeID].noiseOptions.mountains)
        {
            mountains.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            mountains.SetFractalType(FastNoiseLite.FractalType.Ridged);
            mountains.SetFractalOctaves(world.biomes[biomeID].noiseOptions.mountainOctaves);
            mountains.SetFractalLacunarity(world.biomes[biomeID].noiseOptions.mountainLacunarity);
            mountains.SetFractalGain(world.biomes[biomeID].noiseOptions.mountainFractalGain);
            mountains.SetFrequency(world.biomes[biomeID].noiseOptions.mountainFrequency);
        }
        //Hills
        if (world.biomes[biomeID].noiseOptions.hills)
        {
            hills.SetNoiseType(FastNoiseLite.NoiseType.Perlin);
            hills.SetFractalType(FastNoiseLite.FractalType.FBm);
            hills.SetFractalLacunarity(world.biomes[biomeID].noiseOptions.hillLacunarity);
            hills.SetFractalOctaves(world.biomes[biomeID].noiseOptions.hillOctaves);
            hills.SetFrequency(world.biomes[biomeID].noiseOptions.hillFrequency);
        }
        //Cliffs
        if (world.biomes[biomeID].noiseOptions.cliffs)
        {
            cliffs.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            cliffs.SetFractalType(FastNoiseLite.FractalType.None);
            cliffs.SetFrequency(world.biomes[biomeID].noiseOptions.cliffFrequency);
            cliffs.SetCellularDistanceFunction(FastNoiseLite.CellularDistanceFunction.Hybrid);
            cliffs.SetCellularReturnType(FastNoiseLite.CellularReturnType.CellValue);
            cliffs.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
            cliffs.SetDomainWarpAmp(150);
        }
        //Islands
        if (world.biomes[biomeID].noiseOptions.islands)
        {
            islands.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            islands.SetFractalType(FastNoiseLite.FractalType.FBm);
            islands.SetFractalOctaves(world.biomes[biomeID].noiseOptions.islandsOctaves);
            islands.SetFractalLacunarity(world.biomes[biomeID].noiseOptions.islandsLacunarity);
            islands.SetFractalGain(world.biomes[biomeID].noiseOptions.islandsFractalGain);
            islands.SetFractalWeightedStrength(world.biomes[biomeID].noiseOptions.islandsWeightedStrength);
        }
        //Spores
        if (world.biomes[biomeID].noiseOptions.spores)
        {
            spores.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            spores.SetFractalType(FastNoiseLite.FractalType.FBm);
            spores.SetFrequency(world.biomes[biomeID].noiseOptions.sporesFrequency);
            spores.SetFractalOctaves(world.biomes[biomeID].noiseOptions.sporesOctaves);
            spores.SetFractalLacunarity(world.biomes[biomeID].noiseOptions.sporesLacunarity);
            spores.SetFractalGain(world.biomes[biomeID].noiseOptions.sporesFractalGain);
            spores.SetFractalWeightedStrength(world.biomes[biomeID].noiseOptions.sporesWeightedStrength);
            spores.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Add);
        }
        //Ridges
        if (world.biomes[biomeID].noiseOptions.ridges)
        {
            ridges.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            ridges.SetFractalType(FastNoiseLite.FractalType.Ridged);
            ridges.SetFrequency(world.biomes[biomeID].noiseOptions.ridgesFrequency);
            ridges.SetFractalOctaves(world.biomes[biomeID].noiseOptions.ridgesOctaves);
            ridges.SetFractalLacunarity(world.biomes[biomeID].noiseOptions.ridgesLacunarity);
            ridges.SetFractalGain(world.biomes[biomeID].noiseOptions.ridgesFractalGain);
            ridges.SetFractalWeightedStrength(world.biomes[biomeID].noiseOptions.ridgesWeightedStrength);
        }
    }
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
public class Biome
{
    public string biomeName;
    public NoiseTypesOptions noiseOptions;
    public Spawnable[] spawnables;
    public enum TreeTypes { None, Oak, Birch, Jungle, Shrub, Crimson, Charcoaled }
    //public TreeTypes[] treeTypes;
    public enum BuildingTypes { None, Dungeon, Colleseum, Village, StoneHedge, AtlantisVillage, Temple, House, AbandonedStructure }
    //public BuildingTypes[] buildingTypes;
    public enum LootBoxTypes { None, SmallChest, LargeChest, Crate, AlienChest, HomeOwnersStorageBox }
    //public LootBoxTypes[] lootBoxTypes;
}