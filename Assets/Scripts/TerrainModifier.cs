using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class TerrainModifier : MonoBehaviour
{
    public LayerMask groundLayer;
    public Camera playerCamera; // Assign your player's camera here
    public float rayLength = 400; // Max distance for raycast
    public int explosionRadius = 10; // Radius of the explosion

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse click
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.red, 1f);

            if (Physics.Raycast(ray, out hit, rayLength, groundLayer))
            {
                Explode(hit.point, explosionRadius);
            }
        }
    }


    public void Explode(Vector3 explosionCenter, int explosionRadius)
    {
        HashSet<ChunkPos> affectedChunks = new HashSet<ChunkPos>();

        // Iterate over a cubic region around the explosion center
        for (int x = -explosionRadius; x <= explosionRadius; x++)
        {
            for (int y = -explosionRadius; y <= explosionRadius; y++)
            {
                for (int z = -explosionRadius; z <= explosionRadius; z++)
                {
                    Vector3 explosionOffset = new Vector3(x, y, z);
                    Vector3 blockPosition = explosionCenter + explosionOffset;

                    // Check if this block is within the explosion radius
                    if (explosionOffset.magnitude <= explosionRadius)
                    {
                        // Convert blockPosition to chunk coordinates and block index
                        int chunkPosX = Mathf.FloorToInt(blockPosition.x / 16f) * 16;
                        int chunkPosZ = Mathf.FloorToInt(blockPosition.z / 16f) * 16;
                        ChunkPos cp = new ChunkPos(chunkPosX, chunkPosZ);

                        if (TerrainGenerator.chunks.TryGetValue(cp, out TerrainChunk tc))
                        {
                            int bix = Mathf.FloorToInt(blockPosition.x) - chunkPosX + 1;
                            int biy = Mathf.FloorToInt(blockPosition.y);
                            int biz = Mathf.FloorToInt(blockPosition.z) - chunkPosZ + 1;

                            if (bix >= 0 && bix <= 16 && biy >= 0 && biy < 256 && biz >= 0 && biz <= 16)
                            {           
                                tc.blocks[bix, biy, biz] = BlockType.Air;
                                affectedChunks.Add(cp);
                            }

                        }
                    }
                }
            }
        }

        foreach (var chunkPos in affectedChunks)
        {
            if (TerrainGenerator.chunks.TryGetValue(chunkPos, out TerrainChunk affectedChunk))
            {
                affectedChunk.BuildMesh();
                Debug.DrawLine(explosionCenter, affectedChunk.transform.position, Color.red, 100f);
            }
        }
    }
}
