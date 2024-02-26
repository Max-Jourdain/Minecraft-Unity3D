using UnityEngine;
using System.Collections.Generic;

public class TerrainModifier : MonoBehaviour
{
    public LayerMask groundLayer;
    public Camera playerCamera;
    public float rayLength = 400;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse click
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, rayLength, groundLayer))
            {
                ProcessHit(hit.point);
            }
        }
    }

    private void ProcessHit(Vector3 hitPoint)
    {
        Vector3 adjustedHitPoint = hitPoint + new Vector3(0f, 0.01f, 0f); // Slightly adjust the hit point to ensure it's inside the block
        Vector3Int blockPos = Vector3Int.FloorToInt(adjustedHitPoint); // Convert the hit point to a block position

        ChunkPos chunkPos = GetChunkPosition(blockPos); // Get the chunk position from the block position
        if (TerrainGenerator.chunks.TryGetValue(chunkPos, out TerrainChunk chunk)) // Check if the chunk exists
        {
            int localX = blockPos.x - chunkPos.x + 1, localZ = blockPos.z - chunkPos.z + 1; // Calculate local block positions within the chunk
            
            // Check if the block is unplayed before starting flood fill
            if (chunk.blocks[localX, blockPos.y - 1, localZ] == BlockType.Mine)
            {
                Debug.Log("Game Over");
            }
            else if (chunk.blocks[localX, blockPos.y - 1, localZ] == BlockType.Unplayed) // Ensure the block is unplayed
            {
                FloodFill(blockPos, localX, localZ); // Start flood fill from this block
            }
            else
            {
                Debug.Log("Block already played");
            }
        }
    }

    private void FloodFill(Vector3Int blockPos, int localX, int localZ)
    {
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        // Convert local positions back to world positions before enqueuing
        ChunkPos initialChunkPos = GetChunkPosition(blockPos);
        Vector3Int worldPos = new Vector3Int(localX + initialChunkPos.x, blockPos.y, localZ + initialChunkPos.z);
        queue.Enqueue(worldPos);

        while (queue.Count > 0)
        {
            Vector3Int currentPos = queue.Dequeue();
            ChunkPos currentChunkPos = GetChunkPosition(currentPos);
            TerrainChunk currentChunk;

            if (!TerrainGenerator.chunks.TryGetValue(currentChunkPos, out currentChunk)) continue;

            int x = currentPos.x - currentChunkPos.x + 1;
            int z = currentPos.z - currentChunkPos.z + 1;

            // Skip already processed or invalid blocks
            if (!IsValidBlock(currentChunk, x, currentPos.y, z)) continue;

            // Process current block
            int mineCount = CountMines(currentChunk, x, currentPos.y, z, currentChunkPos);

            // Update current block based on mine count
            if (mineCount == 0)
            {
                currentChunk.blocks[x, currentPos.y - 1, z] = BlockType.Played;
                // Enqueue neighbors
                EnqueueNeighbors(queue, currentPos);
            }
            else
            {
                currentChunk.blocks[x, currentPos.y - 1, z] = (BlockType)mineCount - 1;
            }

            currentChunk.BuildMesh();
        }
    }

    private void EnqueueNeighbors(Queue<Vector3Int> queue, Vector3Int currentPos)
    {
        // Enqueue all eight neighboring blocks
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                if (dx == 0 && dz == 0) continue; // Skip the current block
                Vector3Int neighborPos = new Vector3Int(currentPos.x + dx, currentPos.y, currentPos.z + dz);
                queue.Enqueue(neighborPos);
            }
        }
    }

    private bool IsValidBlock(TerrainChunk chunk, int x, int y, int z)
    {
        return x >= 1 && x <= TerrainChunk.chunkWidth && z >= 1 && z <= TerrainChunk.chunkWidth && chunk.blocks[x, y - 1, z] == BlockType.Unplayed;
    }

    private int CountMines(TerrainChunk chunk, int x, int y, int z, ChunkPos chunkPos)
    {
        int mineCount = 0;
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                if (dx == 0 && dz == 0) continue; // Skip the current block

                int neighborX = x + dx;
                int neighborZ = z + dz;
                ChunkPos neighborChunkPos = GetChunkPosition(new Vector3Int(neighborX + chunkPos.x - 1, y, neighborZ + chunkPos.z - 1));
                TerrainChunk neighborChunk;

                if (TerrainGenerator.chunks.TryGetValue(neighborChunkPos, out neighborChunk))
                {
                    int localX = (neighborX - 1) % TerrainChunk.chunkWidth + 1;
                    int localZ = (neighborZ - 1) % TerrainChunk.chunkWidth + 1;

                    // Adjust localX and localZ for edge cases
                    if (localX <= 0) localX += TerrainChunk.chunkWidth;
                    if (localZ <= 0) localZ += TerrainChunk.chunkWidth;
                    if (localX > TerrainChunk.chunkWidth) localX -= TerrainChunk.chunkWidth;
                    if (localZ > TerrainChunk.chunkWidth) localZ -= TerrainChunk.chunkWidth;

                    if (neighborChunk.blocks[localX, y - 1, localZ] == BlockType.Mine)
                    {
                        mineCount++;
                    }
                }
            }
        }
        return mineCount;
    }

    private ChunkPos GetChunkPosition(Vector3Int blockPos)
    {
        // Calculate the chunk position based on the block position
        return new ChunkPos(Mathf.FloorToInt(blockPos.x / (float)TerrainChunk.chunkWidth) * TerrainChunk.chunkWidth,
                            Mathf.FloorToInt(blockPos.z / (float)TerrainChunk.chunkWidth) * TerrainChunk.chunkWidth);
    }
}
