using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawNoise : MonoBehaviour
{
    private World world;
    public Renderer noiseRenderer;
    public bool drawTexture = false;
    private void Start()
    {
        world = FindObjectOfType<World>();
    }
    void DrawTexture()
    {
        int size = world.terrainSettings.chunkWidth * world.terrainSettings.mapChunkSize;
        Texture2D noise = new Texture2D(size, size);
        Color[] colorMap = new Color[size * size];
        Color noiseColor;
        for (int x = 0; x < size; x++)
        {
            for (int z = 0; z < size; z++)
            {
                float colorIndex = world.noiseSettings.GetBiomes(new Vector3(x, 0, z), 1, false, world.seed);
                if (colorIndex == 1)
                    noiseColor = Color.white;
                else if (colorIndex == 0)
                    noiseColor = Color.black;
                else
                    noiseColor = Color.gray;
                noise.SetPixel(x, z, noiseColor);
            }
        }
        //noise.SetPixels(colorMap);
        noise.Apply();
        noiseRenderer.sharedMaterial.mainTexture = noise;
        noiseRenderer.transform.localScale = new Vector3(size, 1, size);
    }
    private void Update()
    {
        if (drawTexture)
        {
            DrawTexture();
            drawTexture = false;
        }
    }
}
