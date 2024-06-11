using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float transitionSpeed = 1.0f;
    private bool canMove = true;
    private Vector3 targetPosition;
    public float movementDistance = 1.0f;
    private GameObject chunks;
    public float maxZ = 999.0f;
    public TerrainModifier terrainModifier; // Reference to the TerrainModifier script

    void Start()
    {
        targetPosition = transform.position;
    }

    public void CheckRowsAndMoveForward(int rowsToCheck)
    {
        Vector3Int playerBlockPos = Vector3Int.FloorToInt(transform.position);
        ChunkPos playerChunkPos = terrainModifier.GetChunkPosition(playerBlockPos);

        for (int i = 3; i <= rowsToCheck + 2; i++)
        {
            for (int dx = -TerrainChunk.chunkWidth / 2; dx <= TerrainChunk.chunkWidth / 2; dx++)
            {
                Vector3Int checkPos = new Vector3Int(playerBlockPos.x + dx, playerBlockPos.y - 3, playerBlockPos.z + i);
                ChunkPos checkChunkPos = terrainModifier.GetChunkPosition(checkPos);

                Debug.DrawRay(checkPos, Vector3.up, Color.green, 10.0f);

                if (TerrainGenerator.chunks.TryGetValue(checkChunkPos, out TerrainChunk chunk))
                {
                    int localX = checkPos.x - checkChunkPos.x + 1;
                    int localZ = checkPos.z - checkChunkPos.z + 1;

                    BlockType blockType = chunk.blocks[localX, checkPos.y - 1, localZ];

                    if (blockType == BlockType.Mine && !terrainModifier.originalBlockStates.ContainsKey(checkPos))
                    {
                        Debug.Log("Game over - Unflagged mine detected");
                        return;
                    }
                    else if (blockType == BlockType.Flag && terrainModifier.originalBlockStates[checkPos] != BlockType.Mine)
                    {
                        Debug.Log("Game over - Wrongful flag detected");
                        return;
                    }
                }
            }
        }

        // If no issues were found, move the player forward
        targetPosition = transform.position + Vector3.forward * movementDistance;
        Debug.Log("Moving player forward");
    }

    void Update()
    {
        if (transform.position == targetPosition)
        {
            canMove = true;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && canMove)
        {
            CheckRowsAndMoveForward(3);
            canMove = false;
        }

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, transitionSpeed * Time.deltaTime);
    }

    bool IsOutOfBounds(Vector3 position)
    {
        return position.z > maxZ;
    }
}
