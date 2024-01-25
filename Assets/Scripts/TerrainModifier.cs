using System.Collections.Generic;
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

        for (int x = -explosionRadius; x <= explosionRadius; x++)
        {
            for (int y = -explosionRadius; y <= explosionRadius; y++)
            {
                for (int z = -explosionRadius; z <= explosionRadius; z++)
                {
                    Vector3 explosionOffset = new Vector3(x, y, z);
                    Vector3 blockPosition = explosionCenter + explosionOffset;

                    if (explosionOffset.magnitude <= explosionRadius)
                    {
                        int globalX = Mathf.FloorToInt(blockPosition.x);
                        int globalY = Mathf.FloorToInt(blockPosition.y);
                        int globalZ = Mathf.FloorToInt(blockPosition.z);

                        int chunkPosX = Mathf.FloorToInt(globalX / 16f) * 16;
                        int chunkPosZ = Mathf.FloorToInt(globalZ / 16f) * 16;
                        ChunkPos cp = new ChunkPos(chunkPosX, chunkPosZ);

                        if (TerrainGenerator.chunks.TryGetValue(cp, out TerrainChunk tc))
                        {
                            int bix = globalX - chunkPosX;
                            int biy = globalY;
                            int biz = globalZ - chunkPosZ;

                            tc.blocks[bix, biy, biz] = BlockType.Air;
                            affectedChunks.Add(cp);
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
            }
        }
    }
}
