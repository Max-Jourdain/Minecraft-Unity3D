using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Block Types")]
    public GameObject blockPrefab;

    [Header("Chunk Settings")]
    public int chunkRows = 9; // X
    public int chunkColumns = 30; // Y
    [Range(0.10f, 0.25f)] public float minePercentage = 0.1f;
    public bool isFirstChunkMinesActive = false;

    [Header("Private Variables")]
    public int chunkNumber = 0;
    public Dictionary<Vector2, ChunkData> chunks = new Dictionary<Vector2, ChunkData>();
    public Dictionary<Vector2Int, Cell> generatedCells = new Dictionary<Vector2Int, Cell>();
    
    public void SpawnChunkAtWorldCoord(Vector3 worldCoord)
    {
        Vector2 chunkCoord = WorldCoordToChunkCoord(worldCoord);

        if (!chunks.ContainsKey(chunkCoord))
        {
            GenerateChunk(chunkCoord);
            MoveChunks(Vector3.back);
        }
    }

    public void LoadInitialChunks()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 worldCoord = new Vector3(0, 0, 100);
            SpawnChunkAtWorldCoord(worldCoord);
        }
    }

    private void GenerateChunk(Vector2 chunkCoord)
    {
        chunkNumber++;
        ChunkData newChunk = CreateChunk(chunkCoord, chunkNumber);
        chunks[chunkCoord] = newChunk;

        // Add mines here
        AddMinesToChunk(newChunk, minePercentage);
    }

    public void AddMinesToFirstChunk(Cell safeCell)
    {
        GameObject firstChunk = GameObject.Find("Chunk_1");
        if (firstChunk != null)
        {
            foreach (Transform child in firstChunk.transform)
            {
                Cell cell = child.GetComponent<Cell>();
                if (cell != null && cell != safeCell)
                {
                    if (Random.Range(0f, 1f) < minePercentage)
                    {
                        // Set as mine only if it's not the clicked cell or its adjacent cells
                        if (!IsAdjacentToSafeCell(cell, safeCell))
                        {
                            cell.SetAsMine();
                        }
                    }
                }
            }

            isFirstChunkMinesActive = true;
        }
    }

    private bool IsAdjacentToSafeCell(Cell cell, Cell safeCell)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // Calculate position of the adjacent cell
                int adjacentX = safeCell.Position.x + i;
                int adjacentY = safeCell.Position.y + j;

                // Check if the cell is adjacent to the safe cell
                if (cell.Position.x == adjacentX && cell.Position.y == adjacentY)
                    return true;
            }
        }
        return false;
    }


    private void AddMinesToChunk(ChunkData chunk, float minePercentage)
    {
        foreach (var block in chunk.Blocks)
        {
            Cell cell = block.GetComponent<Cell>();
            if (cell != null && Random.Range(0f, 1f) < minePercentage)
            {
                // Set as mine only if first chunk mines are active or it's not the first chunk
                if (isFirstChunkMinesActive || chunkNumber > 1)
                {
                    cell.SetAsMine();
                }
            }
        }
    }

    private Vector2 WorldCoordToChunkCoord(Vector3 worldCoord)
    {
        int x = Mathf.FloorToInt((worldCoord.x + chunkRows / 2) / chunkRows);
        int y = Mathf.FloorToInt((worldCoord.z + chunkColumns / 2) / chunkColumns);
        return new Vector2(x, y);
    }

    private ChunkData CreateChunk(Vector2 chunkCoord, int chunkNumber)
    {
        GameObject chunkObject = new GameObject($"Chunk_{chunkNumber}");
        List<GameObject> chunkBlocks = GenerateBlocksForChunk(chunkCoord, chunkObject);
        chunkObject.transform.SetParent(this.transform);

        return new ChunkData(chunkBlocks, chunkObject);
    }

    private List<GameObject> GenerateBlocksForChunk(Vector2 chunkCoord, GameObject parentObject)
    {
        List<GameObject> blocks = new List<GameObject>();
        for (int x = 0; x < chunkRows; x++)
        {
            for (int z = 0; z < chunkColumns; z++)
            {
                Vector3 blockPosition = new Vector3(chunkCoord.x * chunkRows + x - chunkRows / 2, 0, chunkCoord.y * chunkColumns + z - chunkColumns / 2);
                GameObject block = Instantiate(blockPrefab, blockPosition, Quaternion.identity);
                Cell cellComponent = block.GetComponent<Cell>();
                cellComponent.Position = new Vector3Int(x, z, 0); 
                Vector2Int position = new Vector2Int(x, z);
                generatedCells[position] = cellComponent;
                block.transform.SetParent(parentObject.transform);
                blocks.Add(block);
            }
        }
        return blocks;
    }

    public Cell GetCellAt(int x, int y)
    {
        Vector2Int position = new Vector2Int(x, y);
        if (generatedCells.TryGetValue(position, out Cell cell))
        {
            return cell;
        }
        return null;
    }

    private void MoveChunks(Vector3 direction)
    {
        Dictionary<Vector2, ChunkData> newChunks = new Dictionary<Vector2, ChunkData>();
        Dictionary<Vector2Int, Cell> newGeneratedCells = new Dictionary<Vector2Int, Cell>();

        foreach (var chunk in chunks)
        {
            Vector3 chunkMove = new Vector3(direction.x * chunkRows, direction.y * chunkRows, direction.z * chunkColumns);
            chunk.Value.ChunkObject.transform.position += chunkMove;

            // Update cells in generatedCells
            foreach (var block in chunk.Value.Blocks)
            {
                Cell cell = block.GetComponent<Cell>();
                if (cell != null)
                {
                    Vector2Int oldPos = new Vector2Int(cell.Position.x, cell.Position.y);
                    Vector2Int newPos = oldPos + new Vector2Int((int)chunkMove.x, (int)chunkMove.z);
                    cell.Position = new Vector3Int(newPos.x, newPos.y, cell.Position.z);
                    newGeneratedCells[newPos] = cell;
                }
            }

            Vector2 newChunkCoord = WorldCoordToChunkCoord(chunk.Value.ChunkObject.transform.position);
            newChunks[newChunkCoord] = chunk.Value;
        }

        chunks = newChunks;
        generatedCells = newGeneratedCells;
    }

    public void DeleteChunks()
    {
        List<Vector2> chunksToDelete = new List<Vector2>();

        foreach (var chunk in chunks)
        {
            if (chunk.Value.ChunkObject.transform.position.z <= -100)
            {
                chunksToDelete.Add(chunk.Key);

                // Remove cells in this chunk from generatedCells
                foreach (var block in chunk.Value.Blocks)
                {
                    Cell cell = block.GetComponent<Cell>();
                    if (cell != null)
                    {
                        Vector2Int cellPos = new Vector2Int(cell.Position.x, cell.Position.y);
                        generatedCells.Remove(cellPos);
                    }
                }
            }
        }

        foreach (var chunkCoord in chunksToDelete)
        {
            Destroy(chunks[chunkCoord].ChunkObject);
            chunks.Remove(chunkCoord);
        }
    }
}

public struct ChunkData
{
    public List<GameObject> Blocks;
    public GameObject ChunkObject;

    public ChunkData(List<GameObject> blocks, GameObject chunkObject)
    {
        Blocks = blocks;
        ChunkObject = chunkObject;
    }
}
