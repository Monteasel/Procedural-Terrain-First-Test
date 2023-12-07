using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteTerrain : MonoBehaviour
{
    public Transform viewer;
    public static Vector2 viewerPos;
    public const float maxViewDst = 450;

    int terrainChunkSize;
    int chunksVisibleInViewDst;

    Dictionary<Vector2, TerrainChunk> terrainDictionary= new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunksVisibleLastFrame = new List<TerrainChunk>();

    private void Start()
    {
        terrainChunkSize = NoiseFinalizer.mapChunkSize-1;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst/ terrainChunkSize);
    }

    private void Update()
    {
        viewerPos = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisbleChunks();
    }

    public void UpdateVisbleChunks()
    {
        for (int i = 0; i < terrainChunksVisibleLastFrame.Count; i++)
        {
            terrainChunksVisibleLastFrame[i].SetVisible(false);
        }
        terrainChunksVisibleLastFrame.Clear();

        int currentChunkX = Mathf.RoundToInt(viewerPos.x / terrainChunkSize);
        int currentChunkY = Mathf.RoundToInt(viewerPos.y / terrainChunkSize);

        for(int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for(int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkX + xOffset, currentChunkY + yOffset);

                if(terrainDictionary.ContainsKey(viewedChunkCoord))
                {
                    terrainDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    if(terrainDictionary[viewedChunkCoord].IsVisible())
                    {
                        terrainChunksVisibleLastFrame.Add(terrainDictionary[viewedChunkCoord]);
                    }
                }
                else
                {
                    terrainDictionary.Add(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, terrainChunkSize, transform));
                }
            }
        }
    }
    public class TerrainChunk
    {
        Vector2 pos;
        GameObject meshObject;

        Bounds bounds;

        public TerrainChunk(Vector2 coord, int terrainChunkSize, Transform parent)
        {
            pos = coord * terrainChunkSize;
            Vector3 posV3 = new Vector3(pos.x, 0, pos.y);
            bounds = new Bounds(pos, Vector2.one * terrainChunkSize);

            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.localPosition = posV3;
            meshObject.transform.localScale = Vector3.one * terrainChunkSize / 10f;
            meshObject.transform.parent = parent;
            SetVisible(false);
        }
        public void UpdateTerrainChunk()
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPos));
            SetVisible(viewerDstFromNearestEdge <= maxViewDst);
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }
}
