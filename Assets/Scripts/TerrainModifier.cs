using UnityEngine;

public class TerrainModifier : MonoBehaviour
{
    public LayerMask groundLayer;
    public Camera playerCamera;
    public float rayLength = 400;

    private int mineCount = 0;

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
        const int maxHeightForColorChange = 25;

        // Adjust hitPoint slightly to ensure correct flooring, especially for y-coordinate
        Vector3 adjustedHitPoint = hitPoint + new Vector3(0f, 0.01f, 0f);
        Vector3Int blockPos = Vector3Int.FloorToInt(adjustedHitPoint);

        if (blockPos.y > maxHeightForColorChange || !IsWithinStripBounds(blockPos.x)) return;

        ChunkPos chunkPos = GetChunkPosition(blockPos);
        if (TerrainGenerator.chunks.TryGetValue(chunkPos, out TerrainChunk chunk))
        {
            int localX = blockPos.x - chunkPos.x + 1, localZ = blockPos.z - chunkPos.z + 1;
            BlockType currentBlock = chunk.blocks[localX, blockPos.y - 1, localZ];

            if (currentBlock == BlockType.Mine)
            {
                Debug.Log("Game Over");
                return;
            }

            UpdateChunkAndNeighbors(chunk, blockPos, chunkPos, localX, localZ);
        }
    }

    private bool IsWithinStripBounds(int x) => Mathf.Abs(x) <= 4;

    private ChunkPos GetChunkPosition(Vector3Int blockPos)
    {
        return new ChunkPos(Mathf.FloorToInt(blockPos.x / (float)TerrainChunk.chunkWidth) * TerrainChunk.chunkWidth,
                            Mathf.FloorToInt(blockPos.z / (float)TerrainChunk.chunkWidth) * TerrainChunk.chunkWidth);
    }

    private void UpdateChunkAndNeighbors(TerrainChunk chunk, Vector3Int blockPos, ChunkPos chunkPos, int localX, int localZ)
    {
        mineCount = 0;

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                if (dx == 0 && dz == 0) continue;
                UpdateNeighbor(chunk, blockPos, chunkPos, localX + dx, localZ + dz);

            }
        }

        if (mineCount == 0) 
        {
            chunk.blocks[localX, blockPos.y - 1, localZ] = BlockType.Air;
            // Recursively update neighbors
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    if (dx == 0 && dz == 0) continue;
                    Vector3Int neighborPos = new Vector3Int(blockPos.x + dx, blockPos.y, blockPos.z + dz);
                    ChunkPos neighborChunkPos = GetChunkPosition(neighborPos);
                    if (TerrainGenerator.chunks.TryGetValue(neighborChunkPos, out TerrainChunk neighborChunk))
                    {
                        int localNeighborX = neighborPos.x - neighborChunkPos.x + 1, localNeighborZ = neighborPos.z - neighborChunkPos.z + 1;
                        UpdateChunkAndNeighbors(neighborChunk, neighborPos, neighborChunkPos, localNeighborX, localNeighborZ);
                    }
                }
            }
        }

        else if (mineCount == 1) chunk.blocks[localX, blockPos.y - 1, localZ] = BlockType.Num1;
        else if (mineCount == 2) chunk.blocks[localX, blockPos.y - 1, localZ] = BlockType.Num2;

        chunk.BuildMesh();

        Debug.Log("Total Mine Count: " + mineCount);
    }

    private void UpdateNeighbor(TerrainChunk chunk, Vector3Int blockPos, ChunkPos chunkPos, int neighborX, int neighborZ)
    {
        if (neighborX >= 1 && neighborX <= TerrainChunk.chunkWidth && neighborZ >= 1 && neighborZ <= TerrainChunk.chunkWidth)
        {
            if (chunk.blocks[neighborX, blockPos.y - 1, neighborZ] == BlockType.Mine)
            {
                mineCount++;
            }
        }
        else HandleEdgeCases(chunkPos, blockPos, neighborX, neighborZ);
    }

    private void HandleEdgeCases(ChunkPos chunkPos, Vector3Int blockPos, int neighborX, int neighborZ)
    {
        // Determine the offset for neighbor chunks based on the direction of the neighbor block.
        int chunkOffsetX = 0, chunkOffsetZ = 0;

        // Determine if the neighbor block is beyond the current chunk boundaries (edge or corner).
        if (neighborX < 1 || neighborX > TerrainChunk.chunkWidth)
        {
            chunkOffsetX = neighborX < 1 ? -TerrainChunk.chunkWidth : TerrainChunk.chunkWidth;
        }
        if (neighborZ < 1 || neighborZ > TerrainChunk.chunkWidth)
        {
            chunkOffsetZ = neighborZ < 1 ? -TerrainChunk.chunkWidth : TerrainChunk.chunkWidth;
        }

        // Calculate the position of the neighbor chunk taking into account the current chunk position and the determined offsets.
        ChunkPos neighborChunkPos = new ChunkPos(chunkPos.x + chunkOffsetX, chunkPos.z + chunkOffsetZ);

        int targetX = (neighborX < 1 || neighborX > TerrainChunk.chunkWidth) ? (neighborX < 1 ? TerrainChunk.chunkWidth : 1) : neighborX - chunkOffsetX;
        int targetZ = (neighborZ < 1 || neighborZ > TerrainChunk.chunkWidth) ? (neighborZ < 1 ? TerrainChunk.chunkWidth : 1) : neighborZ - chunkOffsetZ;

        // Attempt to find the neighbor chunk
        if (TerrainGenerator.chunks.TryGetValue(neighborChunkPos, out TerrainChunk neighborChunk))
        {
            if (neighborChunk.blocks[targetX, blockPos.y - 1, targetZ] == BlockType.Mine)
            {
                mineCount++;
            }
        }
    }
}