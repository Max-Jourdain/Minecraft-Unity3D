using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections;

public class TerrainModifier : MonoBehaviour
{
    public LayerMask groundLayer;
    public Camera playerCamera;
    public float rayLength = 400;
    public Dictionary<Vector3Int, BlockType> originalBlockStates = new Dictionary<Vector3Int, BlockType>();
    public bool hasFirstClickOccurred = false;
    public bool isGameOver = false;
    [SerializeField] private int score = 0;
    [SerializeField] private TMP_Text scorText;
    TerrainGenerator _terrainGenerator;
    private float touchStartTime;
    private float holdThreshold = 0.25f;
    private bool hasVibrated = false;

    void Awake()
    {
        _terrainGenerator = FindObjectOfType<TerrainGenerator>();
    }

    void Update()
    {
        if (isGameOver) return;

        #if UNITY_EDITOR
        HandleEditorInput();
        #else
        HandleMobileInput();
        #endif
    }

    private void HandleEditorInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastAndProcess(Input.mousePosition, false);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            RaycastAndProcess(Input.mousePosition, true);
        }
    }

    private void HandleMobileInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartTime = Time.time;
                hasVibrated = false; // Reset vibration flag when a new touch begins
            }
            else if (touch.phase == TouchPhase.Stationary && Time.time - touchStartTime > holdThreshold && !hasVibrated)
            {
                Handheld.Vibrate();
                hasVibrated = true; // Ensure vibration happens only once per hold
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                float touchDuration = Time.time - touchStartTime;

                if (touchDuration < holdThreshold)
                {
                    RaycastAndProcess(touch.position, false); // Quick tap
                }
                else
                {
                    RaycastAndProcess(touch.position, true); // Hold
                }

                hasVibrated = false; // Reset for the next touch
            }
        }
    }

    private void RaycastAndProcess(Vector3 mousePosition, bool isRightClick)
    {
        Ray ray = playerCamera.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, rayLength, groundLayer))
        {
            if (isRightClick)
            {
                ProcessRightClick(hit.point);
            }
            else
            {
                ProcessHit(hit.point);
            }
        }
    }

    private void ProcessRightClick(Vector3 hitPoint)
    {
        if (!hasFirstClickOccurred) return;

        Vector3 adjustedHitPoint = hitPoint + new Vector3(0f, 0.01f, 0f);
        Vector3Int blockPos = Vector3Int.FloorToInt(adjustedHitPoint);

        ChunkPos chunkPos = GetChunkPosition(blockPos);
        if (_terrainGenerator.chunks.TryGetValue(chunkPos, out TerrainChunk chunk))
        {
            int localX = blockPos.x - chunkPos.x;
            int localZ = blockPos.z - chunkPos.z;

            Vector3Int localPos = new Vector3Int(localX + 1, blockPos.y - 1, localZ + 1);
            BlockType currentBlock = chunk.blocks[localPos.x, localPos.y, localPos.z];

            if (currentBlock == BlockType.Flag)
            {
                // If the block is flagged, unflag it and restore its original state
                if (originalBlockStates.TryGetValue(blockPos, out BlockType originalState))
                {
                    chunk.blocks[localPos.x, localPos.y, localPos.z] = originalState;
                    originalBlockStates.Remove(blockPos); // Remove the entry as it's no longer needed
                }
            }
            else if (currentBlock == BlockType.Mine || currentBlock == BlockType.Unplayed)
            {
                // If the block is a mine or unplayed, flag it and remember its original state
                originalBlockStates[blockPos] = currentBlock; // Remember the original state
                chunk.blocks[localPos.x, localPos.y, localPos.z] = BlockType.Flag;
            }

            chunk.BuildMesh();
        }
    }

    private void ProcessHit(Vector3 hitPoint)
    {
        Vector3 adjustedHitPoint = hitPoint + new Vector3(0f, 0.01f, 0f); // Adjust the hit point to ensure it's inside the block
        Vector3Int blockPos = Vector3Int.FloorToInt(adjustedHitPoint); // Convert the hit point to a block position

        ChunkPos chunkPos = GetChunkPosition(blockPos); // Get the chunk position from the block position
        if (_terrainGenerator.chunks.TryGetValue(chunkPos, out TerrainChunk chunk)) // Check if the chunk exists
        {
            int localX = blockPos.x - chunkPos.x;
            int localZ = blockPos.z - chunkPos.z;

            if (!hasFirstClickOccurred)
            {
                hasFirstClickOccurred = true;
                MakeFirstClickSafe(blockPos, localX, localZ, chunk);
                FloodFill(blockPos, localX, localZ);
            }
            else
            {
                // Normal game logic for handling clicks after the first one
                if (chunk.blocks[localX + 1, blockPos.y - 1, localZ + 1] == BlockType.Mine)
                {
                    isGameOver = true;
                    Block.UpdateTile(BlockType.Mine, Tile.Mine);
                    UpdateVisibleChunks();
                }
                else if (chunk.blocks[localX + 1, blockPos.y - 1, localZ + 1] == BlockType.Unplayed) 
                {
                    FloodFill(blockPos, localX, localZ);
                }
            }
        }
    }

    private void UpdateVisibleChunks()
    {
        foreach (TerrainChunk chunk in _terrainGenerator.chunks.Values)
        {
            chunk.BuildMesh();
        }
    }

    private void MakeFirstClickSafe(Vector3Int blockPos, int localX, int localZ, TerrainChunk chunk)
    {
        int safeRadius = 1; 

        for (int dx = -safeRadius; dx <= safeRadius; dx++)
        {
            for (int dz = -safeRadius; dz <= safeRadius; dz++)
            {
                // Calculate the global position of the neighbor block.
                Vector3Int neighborPos = new Vector3Int(blockPos.x + dx, blockPos.y, blockPos.z + dz);
                ChunkPos neighborChunkPos = GetChunkPosition(neighborPos);
                TerrainChunk neighborChunk;
                if (_terrainGenerator.chunks.TryGetValue(neighborChunkPos, out neighborChunk))
                {
                    int localNeighborX = neighborPos.x - neighborChunkPos.x + 1;
                    int localNeighborZ = neighborPos.z - neighborChunkPos.z + 1;

                    if (neighborChunk.blocks[localNeighborX, neighborPos.y - 1, localNeighborZ] == BlockType.Mine)
                    {
                        neighborChunk.blocks[localNeighborX, neighborPos.y - 1, localNeighborZ] = BlockType.Unplayed;
                        neighborChunk.BuildMesh();
                    }
                }
            }
        }
    }
    private void FloodFill(Vector3Int blockPos, int localX, int localZ)
    {
        StartCoroutine(FloodFillCoroutine(blockPos, localX, localZ));
    }

    private IEnumerator FloodFillCoroutine(Vector3Int blockPos, int localX, int localZ)
    {
        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        HashSet<ChunkPos> affectedChunks = new HashSet<ChunkPos>(); // Track affected chunks
        Queue<Vector3Int> queue = new Queue<Vector3Int>();

        ChunkPos initialChunkPos = GetChunkPosition(blockPos);
        Vector3Int worldPos = new Vector3Int(localX + initialChunkPos.x, blockPos.y, localZ + initialChunkPos.z);
        queue.Enqueue(worldPos);
        visited.Add(worldPos);

        int floodFillScore = 0; // Score for the flood fill
        float delay = 0.030f; // Adjust the delay as needed

        // Layers of blocks to be processed
        List<Vector3Int> currentLayer = new List<Vector3Int> { worldPos };

        while (currentLayer.Count > 0)
        {
            List<Vector3Int> nextLayer = new List<Vector3Int>();

            foreach (var currentPos in currentLayer)
            {
                ChunkPos currentChunkPos = GetChunkPosition(currentPos);
                TerrainChunk currentChunk;
                if (!_terrainGenerator.chunks.TryGetValue(currentChunkPos, out currentChunk)) continue;

                int x = currentPos.x - currentChunkPos.x + 1;
                int z = currentPos.z - currentChunkPos.z + 1;

                if (!IsValidBlock(currentChunk, x, currentPos.y, z)) continue;

                // Add the chunk to the affectedChunks set to ensure it gets updated
                affectedChunks.Add(currentChunkPos);

                int mineCount = CountMines(currentChunk, x, currentPos.y, z, currentChunkPos);

                if (mineCount == 0)
                {
                    currentChunk.blocks[x, currentPos.y - 1, z] = BlockType.Played;
                    foreach (var neighbor in GetNeighbors(currentPos))
                    {
                        if (!visited.Contains(neighbor))
                        {
                            nextLayer.Add(neighbor);
                            visited.Add(neighbor);
                        }
                    }
                }
                else
                {
                    currentChunk.blocks[x, currentPos.y - 1, z] = (BlockType)mineCount - 1;
                }

                // Increment score for each block
                floodFillScore++;

                // Update the mesh for the current chunk
                currentChunk.BuildMesh();
            }

            // Move to the next layer
            currentLayer = nextLayer;

            // Yield execution to create the delay effect
            yield return new WaitForSeconds(delay);
        }

        // Update the score
        score += floodFillScore;
        scorText.text = "Score: " + score;

        // After processing all blocks, update the mesh for each affected chunk
        foreach (var chunkPos in affectedChunks)
        {
            if (_terrainGenerator.chunks.TryGetValue(chunkPos, out TerrainChunk chunk))
            {
                chunk.BuildMesh();
            }
        }
    }

    private IEnumerable<Vector3Int> GetNeighbors(Vector3Int pos)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                if (dx == 0 && dz == 0) continue;
                yield return new Vector3Int(pos.x + dx, pos.y, pos.z + dz);
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
                Vector3Int neighborGlobalPos = new Vector3Int(neighborX + chunkPos.x - 1, y, neighborZ + chunkPos.z - 1);
                ChunkPos neighborChunkPos = GetChunkPosition(neighborGlobalPos);
                TerrainChunk neighborChunk;

                if (_terrainGenerator.chunks.TryGetValue(neighborChunkPos, out neighborChunk))
                {
                    int localX = neighborGlobalPos.x - neighborChunkPos.x + 1;
                    int localZ = neighborGlobalPos.z - neighborChunkPos.z + 1;

                    // Ensure localX and localZ are within bounds
                    if (localX < 1 || localX > TerrainChunk.chunkWidth || localZ < 1 || localZ > TerrainChunk.chunkWidth)
                        continue;

                    BlockType neighborBlockType = neighborChunk.blocks[localX, y - 1, localZ];
                    if (neighborBlockType == BlockType.Mine)
                    {
                        mineCount++;
                    }
                    else if (neighborBlockType == BlockType.Flag)
                    {
                        if (originalBlockStates.TryGetValue(neighborGlobalPos, out BlockType originalState) && originalState == BlockType.Mine)
                        {
                            mineCount++;
                        }
                    }
                }
            }
        }
        return mineCount;
    }

    public ChunkPos GetChunkPosition(Vector3Int blockPos)
    {
        // Calculate the chunk position based on the block position
        return new ChunkPos(Mathf.FloorToInt(blockPos.x / (float)TerrainChunk.chunkWidth) * TerrainChunk.chunkWidth,
                            Mathf.FloorToInt(blockPos.z / (float)TerrainChunk.chunkWidth) * TerrainChunk.chunkWidth);
    }
}