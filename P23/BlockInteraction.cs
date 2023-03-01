﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInteraction : MonoBehaviour
{
    public GameObject cam;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;


            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 4))
            {
                Vector3 hitBlock = hit.point - hit.normal / 2.0f;

                int x = (int)(Mathf.Round(hitBlock.x) - hit.collider.gameObject.transform.position.x);
                int y = (int)(Mathf.Round(hitBlock.y) - hit.collider.gameObject.transform.position.y);
                int z = (int)(Mathf.Round(hitBlock.z) - hit.collider.gameObject.transform.position.z);

                Chunk hitC;
                if (World.world_Chunks.TryGetValue(hit.collider.gameObject.name, out hitC) && hitC.chunkData[x, y, z].hitBlock())
                {

                    List<string> neighboursUpdates = new List<string>();
                    float thisChunkx = hitC.chunk.transform.position.x;
                    float thisChunky = hitC.chunk.transform.position.y;
                    float thisChunkz = hitC.chunk.transform.position.z;

                    //neighboursUpdates.Add(hit.collider.gameObject.name);

                    //update neighbours
                    if (x == 0)
                        neighboursUpdates.Add(World.BuildChunkName(new Vector3(thisChunkx - World.chunkSize, thisChunky, thisChunkz)));
                    if (x == World.chunkSize - 1)
                        neighboursUpdates.Add(World.BuildChunkName(new Vector3(thisChunkx + World.chunkSize, thisChunky, thisChunkz)));
                    if (y == 0)
                        neighboursUpdates.Add(World.BuildChunkName(new Vector3(thisChunkx, thisChunky - World.chunkSize, thisChunkz)));
                    if (y == World.chunkSize - 1)
                        neighboursUpdates.Add(World.BuildChunkName(new Vector3(thisChunkx, thisChunky + World.chunkSize, thisChunkz)));
                    if (z == 0)
                        neighboursUpdates.Add(World.BuildChunkName(new Vector3(thisChunkx, thisChunky, thisChunkz - World.chunkSize)));
                    if (z == World.chunkSize - 1)
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

