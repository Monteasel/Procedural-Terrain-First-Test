using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class NoiseFinalizer : MonoBehaviour
{
    public enum SampleMode { FromNoise, FromImage };
    public enum DrawMode { DrawHeightMap, DrawColorMap, Whitebeard, DrawMesh };

    public SampleMode sampleMode;
    public Texture2D imageSample;
    public DrawMode drawMode;

    public const int mapChunkSize = 241;
    [Range(0,6)]
    public int levelOfDetail;
    public float scale;

    public int octaves;
    [Range(0f, 1f)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public Terrain[] terrain;
    public void GenerateMap()
    {
        float[,] heightMap = null;
        if (sampleMode == SampleMode.FromNoise)
        {
            heightMap = NoiseMapGenerator.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, scale, octaves, persistance, lacunarity, offset);
        }
        else if(sampleMode == SampleMode.FromImage)
        {
            float[,] imageHeightMap = new float[imageSample.width, imageSample.height];
            for(int y = 0; y < imageSample.height; y++)
            {
                for(int x = 0; x < imageSample.width; x++)
                {
                    imageHeightMap[x, y] = imageSample.GetPixel(x, y).r;
                }
            }
            heightMap = imageHeightMap;
        }

        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

        Color[] terrainTextureColorMap = new Color[mapChunkSize * mapChunkSize];

        /*for(int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = heightMap[x, y];
                for (int i = 0; i < terrain.Length; i++)
                {
                    if (currentHeight <= terrain[i].height)
                    {
                        colorMap[y * mapChunkSize + x] = terrain[i].color;

                        terrainTextureColorMap[y * mapChunkSize + x] = terrain[i].texture.GetPixel(x,y);
                        
                        break;
                    }
                }
            }
        }*/
        DisplayMap display = FindObjectOfType<DisplayMap>();

        if(drawMode == DrawMode.DrawHeightMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        }
        else if(drawMode == DrawMode.DrawColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }
        else if(drawMode == DrawMode.Whitebeard)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(terrainTextureColorMap, mapChunkSize, mapChunkSize));
        }
        else if(drawMode == DrawMode.DrawMesh)
        {
            display.DrawMesh(MeshGenerator.GenerateMesh(heightMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }
    }

    public void OnValidate()
    {
        if (octaves < 0)
            octaves = 0;
        if(lacunarity < 1)
            lacunarity = 1;
    }

    [System.Serializable]
    public struct Terrain
    {
        public string name;
        public float height;
        public Color color;
        public Texture2D texture;
    }
}
