using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public GameObject terrainChunk;

    public Transform player;
    [Range(2f, 8f)][SerializeField] private int colorFrequency = 2;

    public static Dictionary<ChunkPos, TerrainChunk> chunks = new Dictionary<ChunkPos, TerrainChunk>();

    FastNoise noise = new FastNoise();

    int chunkDistX = 3; // Distance of chunks to load in X direction
    int chunkDistZ = 6; // Distance of chunks to load in Z direction

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
        if(pooledChunks.Count > 0)//look in the poo first
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


        // GenerateTrees(chunk.blocks, xPos, zPos);

        chunk.BuildMesh();

        chunks.Add(new ChunkPos(xPos, zPos), chunk);
    }


    BlockType GetBlockType(int x, int y, int z)
    {
        // Base noise for the terrain
        float simplex1 = noise.GetSimplex(x * 0.8f, z * 0.8f) * 5;
        float simplex2 = noise.GetSimplex(x * 3f, z * 3f) * 5 * (noise.GetSimplex(x * 0.3f, z * 0.3f) + 0.5f);

        // Determine the distance from the strip
        float distanceFromStrip = Mathf.Max(0, Mathf.Abs(x) - 5);
        // Scale factor for noise (increases with distance from strip)
        float noiseScaleFactor = Mathf.Clamp(distanceFromStrip / 10f, 0, 1); // Adjust the divisor for more/less sensitivity

        // Apply scaled noise
        float heightMap = (simplex1 + simplex2) * noiseScaleFactor;

        // Flat zone condition
        if (x >= -4 && x <= 4)
        {
            if (y <= 32) // Height of the flat zone
                return BlockType.MainSurface; 
            else
                return BlockType.Air; 
        }

        // Parabolic elevation effect outside the flat zone
        float elevationEffect = distanceFromStrip * distanceFromStrip * 0.025f; // Parabolic effect

        // Combine parabolic elevation with noise
        float baseLandHeight = TerrainChunk.chunkHeight * 0.5f + heightMap + elevationEffect;

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
        // Extended range for new color zone
        if (noiseValue < -0.6)
            return BlockType.Color1;
        else if (noiseValue < -0.2)
            return BlockType.Color2;
        else if (noiseValue < 0.2)
            return BlockType.Color3;
        else if (noiseValue < 0.6)
            return BlockType.Color4;
        else
            return BlockType.Color5; // New color zone
    }

    ChunkPos curChunk = new ChunkPos(-1,-1);
    
    void LoadChunks(bool instant = false)
    {
        //the current chunk the player is in
        int curChunkPosX = Mathf.FloorToInt(player.position.x/16)*16;
        int curChunkPosZ = Mathf.FloorToInt(player.position.z/16)*16;

        //entered a new chunk
        if(curChunk.x != curChunkPosX || curChunk.z != curChunkPosZ)
        {
            curChunk.x = curChunkPosX;
            curChunk.z = curChunkPosZ;


        // Adjust the for loop to use the new chunkDistX and chunkDistZ
            for(int i = curChunkPosX - 16 * chunkDistX; i <= curChunkPosX + 16 * chunkDistX; i += 16)
                for(int j = curChunkPosZ - 16 * chunkDistZ; j <= curChunkPosZ + 16 * chunkDistZ; j += 16)
                {
                    ChunkPos cp = new ChunkPos(i, j);

                    if(!chunks.ContainsKey(cp) && !toGenerate.Contains(cp))
                    {
                        if(instant)
                            BuildChunk(i, j);
                        else
                            toGenerate.Add(cp);
                    }
                }

            //remove chunks that are too far away
            List<ChunkPos> toDestroy = new List<ChunkPos>();
            foreach (KeyValuePair<ChunkPos, TerrainChunk> c in chunks)
            {
                ChunkPos cp = c.Key;
                if (Mathf.Abs(curChunkPosX - cp.x) > 16 * (chunkDistX + 3) || Mathf.Abs(curChunkPosZ - cp.z) > 16 * (chunkDistZ + 3))
                {
                    toDestroy.Add(c.Key);
                }
            }

            //remove any up for generation
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

    // void GenerateTrees(BlockType[,,] blocks, int x, int z)
    // {
    //     System.Random rand = new System.Random(x * 10000 + z);

    //     float simplex = noise.GetSimplex(x * .8f, z * .8f);

    //     if(simplex > 0)
    //     {
    //         simplex *= 2f;
    //         int treeCount = Mathf.FloorToInt((float)rand.NextDouble() * 5 * simplex);

    //         for(int i = 0; i < treeCount; i++)
    //         {
    //             int xPos = (int)(rand.NextDouble() * 14) + 1;
    //             int zPos = (int)(rand.NextDouble() * 14) + 1;

    //             int y = TerrainChunk.chunkHeight - 1;
    //             //find the ground
    //             while(y > 0 && blocks[xPos, y, zPos] == BlockType.Air)
    //             {
    //                 y--;
    //             }
    //             y++;

    //             int treeHeight = 4 + (int)(rand.NextDouble() * 4);

    //             for(int j = 0; j < treeHeight; j++)
    //             {
    //                 if(y+j < 64)
    //                     blocks[xPos, y+j, zPos] = BlockType.MainSurface;
    //             }

    //             int leavesWidth = 1 + (int)(rand.NextDouble() * 6);
    //             int leavesHeight = (int)(rand.NextDouble() * 3);

    //             int iter = 0;
    //             for(int m = y + treeHeight - 1; m <= y + treeHeight - 1 + treeHeight; m++)
    //             {
    //                 for(int k = xPos - (int)(leavesWidth * .5)+iter/2; k <= xPos + (int)(leavesWidth * .5)-iter/2; k++)
    //                     for(int l = zPos - (int)(leavesWidth * .5)+iter/2; l <= zPos + (int)(leavesWidth * .5)-iter/2; l++)
    //                     {
    //                         if(k >= 0 && k < 16 && l >= 0 && l < 16 && m >= 0 && m < 64 && rand.NextDouble() < .8f)
    //                             blocks[k, m, l] = BlockType.MainSurface;
    //                     }

    //                 iter++;
    //             }
    //         }
    //     }
    // }

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