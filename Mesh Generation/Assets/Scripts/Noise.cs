using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise
{
    public static float GetTerrainGenerationFromNoise(Vector3 pos, int chunkWidth,float scale)
    {
        FastNoiseLite flatLand = new FastNoiseLite();
        FastNoiseLite mountainsAndHills = new FastNoiseLite();
        float worldNoiseX = ((pos.x + 0.1f) / chunkWidth * scale);
        float worldNoiseZ = ((pos.z + 0.1f) / chunkWidth * scale);
        //SetNoiseValues(flatLand, mountainsAndHills);
        //return flatLand.GetNoise(worldNoiseX, worldNoiseZ) + mountainsAndHills.GetNoise(worldNoiseX, worldNoiseZ);
        return Mathf.PerlinNoise(worldNoiseX, worldNoiseZ);
    }
    static void SetNoiseValues(FastNoiseLite flatLand, FastNoiseLite mountainsAndHills)
    {
        //Flatland
        flatLand.SetNoiseType(FastNoiseLite.NoiseType.Value);
        flatLand.SetFractalType(FastNoiseLite.FractalType.None);
        //Mountains and Hills
        mountainsAndHills.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
        mountainsAndHills.SetFractalType(FastNoiseLite.FractalType.FBm);
        mountainsAndHills.SetFractalLacunarity(9);
        mountainsAndHills.SetFractalOctaves(9);
        mountainsAndHills.SetFrequency(5);
    }
}
