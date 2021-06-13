using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScriptThing : MonoBehaviour
{
    Mesh mesh;
    
    Vector3[] vertices;
    int[] triangles;

    public int xSize = 200;
    public int zSize = 200;
    public int mapHeight = 12;
    public float depth = .25f;
    float[,] falloffMap;
    public AnimationCurve noiseCurve;

    private MeshCollider _collider;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        meshRenderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<MeshCollider>();
    }

    private void Start()
    {
        CreateShape();
        UpdateMesh();
    }

    void CreateShape()
    {
        //float[,] noiseMap = GenerateNoiseMap(xSize, zSize, 0, 1, 5, 0.1f, 1.5f, new Vector2(0, 0));
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        for(int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = noiseCurve.Evaluate(GetNoiseProfiles(new Vector3(x,0,z), xSize, 20));
                //noiseMap[x, z] = Mathf.Clamp01(noiseMap[x, z] - falloffMap[x, z]);
                vertices[i++] = new Vector3(x, y * mapHeight, z);
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris] = vert;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;
                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

        _collider.sharedMesh = mesh;
    }
    public float GetNoiseProfiles(Vector3 pos, int mapWidth, float scale)
    {
        FastNoiseLite mapNoise = new FastNoiseLite();
        float worldNoiseX = ((pos.x + 0.1f) / mapWidth * scale);
        float worldNoiseZ = ((pos.z + 0.1f) / mapWidth * scale);
        SetNoiseValues(mapNoise);
        return mapNoise.GetNoise(worldNoiseX, worldNoiseZ);
    }
    public void SetNoiseValues(FastNoiseLite mapNoise)
    {
        mapNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
        mapNoise.SetFractalType(FastNoiseLite.FractalType.FBm);
        mapNoise.SetFractalOctaves(5);
        mapNoise.SetFrequency(0.1f);
        mapNoise.SetFractalLacunarity(1.5f);
    }
    public static float[,] GenerateFalloffMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value);
            }
        }

        return map;
    }
    static float Evaluate(float value)
    {
        float a = 3;
        float b = 2.2f;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;


        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffsets[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }
                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
            }
        }
        return noiseMap;
    }
}
