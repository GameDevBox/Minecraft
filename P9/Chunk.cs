using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Material cube_Material;
    public Block[,,] chunkData;

    IEnumerator buildChunk(int sizeX, int sizeY, int sizeZ)
    {
        chunkData = new Block[sizeX, sizeY, sizeZ];


        for (int z = 0; z < sizeZ; z++)
            for (int y = 0; y < sizeY; y++)
                for (int x = 0; x < sizeX; x++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    chunkData[x,y,z] = new Block(cube_Material, Block.BlockType.DIRT, pos, this.gameObject);

                }


        //Draw block
        for(int z = 0; z < sizeZ;z++)
            for(int y = 0; y < sizeY; y++)
                for(int x = 0; x < sizeX; x++)
                {
                    chunkData[x, y, z].Draw();
                    yield return null;
                }



        CombineQuads();
    }






    private void CombineQuads()
    {
        // Combine all children meshes 
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            i++;
        }


        // create a new mesh on the parent object + Add Combined mesh to it
        MeshFilter mf = (MeshFilter)gameObject.AddComponent(typeof(MeshFilter));
        mf.mesh = new Mesh();

        mf.mesh.CombineMeshes(combine);


        // Create renderer for the parent 
        MeshRenderer meshRenderer = gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshRenderer.material = cube_Material;


        // Delete all uncombined children
        foreach (Transform quad in transform)
        {
            Destroy(quad.gameObject);
        }
    }


    private void Start()
    {
        StartCoroutine(buildChunk(6, 6, 6));
    }
}
