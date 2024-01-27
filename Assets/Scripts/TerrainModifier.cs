using UnityEngine;

public class TerrainModifier : MonoBehaviour
{
    public LayerMask groundLayer;
    public Camera playerCamera;
    public float rayLength = 400;

    private BlockType selectedColor = BlockType.Color1; // Default color

    void Update()
    {
        CheckForColorChangeInput(); // Check if a number key is pressed

        if (Input.GetMouseButtonDown(0)) // Left mouse click
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, rayLength, groundLayer))
            {
                ProcessHit(hit.point);
            }
        }
    }

    private void CheckForColorChangeInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedColor = BlockType.Num1;
        else if (Input.GetKeyDown(KeyCode.Alpha2)) selectedColor = BlockType.Num2;
        else if (Input.GetKeyDown(KeyCode.Alpha3)) selectedColor = BlockType.Color3;
        else if (Input.GetKeyDown(KeyCode.Alpha4)) selectedColor = BlockType.Color4;
        else if (Input.GetKeyDown(KeyCode.Alpha5)) selectedColor = BlockType.Color5;
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
                UpdateChunkBlock(chunk, blockPos, chunkPos);
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
        int halfStripSize = TerrainGenerator.stripSize / 2;
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

        if (chunk.blocks[localX, blockPos.y, localZ] != BlockType.Air)
        {
            chunk.blocks[localX, blockPos.y - 1, localZ] = selectedColor; // Use selected color
        }
        else
        {
            chunk.blocks[localX, blockPos.y - 1, localZ] = selectedColor; // Use selected color
        }

        chunk.BuildMesh();
    }
}
