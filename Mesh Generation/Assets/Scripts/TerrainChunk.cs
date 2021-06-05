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
    World world;
    Noise noise;
    Vector2[] uvs;
    private GameObject terrainChunkObject;
    private GameObject waterChunkObject;
    public TerrainChunk NewChunk(Vector3 pos)
    {
        terrainChunkObject = new GameObject();
        waterChunkObject = new GameObject();
        terrainChunkObject.transform.position = pos;
        waterChunkObject.transform.position = pos;
        terrainMeshRenderer = terrainChunkObject.AddComponent<MeshRenderer>();
        terrainMeshFilter = terrainChunkObject.AddComponent<MeshFilter>();
        waterMeshRenderer = waterChunkObject.AddComponent<MeshRenderer>();
        waterMeshFilter = waterChunkObject.AddComponent<MeshFilter>();
        world = GameObject.FindObjectOfType<World>();
        terrainChunkObject.transform.SetParent(world.transform);
        waterChunkObject.transform.SetParent(world.transform);
        terrainChunkObject.name = "TerrainChunk " + (pos.x / TerrainSettings.chunkWidth).ToString() + ", " + (pos.z / TerrainSettings.chunkWidth).ToString();
        waterChunkObject.name = "WaterChunk" + (pos.x / TerrainSettings.chunkWidth).ToString() + ", " + (pos.z / TerrainSettings.chunkWidth).ToString();
        terrainMesh = new Mesh();
        waterMesh = new Mesh();
        terrainMesh.Clear();
        waterMesh.Clear();
        GenerateTerrainChunk(pos);
        GenerateWaterChunk(pos);
        terrainMeshFilter.mesh = terrainMesh;
        waterMeshFilter.mesh = waterMesh;
        return null;
    }
    public void GenerateTerrainChunk(Vector3 pos)
    {
        terrainVertices = new Vector3[(TerrainSettings.chunkWidth + 1) * (TerrainSettings.chunkWidth + 1)];
        for (int i = 0, z = 0; z <= TerrainSettings.chunkWidth; z++)
        {
            for (int x = 0; x <= TerrainSettings.chunkWidth; x++)
            {
                float terrainY = Noise.GetTerrainGenerationFromNoise(new Vector3(x + pos.x,0,z + pos.z), TerrainSettings.chunkWidth, 1);
                terrainVertices[i] = new Vector3(x, world.lockedCurve.Evaluate(terrainY) * TerrainSettings.terrainHeight, z);
                i++;
            }
        }
        int vertexIndex = 0;
        int triangleIndex = 0;
        terrainTriangles = new int[TerrainSettings.chunkWidth * TerrainSettings.chunkWidth * 6];
        for (int z = 0; z < TerrainSettings.chunkWidth; z++)
        {
            for (int x = 0; x < TerrainSettings.chunkWidth; x++)
            {
                terrainTriangles[triangleIndex + 0] = vertexIndex;
                terrainTriangles[triangleIndex + 1] = vertexIndex + TerrainSettings.chunkWidth + 1;
                terrainTriangles[triangleIndex + 2] = vertexIndex + 1;
                terrainTriangles[triangleIndex + 3] = vertexIndex + 1;
                terrainTriangles[triangleIndex + 4] = vertexIndex + TerrainSettings.chunkWidth + 1;
                terrainTriangles[triangleIndex + 5] = vertexIndex + TerrainSettings.chunkWidth + 2;
                vertexIndex++;
                triangleIndex += 6;
            }
            vertexIndex++;
        }
        UpdateTerrainMesh();
    }
    public void GenerateWaterChunk(Vector3 pos)
    {
        waterVertices = new Vector3[(TerrainSettings.chunkWidth + 1) * (TerrainSettings.chunkWidth + 1)];
        for (int i = 0, z = 0; z <= TerrainSettings.chunkWidth; z++)
        {
            for (int x = 0; x <= TerrainSettings.chunkWidth; x++)
            {
                waterVertices[i] = new Vector3(x, world.waterHeight / TerrainSettings.terrainHeight * 5, z);
                i++;
            }
        }
        int vertexIndex = 0;
        int triangleIndex = 0;
        waterTriangles = new int[TerrainSettings.chunkWidth * TerrainSettings.chunkWidth * 6];
        for (int z = 0; z < TerrainSettings.chunkWidth; z++)
        {
            for (int x = 0; x < TerrainSettings.chunkWidth; x++)
            {
                waterTriangles[triangleIndex + 0] = vertexIndex;
                waterTriangles[triangleIndex + 1] = vertexIndex + TerrainSettings.chunkWidth + 1;
                waterTriangles[triangleIndex + 2] = vertexIndex + 1;
                waterTriangles[triangleIndex + 3] = vertexIndex + 1;
                waterTriangles[triangleIndex + 4] = vertexIndex + TerrainSettings.chunkWidth + 1;
                waterTriangles[triangleIndex + 5] = vertexIndex + TerrainSettings.chunkWidth + 2;
                vertexIndex++;
                triangleIndex += 6;
            }
            vertexIndex++;
        }
        UpdateWaterMesh();
    }
    void UpdateTerrainMesh()
    {
        terrainMesh.vertices = terrainVertices;
        terrainMesh.triangles = terrainTriangles;
        terrainMesh.RecalculateNormals();
    }
    void UpdateWaterMesh()
    {
        uvs = new Vector2[waterVertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(waterVertices[i].x, waterVertices[i].z);
        }
        waterMesh.vertices = waterVertices;
        waterMesh.triangles = waterTriangles;
        waterMesh.uv = uvs;
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
