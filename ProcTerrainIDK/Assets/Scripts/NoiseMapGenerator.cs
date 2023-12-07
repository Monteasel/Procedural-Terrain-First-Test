using Microsoft.Win32.SafeHandles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseMapGenerator
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacturnity, Vector2 offset)
    {
        if (scale <= 0)
        { 
            scale = 0.0001f;
        }

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            octaveOffsets[i].x = prng.Next(-100_000, 100_000) + offset.x;
            octaveOffsets[i].y = prng.Next(-100_000, 100_000) + offset.y;
        }

        float minNoiseHeight = float.MaxValue;
        float maxNoiseHeight = float.MinValue;

        float halfWidth = mapWidth / 2;
        float halfHeight = mapHeight / 2;

        float[,] noiseMap = new float[mapWidth, mapHeight];
        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int octave = 0; octave < octaves; octave++)
                { 
                    float sampleX = (x-halfWidth) / scale * frequency + octaveOffsets[octave].x;
                    float sampleY = (y-halfHeight) / scale * frequency + octaveOffsets[octave].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    frequency *= lacturnity;
                    amplitude *= persistance;
                }

                if(noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                else if(noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x,y] = Mathf.InverseLerp(minNoiseHeight,maxNoiseHeight, noiseMap[x,y]);
            }
        }
                return noiseMap;
    }
}
