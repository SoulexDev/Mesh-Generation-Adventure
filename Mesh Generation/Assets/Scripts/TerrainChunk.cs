using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk
{
    public MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    Vector3[] vertices;
    int[] triangles;
    Mesh mesh;
    World world;
    Noise noise;
    private GameObject chunkObject; 
    public TerrainChunk NewChunk(Vector3 pos)
    {
        chunkObject = new GameObject();
        chunkObject.transform.position = (pos);
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        world = GameObject.FindObjectOfType<World>();
        chunkObject.transform.SetParent(world.transform);
        chunkObject.name = "Chunk " + (pos.x / TerrainSettings.chunkWidth).ToString() + ", " + (pos.z / TerrainSettings.chunkWidth).ToString();
        mesh = new Mesh();
        mesh.Clear();
        GenerateChunk(pos);
        meshFilter.mesh = mesh;
        return null;
    }
    public void GenerateChunk(Vector3 pos)
    {
        vertices = new Vector3[(TerrainSettings.chunkWidth + 1) * (TerrainSettings.chunkWidth + 1)];
        for (int i = 0, z = 0; z <= TerrainSettings.chunkWidth; z++)
        {
            for (int x = 0; x <= TerrainSettings.chunkWidth; x++)
            {
                float terrainY = Noise.GetTerrainGenerationFromNoise(new Vector3(x + pos.x,0,z + pos.z), TerrainSettings.chunkWidth, 1);
                //float terrainY = Mathf.PerlinNoise(x * TerrainSettings.mapChunkSize, z * TerrainSettings.mapChunkSize) * 8;
                vertices[i] = new Vector3(x, world.lockedCurve.Evaluate(terrainY) * TerrainSettings.terrainHeight, z);
                i++;
            }
        }
        int vertexIndex = 0;
        int triangleIndex = 0;
        triangles = new int[TerrainSettings.chunkWidth * TerrainSettings.chunkWidth * 6];
        for (int z = 0; z < TerrainSettings.chunkWidth; z++)
        {
            for (int x = 0; x < TerrainSettings.chunkWidth; x++)
            {
                triangles[triangleIndex + 0] = vertexIndex;
                triangles[triangleIndex + 1] = vertexIndex + TerrainSettings.chunkWidth + 1;
                triangles[triangleIndex + 2] = vertexIndex + 1;
                triangles[triangleIndex + 3] = vertexIndex + 1;
                triangles[triangleIndex + 4] = vertexIndex + TerrainSettings.chunkWidth + 1;
                triangles[triangleIndex + 5] = vertexIndex + TerrainSettings.chunkWidth + 2;
                vertexIndex++;
                triangleIndex += 6;
            }
            vertexIndex++;
        }
        UpdateMesh();
    }
    void UpdateMesh()
    {
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
    private void OnDrawGizmos()
    {
        if (vertices == null)
            return;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(vertices[i], 0.1f);
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
