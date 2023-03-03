using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class BlockInteraction : MonoBehaviour
{
    public GameObject cam;
    Block.BlockType buildtype = Block.BlockType.DIRT;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
            buildtype = Block.BlockType.DIRT;
        if (Input.GetKeyDown("2"))
            buildtype = Block.BlockType.STONE;
        if (Input.GetKeyDown("3"))
            buildtype = Block.BlockType.REDSTONE;
        if (Input.GetKeyDown("4"))
            buildtype = Block.BlockType.GOLD;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;


            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 4))
            {

                Chunk hitC;
                if (!World.world_Chunks.TryGetValue(hit.collider.gameObject.name, out hitC)) return;


                Vector3 hitBlock;
                if (Input.GetMouseButtonDown(0))
                {
                    hitBlock = hit.point - hit.normal / 2.0f;

                }
                else
                    hitBlock = hit.point + hit.normal / 2.0f;


                Block b = World.GetWorldBlock(hitBlock);
                hitC = b.owner;


                int x = (int)(Mathf.Round(hitBlock.x) - hit.collider.gameObject.transform.position.x);
                int y = (int)(Mathf.Round(hitBlock.y) - hit.collider.gameObject.transform.position.y);
                int z = (int)(Mathf.Round(hitBlock.z) - hit.collider.gameObject.transform.position.z);


                bool update = false;
                if (Input.GetMouseButtonDown(0))
                    update = b.hitBlock();
                else
                {
                    update = b.BuildBlock(buildtype);
                }

                if (update)
                {
                    hitC.changed = true;
                    List<string> neighboursUpdates = new List<string>();
                    float thisChunkx = hitC.chunk.transform.position.x;
                    float thisChunky = hitC.chunk.transform.position.y;
                    float thisChunkz = hitC.chunk.transform.position.z;

                    //neighboursUpdates.Add(hit.collider.gameObject.name);

                    //update neighbours
                    if (b.position.x == 0)
                        neighboursUpdates.Add(World.BuildChunkName(new Vector3(thisChunkx - World.chunkSize, thisChunky, thisChunkz)));
                    if (b.position.x == World.chunkSize - 1)
                        neighboursUpdates.Add(World.BuildChunkName(new Vector3(thisChunkx + World.chunkSize, thisChunky, thisChunkz)));
                    if (b.position.y == 0)
                        neighboursUpdates.Add(World.BuildChunkName(new Vector3(thisChunkx, thisChunky - World.chunkSize, thisChunkz)));
                    if (b.position.y == World.chunkSize - 1)
                        neighboursUpdates.Add(World.BuildChunkName(new Vector3(thisChunkx, thisChunky + World.chunkSize, thisChunkz)));
                    if (b.position.z == 0)
                        neighboursUpdates.Add(World.BuildChunkName(new Vector3(thisChunkx, thisChunky, thisChunkz - World.chunkSize)));
                    if (b.position.z == World.chunkSize - 1)
                        neighboursUpdates.Add(World.BuildChunkName(new Vector3(thisChunkx, thisChunky, thisChunkz + World.chunkSize)));

                    foreach (string cname in neighboursUpdates)
                    {
                        Chunk c;
                        if (World.world_Chunks.TryGetValue(cname, out c))
                        {
                            c.Redraw();
                        }
                    }
                }
            }
        }
    }
}

