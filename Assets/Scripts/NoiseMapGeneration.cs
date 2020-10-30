using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Wave
{
    public float seed;
    public float frequency;
    public float amplitude;
}

public class NoiseMapGeneration : MonoBehaviour
{
    public float[,] GeneratePerlinNoiseMap(
        int mapDepth, int mapWidth, float scale, float offsetX, float offsetZ, Wave[] waves
    )
    {
        float[,] noiseMap = new float[mapDepth, mapWidth];

        for (int zIndex = 0; zIndex < mapDepth; zIndex++)
        {
            for (int xIndex = 0; xIndex < mapWidth; xIndex++)
            {
                float sampleX = (xIndex + offsetX) / scale;
                float sampleZ = (zIndex + offsetZ) / scale;

                float noise = 0f;
                float normalization = 0f;
                foreach (Wave wave in waves)
                {
                    noise += wave.amplitude * Mathf.PerlinNoise(
                        sampleX * wave.frequency + wave.seed,
                        sampleZ * wave.frequency + wave.seed
                    );
                    
                    normalization += wave.amplitude;
                }

                noise /= normalization;
                
                noiseMap[zIndex, xIndex] = noise;
            }        
        }
        return noiseMap;

    }

    public float[,] GenerateWhiteNoiseMap(
        int mapDepth, int mapWidth, float centerVertexZ, float maxDistanceZ, float offsetZ
    ) {
        float[,] noiseMap = new float[mapDepth, mapWidth];

        for (int zIndex = 0; zIndex < mapDepth; zIndex++)
        {
            float sampleZ = zIndex + offsetZ;
            float noise = Mathf.Abs(sampleZ - centerVertexZ) / maxDistanceZ;

            for (int xIndex = 0; xIndex < mapWidth; xIndex++)
            {
                noiseMap[mapDepth - zIndex - 1, xIndex] = noise;
            }
        }

        return noiseMap;
    }
}