using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk
{
    public MeshRenderer terrainMeshRenderer;
    private MeshFilter terrainMeshFilter;
    public MeshRenderer waterMeshRenderer;
    private MeshFilter waterMeshFilter;
    Vector3[] terrainVertices;
    int[] terrainTriangles;
    Vector3[] waterVertices;
    int[] waterTriangles;
    Mesh terrainMesh;
    Mesh waterMesh;
    MeshCollider terrainCollider;
    World world;
    NoiseSettings noise;
    TerrainSettings terrainSettings;
    LODS lods;
    Vector2[] waterUvs;
    Vector2[] terrainUvs;
    private GameObject terrainChunkObject;
    private GameObject waterChunkObject;
    public StructureGeneration structureGeneration;
    public void Init(Vector3 pos)
    {
        terrainChunkObject = new GameObject();
        waterChunkObject = new GameObject();
        terrainChunkObject.transform.position = pos;
        waterChunkObject.transform.position = pos;
        terrainMeshRenderer = terrainChunkObject.AddComponent<MeshRenderer>();
        terrainMeshFilter = terrainChunkObject.AddComponent<MeshFilter>();
        waterMeshRenderer = waterChunkObject.AddComponent<MeshRenderer>();
        waterMeshFilter = waterChunkObject.AddComponent<MeshFilter>();
        terrainCollider = terrainChunkObject.AddComponent<MeshCollider>();
        world = GameObject.FindObjectOfType<World>();
        noise = world.noiseSettings;
        lods = world.Lod;
        structureGeneration = world.structureGeneration;
        terrainSettings = world.terrainSettings;
        terrainChunkObject.transform.SetParent(world.transform);
        waterChunkObject.transform.SetParent(world.transform);
        terrainChunkObject.name = "TerrainChunk " + (pos.x / terrainSettings.chunkWidth).ToString() + ", " + (pos.z / terrainSettings.chunkWidth).ToString();
        waterChunkObject.name = "WaterChunk " + (pos.x / terrainSettings.chunkWidth).ToString() + ", " + (pos.z / terrainSettings.chunkWidth).ToString();
        terrainMesh = new Mesh();
        waterMesh = new Mesh();
        terrainMesh.Clear();
        waterMesh.Clear();
        GenerateTerrainChunk(pos);
        GenerateWaterChunk(pos);
        terrainMeshFilter.mesh = terrainMesh;
        waterMeshFilter.mesh = waterMesh;
        terrainCollider.sharedMesh = terrainMesh;
        terrainChunkObject.layer = 3;
    }
    public void GenerateTerrainChunk(Vector3 pos)
    {
        if(structureGeneration != null)
            structureGeneration.GenerateStructure(pos, terrainChunkObject.transform, 0);
        int levelOfDetail = (int)lods.levelsOfDetail;
        int terrainLOD = terrainSettings.chunkWidth / levelOfDetail;
        terrainVertices = new Vector3[(terrainLOD + 1) * (terrainLOD + 1)];
        for (int i = 0, z = 0; z <= terrainSettings.chunkWidth; z+= levelOfDetail)
        {
            for (int x = 0; x <= terrainSettings.chunkWidth; x+= levelOfDetail)
            {
                float terrainY = noise.GetTerrainGenerationFromNoise(new Vector3(x + pos.x,0,z + pos.z), terrainSettings.chunkWidth, 1, world.lockedMountainCurve, world.lockedHillCurve);
                terrainVertices[i] = new Vector3(x, terrainY * terrainSettings.terrainHeight, z);
                i++;
            }
        }
        int vertexIndex = 0;
        int triangleIndex = 0;
        terrainTriangles = new int[terrainLOD * terrainLOD * 6];
        for (int z = 0; z < terrainLOD; z++)
        {
            for (int x = 0; x < terrainLOD; x++)
            {
                terrainTriangles[triangleIndex + 0] = vertexIndex;
                terrainTriangles[triangleIndex + 1] = vertexIndex + terrainLOD + 1;
                terrainTriangles[triangleIndex + 2] = vertexIndex + 1;
                terrainTriangles[triangleIndex + 3] = vertexIndex + 1;
                terrainTriangles[triangleIndex + 4] = vertexIndex + terrainLOD + 1;
                terrainTriangles[triangleIndex + 5] = vertexIndex + terrainLOD + 2;
                vertexIndex++;
                triangleIndex += 6;
            }
            vertexIndex++;
        }
        UpdateTerrainMesh();
    }
    public void GenerateWaterChunk(Vector3 pos)
    {
        waterVertices = new Vector3[(terrainSettings.chunkWidth + 1) * (terrainSettings.chunkWidth + 1)];
        for (int i = 0, z = 0; z <= terrainSettings.chunkWidth; z++)
        {
            for (int x = 0; x <= terrainSettings.chunkWidth; x++)
            {
                waterVertices[i] = new Vector3(x, world.waterHeight / terrainSettings.terrainHeight * 5, z);
                i++;
            }
        }
        int vertexIndex = 0;
        int triangleIndex = 0;
        waterTriangles = new int[terrainSettings.chunkWidth * terrainSettings.chunkWidth * 6];
        for (int z = 0; z < terrainSettings.chunkWidth; z++)
        {
            for (int x = 0; x < terrainSettings.chunkWidth; x++)
            {
                waterTriangles[triangleIndex + 0] = vertexIndex;
                waterTriangles[triangleIndex + 1] = vertexIndex + terrainSettings.chunkWidth + 1;
                waterTriangles[triangleIndex + 2] = vertexIndex + 1;
                waterTriangles[triangleIndex + 3] = vertexIndex + 1;
                waterTriangles[triangleIndex + 4] = vertexIndex + terrainSettings.chunkWidth + 1;
                waterTriangles[triangleIndex + 5] = vertexIndex + terrainSettings.chunkWidth + 2;
                vertexIndex++;
                triangleIndex += 6;
            }
            vertexIndex++;
        }
        UpdateWaterMesh();
    }
    void UpdateTerrainMesh()
    {
        terrainUvs = new Vector2[terrainVertices.Length];
        for (int i = 0; i < terrainUvs.Length; i++)
        {
            terrainUvs[i] = new Vector2(terrainVertices[i].x, terrainVertices[i].z);
        }
        terrainMesh.vertices = terrainVertices;
        terrainMesh.triangles = terrainTriangles;
        terrainMesh.uv = terrainUvs;
        terrainMesh.RecalculateNormals();
    }
    void UpdateWaterMesh()
    {
        waterUvs = new Vector2[waterVertices.Length];
        for (int i = 0; i < waterUvs.Length; i++)
        {
            waterUvs[i] = new Vector2(waterVertices[i].x, waterVertices[i].z);
        }
        waterMesh.vertices = waterVertices;
        waterMesh.triangles = waterTriangles;
        waterMesh.uv = waterUvs;
        waterMesh.RecalculateNormals();
    }
    private void OnDrawGizmos()
    {
        if (terrainVertices == null)
            return;
        for (int i = 0; i < terrainVertices.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(terrainVertices[i], 0.1f);
        }
    }
    static void drawString(string text, Vector3 worldPos, Color? colour = null)
    {
        UnityEditor.Handles.BeginGUI();
        if (colour.HasValue) GUI.color = colour.Value;
        var view = UnityEditor.SceneView.currentDrawingSceneView;
        Vector3 screenPos = view.camera.WorldToScreenPoint(worldPos);
        Vector2 size = GUI.skin.label.CalcSize(new GUIContent(text));
        GUI.Label(new Rect(screenPos.x - (size.x / 2), -screenPos.y + view.position.height + 4, size.x, size.y), text);
        UnityEditor.Handles.EndGUI();
    }
}