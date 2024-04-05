using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class NoiseFinalizer : MonoBehaviour
{
    public enum SampleMode { FromNoise, FromImage };
    public enum DrawMode { DrawHeightMap, DrawColorMap, DrawMesh };

    public DrawMode drawMode;

    public const int mapChunkSize = 241;
    [Range(0, 6)]
    public int levelOfDetail = 0;
    public float scale = 20f;

    public int octaves = 3;
    [Range(0f, 1f)]
    public float persistance = 0.5f;
    public float lacunarity = 2;

    public int seed = 1024;
    public Vector2 offset;

    public float meshHeightMultiplier = 12;
    public AnimationCurve meshHeightCurve = AnimationCurve.Linear(0f,0f,1f,1f);

    public bool autoUpdate = true;

    public Terrain[] terrain;
    public void GenerateMap()
    {
        float[,] heightMap = null;
        heightMap = NoiseMapGenerator.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, scale, octaves, persistance, lacunarity, offset);

        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];


        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = heightMap[x, y];
                for (int i = 0; i < terrain.Length; i++)
                {
                    if (currentHeight <= terrain[i].height)
                    {
                        colorMap[y * mapChunkSize + x] = terrain[i].color;

                        break;
                    }
                }
            }
        }
        DisplayMap display = FindObjectOfType<DisplayMap>();

        if (drawMode == DrawMode.DrawHeightMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
        }
        else if (drawMode == DrawMode.DrawColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.DrawMesh)
        {
            display.DrawMesh(MeshGenerator.GenerateMesh(heightMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColorMap(colorMap, mapChunkSize, mapChunkSize));
        }
    }

    public void OnValidate()
    {
        if (octaves < 0)
            octaves = 0;
        if (lacunarity < 1)
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