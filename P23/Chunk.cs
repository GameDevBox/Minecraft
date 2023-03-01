using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[Serializable]
class BlockData
{
    public Block.BlockType[,,] matrix;

    public BlockData() { }

    public BlockData(Block[,,] b)
    {
        matrix = new Block.BlockType[World.chunkSize, World.chunkSize, World.chunkSize];
        for (int x = 0; x < World.chunkSize; x++)
            for (int y = 0; y < World.chunkSize; y++)
                for (int z = 0; z < World.chunkSize; z++)
                {
                    matrix[x, y, z] = b[x, y, z].bType;
                }
    }
}

public class Chunk
{
    public Material cube_Material;
    public Block[,,] chunkData;
    public GameObject chunk;
    public enum ChunkStatus { DRAW, KEEP, DONE }
    public ChunkStatus status;
    BlockData blockData;

    string BuildChunkFileName(Vector3 v)
    {
        return $"{Application.persistentDataPath}/SaveData/Chunk_{(int)v.x}_{(int)v.y}_{(int)v.z}_{World.chunkSize}_{World.radius}.dat";
    }

    bool Load() // read data from file
    {
        string chunkFile = BuildChunkFileName(chunk.transform.position);
        if (File.Exists(chunkFile))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(chunkFile, FileMode.Open);
            blockData = new BlockData();
            blockData = (BlockData)bf.Deserialize(file);
            file.Close();
            return true;
        }
        return false;
    }

    public void Save() // Write Data To File
    {
        string chunkFile = BuildChunkFileName(chunk.transform.position);
        if (!File.Exists(chunkFile))
            Directory.CreateDirectory(Path.GetDirectoryName(chunkFile));

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(chunkFile, FileMode.OpenOrCreate);
        blockData = new BlockData(chunkData);
        bf.Serialize(file, blockData);
        file.Close();
    }

    void buildChunk()
    {
        bool dataFromFile = false;
        dataFromFile = Load();

        chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];


        for (int z = 0; z < World.chunkSize; z++)
            for (int y = 0; y < World.chunkSize; y++)
                for (int x = 0; x < World.chunkSize; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);

                    int WorldX = (int)(x + chunk.transform.position.x);
                    int WorldY = (int)(y + chunk.transform.position.y);
                    int WorldZ = (int)(z + chunk.transform.position.z);

                    if (dataFromFile)
                    {
                        chunkData[x, y, z] = new Block(blockData.matrix[x, y, z], pos, chunk.gameObject, this);

                        continue;
                    }

                    int surfaceHeight = NoiseUtils.GenerateHeight(WorldX, WorldZ);

                    /*Caves*/
                    if (NoiseUtils.fBM3D(WorldX, WorldY, WorldZ, 0.1f, 3) < 0.43f)
                        chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, chunk.gameObject, this);

                    /*BEDROCK*/
                    else if (WorldY == 0)
                        chunkData[x, y, z] = new Block(Block.BlockType.BEDROCK, pos, chunk.gameObject, this);


                    else if (WorldY <= NoiseUtils.GenerateStoneHeight(WorldX, WorldZ))
                    {
                        /*DIAMOND*/
                        if (NoiseUtils.fBM3D(WorldX, WorldY, WorldZ, 0.01f, 2) < 0.4f)
                            chunkData[x, y, z] = new Block(Block.BlockType.DIAMOND, pos, chunk.gameObject, this);

                        /*REDSTONE*/
                        else if (NoiseUtils.fBM3D(WorldX, WorldY, WorldZ, 0.03f, 3) < 0.41f && WorldY < 20)
                            chunkData[x, y, z] = new Block(Block.BlockType.REDSTONE, pos, chunk.gameObject, this);

                        /*STONE*/
                        else
                            chunkData[x, y, z] = new Block(Block.BlockType.STONE, pos, chunk.gameObject, this);
                    }

                    /*GRASS*/
                    else if (WorldY == surfaceHeight)
                        chunkData[x, y, z] = new Block(Block.BlockType.GRASS, pos, chunk.gameObject, this);

                    /*DIRT*/
                    else if (WorldY < surfaceHeight)
                        chunkData[x, y, z] = new Block(Block.BlockType.DIRT, pos, chunk.gameObject, this);

                    /*AIR*/
                    else
                        chunkData[x, y, z] = new Block(Block.BlockType.AIR, pos, chunk.gameObject, this);

                    status = ChunkStatus.DRAW;
                }
    }


    // Use this for initialization
    public Chunk(Vector3 position, Material mat)
    {
        chunk = new GameObject(World.BuildChunkName(position));
        chunk.transform.position = position;
        cube_Material = mat;
        buildChunk();
    }


    public void Redraw()
    {
        GameObject.DestroyImmediate(chunk.GetComponent<MeshFilter>());
        GameObject.DestroyImmediate(chunk.GetComponent<MeshRenderer>());
        GameObject.DestroyImmediate(chunk.GetComponent<Collider>());
        DrawChunk();
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

        MeshCollider collider = chunk.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        collider.sharedMesh = chunk.transform.GetComponent<MeshFilter>().mesh;
        status = ChunkStatus.DONE;

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
