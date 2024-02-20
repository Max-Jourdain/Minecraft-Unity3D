using UnityEngine;

public class TerrainModifier : MonoBehaviour
{
    public LayerMask groundLayer;
    public Camera playerCamera;
    public float rayLength = 400;
    private BlockType selectedColor = BlockType.Color5; // Default color

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
        const int maxHeightForColorChange = 32;
        Vector3Int blockPos = Vector3Int.FloorToInt(hitPoint);

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
        else Debug.Log($"Chunk not found at position: {chunkPos}");
    }

    private bool IsWithinStripBounds(int x) => Mathf.Abs(x) <= 4;

    private ChunkPos GetChunkPosition(Vector3Int blockPos)
    {
        return new ChunkPos(Mathf.FloorToInt(blockPos.x / (float)TerrainChunk.chunkWidth) * TerrainChunk.chunkWidth,
                            Mathf.FloorToInt(blockPos.z / (float)TerrainChunk.chunkWidth) * TerrainChunk.chunkWidth);
    }

    private void UpdateChunkAndNeighbors(TerrainChunk chunk, Vector3Int blockPos, ChunkPos chunkPos, int localX, int localZ)
    {
        chunk.blocks[localX, blockPos.y - 1, localZ] = BlockType.Color3;
        chunk.BuildMesh();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                if (dx == 0 && dz == 0) continue;
                UpdateNeighbor(chunk, blockPos, chunkPos, localX + dx, localZ + dz);
            }
        }
    }

    private void UpdateNeighbor(TerrainChunk chunk, Vector3Int blockPos, ChunkPos chunkPos, int neighborX, int neighborZ)
    {
        if (neighborX >= 1 && neighborX <= TerrainChunk.chunkWidth && neighborZ >= 1 && neighborZ <= TerrainChunk.chunkWidth)
        {
            Debug.Log($"Same chunk neighbor type: {chunk.blocks[neighborX, blockPos.y - 1, neighborZ]}");
            chunk.blocks[neighborX, blockPos.y - 1, neighborZ] = selectedColor;
            chunk.BuildMesh();
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

        // Attempt to find the neighbor chunk and update the corresponding block if found.
        if (TerrainGenerator.chunks.TryGetValue(neighborChunkPos, out TerrainChunk neighborChunk))
        {
            neighborChunk.blocks[targetX, blockPos.y - 1, targetZ] = selectedColor;
            neighborChunk.BuildMesh();
            Debug.Log($"Updated block in neighbor chunk at {neighborChunkPos} to {selectedColor}");
        }
        else
        {
            Debug.Log($"Neighbor chunk not found at position: {neighborChunkPos}");
        }
    }
}