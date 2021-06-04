using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    [SerializeField] 
    [Range(10,50)]
    public int meshXSize;
    [SerializeField]
    [Range(10, 50)]
    public int meshZSize;
    //List<Vector3> verticies = new List<Vector3>();
    private Vector3[] vertices;
    //List<int> triangles = new List<int>();
    int[] triangles;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    [SerializeField]
    float drawSpeed;
    [SerializeField]
    Color vertexColor;
    Mesh mesh;
    int vertexIndex;
    public float terrainYHeight;
    void Start()
    {
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = new Mesh();
        mesh.MarkDynamic();
        StartCoroutine("Generate");
        meshFilter.mesh = mesh;
    }
    IEnumerator Generate()
    {
        vertices = new Vector3[(meshXSize + 1) * (meshZSize + 1)];
        for (int i = 0, z = 0; z <= meshZSize; z++)
        {
            for (int x = 0; x <= meshXSize; x++)
            {
                float terrainHeight = Mathf.PerlinNoise(x * terrainYHeight, z * terrainYHeight) * 8;
                vertices[i] = new Vector3(x, terrainHeight, z);
                i++;
            }
        }
        triangles = new int[meshXSize * meshZSize * 6];
        int vert = 0;
        int tris = 0;
        for (int z = 0; z < meshZSize; z++)
        {
            for (int x = 0; x < meshXSize; x++)
            {
                triangles[tris + 0] = vert;
                triangles[tris + 1] = vert + meshXSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + meshXSize + 1;
                triangles[tris + 5] = vert + meshXSize + 2;
                vert++;
                tris += 6;
                //yield return new WaitForSeconds(drawSpeed);
                UpdateMesh();
            }
            vert++;
        }
        yield return null;
    }
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.MarkModified();
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
