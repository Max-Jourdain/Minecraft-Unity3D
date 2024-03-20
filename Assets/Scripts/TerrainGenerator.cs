using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public GameObject terrainChunk;
    public Transform player;
    [Range(2f, 8f)][SerializeField] private int colorFrequency = 2;
    [Range(0f, 1f)][SerializeField] private float mineProbability = 0.1f; 
    public static Dictionary<ChunkPos, TerrainChunk> chunks = new Dictionary<ChunkPos, TerrainChunk>();
    FastNoise noise = new FastNoise();
    int chunkDistX = 4; // Visible chunks to the side of the player
    int chunkDistZ = 10; // Visible chunks in front of the player, reduced from 6 to 3
    ChunkPos curChunk = new ChunkPos(-1,-1);
    List<TerrainChunk> pooledChunks = new List<TerrainChunk>();
    List<ChunkPos> toGenerate = new List<ChunkPos>();

    void Start()
    {
        LoadChunks(true);
    }

    private void Update()
    {
        LoadChunks();
    }

    void BuildChunk(int xPos, int zPos)
    {
        TerrainChunk chunk;
        if(pooledChunks.Count > 0) // Look in the pool first
        {
            chunk = pooledChunks[0];
            chunk.gameObject.SetActive(true);
            pooledChunks.RemoveAt(0);
            chunk.transform.position = new Vector3(xPos, 0, zPos);
        }
        else
        {
            GameObject chunkGO = Instantiate(terrainChunk, new Vector3(xPos, 0, zPos), Quaternion.identity);
            chunk = chunkGO.GetComponent<TerrainChunk>();
        }

        for(int x = 0; x < TerrainChunk.chunkWidth+2; x++)
            for(int z = 0; z < TerrainChunk.chunkWidth+2; z++)
                for(int y = 0; y < TerrainChunk.chunkHeight; y++)
                {
                    chunk.blocks[x, y, z] = GetBlockType(xPos+x-1, y, zPos+z-1);
                }

        chunk.BuildMesh();
        chunks.Add(new ChunkPos(xPos, zPos), chunk);
    }

    BlockType GetBlockType(int x, int y, int z)
    {
        // Base noise for the terrain
        float simplex1 = noise.GetSimplex(x * 0.8f, z * 0.8f) * 5;
        float simplex2 = noise.GetSimplex(x * 3f, z * 3f) * 5 * (noise.GetSimplex(x * 0.3f, z * 0.3f) + 0.5f);

        // Determine the distance from the strip
        float distanceFromStrip = Mathf.Max(0, Mathf.Abs(x) - 6);
        // Scale factor for noise (increases with distance from strip)
        float noiseScaleFactor = Mathf.Clamp(distanceFromStrip / 10f, 0, 1); // Adjust the divisor for more/less sensitivity

        // Apply scaled noise
        float heightMap = (simplex1 + simplex2) * noiseScaleFactor;

        if (x >= -4 && x <= 4)
        {
            if (y == 24 && Random.value < mineProbability && z >= 3) 
            {
                return BlockType.Mine;
            }
            else if (y == 24 && z <= 2)
            {
                return GetColorBasedBlockType(noise.GetSimplex(x * colorFrequency, z * colorFrequency));
            } 
            else if (y == 24)
            {
                return BlockType.Unplayed;
            }
            else if (y < 24) 
            {
                return BlockType.Color1;
            }
            else
            {
                return BlockType.Air;
            }
        }


        // Parabolic elevation effect outside the flat zone
        float elevationEffect = distanceFromStrip * distanceFromStrip * 0.025f; // Parabolic effect

        // Combine parabolic elevation with noise
        float baseLandHeight = TerrainChunk.chunkHeight * 0.4f + heightMap + elevationEffect;

        float noiseValue = noise.GetSimplex(x * colorFrequency, z * colorFrequency);

        BlockType blockType = BlockType.Air;
        if (y <= baseLandHeight)
        {
            blockType = GetColorBasedBlockType(noiseValue);
        }

        return blockType;
    }


    BlockType GetColorBasedBlockType(float noiseValue)
    {
        if (noiseValue < -0.6) return BlockType.Color1;
        else if (noiseValue < -0.2) return BlockType.Color2;
        else if (noiseValue < 0.2) return BlockType.Color3;
        else if (noiseValue < 0.6) return BlockType.Color4;
        else return BlockType.Color5;
    }
    
    void LoadChunks(bool instant = false)
    {
        int chunkSize = 16;
        int curChunkPosX = Mathf.FloorToInt((player.position.x + chunkSize / 2) / chunkSize) * chunkSize;
        int curChunkPosZ = Mathf.FloorToInt((player.position.z + chunkSize / 2) / chunkSize) * chunkSize;

        Debug.DrawRay(new Vector3(curChunkPosX, 0, curChunkPosZ), Vector3.up * 100, Color.red);

        if(curChunk.x != curChunkPosX || curChunk.z != curChunkPosZ)
        {
            curChunk.x = curChunkPosX;
            curChunk.z = curChunkPosZ;

            for(int i = curChunkPosX - chunkSize * chunkDistX; i <= curChunkPosX + chunkSize * chunkDistX; i += chunkSize)
            {
                // Load chunks only in front of the player
                for(int j = curChunkPosZ; j <= curChunkPosZ + chunkSize * chunkDistZ; j += chunkSize)
                {
                    ChunkPos cp = new ChunkPos(i, j);

                    if(!chunks.ContainsKey(cp) && !toGenerate.Contains(cp))
                    {
                        if(instant) BuildChunk(i, j);
                        else toGenerate.Add(cp);
                    }
                }
            }

            // Remove chunks that are too far away
            List<ChunkPos> toDestroy = new List<ChunkPos>();
            foreach (KeyValuePair<ChunkPos, TerrainChunk> c in chunks)
            {
                ChunkPos cp = c.Key;
                if (Mathf.Abs(curChunkPosX - cp.x) > 16 * (chunkDistX + 3) || Mathf.Abs(curChunkPosZ - cp.z) > 16 * (chunkDistZ + 3))
                {
                    toDestroy.Add(c.Key);
                }
            }

            foreach(ChunkPos cp in toGenerate)
            {
                if(Mathf.Abs(curChunkPosX - cp.x) > 16 * (chunkDistX + 3) || Mathf.Abs(curChunkPosZ - cp.z) > 16 * (chunkDistZ + 3))
                    toGenerate.Remove(cp);
            }

            foreach(ChunkPos cp in toDestroy)
            {
                chunks[cp].gameObject.SetActive(false);
                pooledChunks.Add(chunks[cp]);
                chunks.Remove(cp);
            }

            StartCoroutine(DelayBuildChunks());
        }
    }

    IEnumerator DelayBuildChunks()
    {
        while(toGenerate.Count > 0)
        {
            BuildChunk(toGenerate[0].x, toGenerate[0].z);
            toGenerate.RemoveAt(0);

            yield return new WaitForSeconds(.2f);
        }
    }
}

public struct ChunkPos
{
    public int x, z;
    public ChunkPos(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
}