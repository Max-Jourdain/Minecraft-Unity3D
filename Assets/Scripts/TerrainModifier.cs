using UnityEngine;
using System.Collections.Generic;

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
        const int maxHeightForColorChange = 32; // Set the max height for color change
        Vector3Int blockPos = ConvertToBlockPosition(hitPoint);

        // Allow color change for blocks at or below maxHeightForColorChange
        if (blockPos.y > maxHeightForColorChange) return; 

        if (IsWithinStripBounds(blockPos.x))
        {
            ChunkPos chunkPos = GetChunkPosition(blockPos);
            if (TerrainGenerator.chunks.TryGetValue(chunkPos, out TerrainChunk chunk))
            {
                // ! Game Over if the block is a mine
                if (chunk.blocks[blockPos.x - chunkPos.x + 1, blockPos.y - 1, blockPos.z - chunkPos.z + 1] == BlockType.Mine)
                {
                    Debug.Log("Game Over");
                }
                else
                {
                    CountSurroundingMines(blockPos, chunk, chunkPos);
                    UpdateChunkBlock(chunk, blockPos, chunkPos);
                }
            }
            else
            {
                Debug.Log($"Chunk not found at position: {chunkPos}");
            }
        }
    }

    private Vector3Int ConvertToBlockPosition(Vector3 hitPoint)
    {
        return new Vector3Int(
            Mathf.FloorToInt(hitPoint.x),
            Mathf.FloorToInt(hitPoint.y),
            Mathf.FloorToInt(hitPoint.z));
    }

    private bool IsWithinStripBounds(int x)
    {
        int halfStripSize = 8 / 2;
        return x >= -halfStripSize && x <= halfStripSize;
    }

    private ChunkPos GetChunkPosition(Vector3Int blockPos)
    {
        return new ChunkPos(
            Mathf.FloorToInt(blockPos.x / (float)TerrainChunk.chunkWidth) * TerrainChunk.chunkWidth,
            Mathf.FloorToInt(blockPos.z / (float)TerrainChunk.chunkWidth) * TerrainChunk.chunkWidth);
    }

    private void UpdateChunkBlock(TerrainChunk chunk, Vector3Int blockPos, ChunkPos chunkPos)
    {
        int localX = blockPos.x - chunkPos.x + 1;
        int localZ = blockPos.z - chunkPos.z + 1;

        // Update the block in the current chunk
        chunk.blocks[localX, blockPos.y - 1, localZ] = selectedColor;
        chunk.BuildMesh();
    }

    private int CountSurroundingMines(Vector3Int blockPos, TerrainChunk chunk, ChunkPos chunkPos)
    {
        int mineCount = 0;
        // Iterate through a 3x3 grid centered on the clicked block
        for (int x = -1; x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0) continue; // Skip the clicked block itself

                int neighborX = blockPos.x + x - chunkPos.x + 1;
                int neighborZ = blockPos.z + z - chunkPos.z + 1;

                // Check if the neighbor is within chunk bounds
                if (neighborX >= 0 && neighborX < TerrainChunk.chunkWidth && neighborZ >= 0 && neighborZ < TerrainChunk.chunkWidth)
                {
                    // Check if the neighboring block is a mine
                    if (chunk.blocks[neighborX, blockPos.y - 1, neighborZ] == BlockType.Mine)
                    {
                        mineCount++;
                    }
                }
            }
        }

        Debug.Log($"Mine count: {mineCount}");
        return mineCount;
    }
}
