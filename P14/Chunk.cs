using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
    public Material cube_Material;
    public Block[,,] chunkData;
    public GameObject chunk;

    void buildChunk()
    {
        chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];


        for (int z = 0; z < World.chunkSize; z++)
            for (int y = 0; y < World.chunkSize; y++)
                for (int x = 0; x < World.chunkSize; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);

                    int WorldX = (int)(x + chunk.transform.position.x);
                    int WorldY = (int)(y + chunk.transform.position.y);
                    int WorldZ = (int)(z + chunk.transform.position.z);

                    if (WorldY <= NoiseUtils.GenerateStoneHeight(WorldX, WorldZ))
                        chunkData[x, y, z] = new Block(Block.BlockType.STONE, pos, chunk.gameObject, this);

                    else if (WorldY == NoiseUtils.GenerateHeight(WorldX, WorldZ))
                        chunkData[x, y, z] = new Block(Block.BlockType.GRASS, pos, chunk.gameObject, this);

                    else if (WorldY < NoiseUtils.GenerateHeight(WorldX, WorldZ))
                        chunkData[x, y, z] = new Block(Block.BlockType.DIRT, pos, chunk.gameObject, this);

                    else
                        chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, chunk.gameObject, this);
                }
    }


    public Chunk(Vector3 position, Material mat)
    {
        chunk = new GameObject(World.BuildChunkName(position));
        chunk.transform.position = position;
        cube_Material = mat;
        buildChunk();
    }


    public void DrawChunk()
    {
        //Draw block
        for (int z = 0; z < World.chunkSize; z++)
            for (int y = 0; y < World.chunkSize; y++)
                for (int x = 0; x < World.chunkSize; x++)
                {
                    chunkData[x, y, z].Draw();
                }

        CombineQuads();
    }

    private void CombineQuads()
    {
        // Combine all children meshes 
        MeshFilter[] meshFilters = chunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }


        // create a new mesh on the parent object + Add Combined mesh to it
        MeshFilter mf = (MeshFilter)chunk.gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();

        mf.mesh.CombineMeshes(combine);


        // Create renderer for the parent 
        MeshRenderer meshRenderer = chunk.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshRenderer.material = cube_Material;


        // Delete all uncombined children
        foreach (Transform quad in chunk.transform)
        {
            GameObject.Destroy(quad.gameObject);
        }
    }
}
