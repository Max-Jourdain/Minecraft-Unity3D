using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainModifier : MonoBehaviour
{
    public LayerMask groundLayer;

    public Inventory inv;

    float maxDist = 400;



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Explode(transform.position, 10);
        }

        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);
        if(leftClick || rightClick)
        {
            RaycastHit hitInfo;
            if(Physics.Raycast(transform.position, transform.forward, out hitInfo, maxDist, groundLayer))
            {
                Vector3 pointInTargetBlock;
                // Debug DrawRay
                Debug.DrawRay(transform.position, transform.forward * maxDist, Color.red);


                //destroy
                if(rightClick)
                    pointInTargetBlock = hitInfo.point + transform.forward * .01f;//move a little inside the block
                else
                    pointInTargetBlock = hitInfo.point - transform.forward * .01f;

                //get the terrain chunk (can't just use collider)
                int chunkPosX = Mathf.FloorToInt(pointInTargetBlock.x / 16f) * 16;
                int chunkPosZ = Mathf.FloorToInt(pointInTargetBlock.z / 16f) * 16;

                ChunkPos cp = new ChunkPos(chunkPosX, chunkPosZ);

                TerrainChunk tc = TerrainGenerator.chunks[cp];

                //index of the target block
                int bix = Mathf.FloorToInt(pointInTargetBlock.x) - chunkPosX+1;
                int biy = Mathf.FloorToInt(pointInTargetBlock.y);
                int biz = Mathf.FloorToInt(pointInTargetBlock.z) - chunkPosZ+1;

                if(rightClick)//replace block with air
                {
                    //inv.AddToInventory(tc.blocks[bix, biy, biz]);
                    tc.blocks[bix, biy, biz] = BlockType.Air;
                    tc.BuildMesh();
                }
                else if(leftClick)
                {
                    if(inv.CanPlaceCur())
                    {
                        tc.blocks[bix, biy, biz] = inv.GetCurBlock();

                        tc.BuildMesh();

                        inv.ReduceCur();
                    }
                    
                }
            }
        }
    }

    public void Explode(Vector3 explosionCenter, int explosionRadius)
    {
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

                            // Check if the coordinates are within bounds
                            if (bix >= 0 && bix < 16 && biy >= 0 && biy < 256 && biz >= 0 && biz < 16)
                            {
                                // Replace the block with air
                                tc.blocks[bix, biy, biz] = BlockType.Air;

                                // Optionally, add some logic here to handle different types of blocks differently
                            }
                        }
                    }
                }
            }
        }

        // Rebuild the affected chunks' meshes
        foreach (var chunk in TerrainGenerator.chunks.Values)
        {
            chunk.BuildMesh();
        }
    }
}
