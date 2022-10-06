using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateQuads : MonoBehaviour
{
    public Material cubeMat;

    enum CubeSide { BOTTOM, TOP, RIGHT, LEFT, BACK, FRONT };

    public enum BlockType { GRASS, DIRT, STONE, Lava};

    public BlockType bType;

    Vector2[,] blockUVs = { 
		/*GRASS TOP*/		{new Vector2( 0.125f, 0.375f ), new Vector2( 0.1875f, 0.375f),
                                new Vector2( 0.125f, 0.4375f ),new Vector2( 0.1875f, 0.4375f )},
		/*GRASS SIDE*/		{new Vector2( 0.1875f, 0.9375f ), new Vector2( 0.25f, 0.9375f),
                                new Vector2( 0.1875f, 1.0f ),new Vector2( 0.25f, 1.0f )},
		/*DIRT*/			{new Vector2( 0.125f, 0.9375f ), new Vector2( 0.1875f, 0.9375f),
                                new Vector2( 0.125f, 1.0f ),new Vector2( 0.1875f, 1.0f )},
		/*STONE*/			{new Vector2( 0, 0.875f ), new Vector2( 0.0625f, 0.875f),
                                new Vector2( 0, 0.9375f ),new Vector2( 0.0625f, 0.9375f )},
        /*Lava*/			{new Vector2( 0.875f, 0.0625f ), new Vector2( 0.9375f, 0.0625f),
                                new Vector2( 0.875f, 0.125f ),new Vector2( 0.9375f, 0.125f )}
                        };

    private void Start()
    {
        CreateCube();
    }

    private void CreateCube()
    {
        CreateQuad(CubeSide.FRONT);
        CreateQuad(CubeSide.BACK);
        CreateQuad(CubeSide.BOTTOM);
        CreateQuad(CubeSide.TOP);
        CreateQuad(CubeSide.RIGHT);
        CreateQuad(CubeSide.LEFT);
        CombineQuads();
    }

    private void CreateQuad(CubeSide side)
    {
        Mesh mesh = new Mesh();
        mesh.name = "S_Mesh" + side.ToString();

        Vector3[] vertices = new Vector3[4];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] triangles = new int[6];

        //all possible vertcies 
        Vector3 p0 = new Vector3(-0.5f, -0.5f, 0.5f);
        Vector3 p1 = new Vector3(0.5f, -0.5f, 0.5f);
        Vector3 p2 = new Vector3(0.5f, -0.5f, -0.5f);
        Vector3 p3 = new Vector3(-0.5f, -0.5f, -0.5f);
        Vector3 p4 = new Vector3(-0.5f, 0.5f, 0.5f);
        Vector3 p5 = new Vector3(0.5f, 0.5f, 0.5f);
        Vector3 p6 = new Vector3(0.5f, 0.5f, -0.5f);
        Vector3 p7 = new Vector3(-0.5f, 0.5f, -0.5f);


        //all possible UVs
        Vector2 uv00;
        Vector2 uv10;
        Vector2 uv01;
        Vector2 uv11;

        if (bType == BlockType.GRASS && side == CubeSide.TOP)
        {
            uv00 = blockUVs[0, 0];
            uv10 = blockUVs[0, 1];
            uv01 = blockUVs[0, 2];
            uv11 = blockUVs[0, 3];
        }
        else if (bType == BlockType.GRASS && side == CubeSide.BOTTOM)
        {
            uv00 = blockUVs[(int)(BlockType.DIRT + 1), 0];
            uv10 = blockUVs[(int)(BlockType.DIRT + 1), 1];
            uv01 = blockUVs[(int)(BlockType.DIRT + 1), 2];
            uv11 = blockUVs[(int)(BlockType.DIRT + 1), 3];
        }
        else
        {
            uv00 = blockUVs[(int)(bType + 1), 0];
            uv10 = blockUVs[(int)(bType + 1), 1];
            uv01 = blockUVs[(int)(bType + 1), 2];
            uv11 = blockUVs[(int)(bType + 1), 3];
        }



        switch (side)
        {
            case CubeSide.BOTTOM:
                vertices = new Vector3[] { p0, p1, p2, p3 };
                normals = new Vector3[] {Vector3.down,
                                 Vector3.down,
                                 Vector3.down,
                                 Vector3.down};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case CubeSide.TOP:
                vertices = new Vector3[] { p7, p6, p5, p4 };
                normals = new Vector3[] {Vector3.up,
                                 Vector3.up,
                                 Vector3.up,
                                 Vector3.up};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case CubeSide.RIGHT:
                vertices = new Vector3[] { p5, p6, p2, p1 };
                normals = new Vector3[] {Vector3.right,
                                 Vector3.right,
                                 Vector3.right,
                                 Vector3.right};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case CubeSide.LEFT:
                vertices = new Vector3[] { p7, p4, p0, p3 };
                normals = new Vector3[] {Vector3.left,
                                 Vector3.left,
                                 Vector3.left,
                                 Vector3.left};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case CubeSide.FRONT:
                vertices = new Vector3[] { p4, p5, p1, p0 };
                normals = new Vector3[] {Vector3.forward,
                                 Vector3.forward,
                                 Vector3.forward,
                                 Vector3.forward};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
            case CubeSide.BACK:
                vertices = new Vector3[] { p6, p7, p3, p2 };
                normals = new Vector3[] {Vector3.back,
                                 Vector3.back,
                                 Vector3.back,
                                 Vector3.back};
                uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                triangles = new int[] { 3, 1, 0, 3, 2, 1 };
                break;
        }
        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();

        GameObject quad = new GameObject("quad");
        quad.transform.parent = gameObject.transform;
        MeshFilter meshFilter = (MeshFilter)quad.AddComponent(typeof(MeshFilter));
        meshFilter.mesh = mesh;
        MeshRenderer meshRenderer = quad.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        meshRenderer.material = cubeMat;
    }


    private void CombineQuads()
    {
        // Combine all children meshes 
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while(i < meshFilters.Length)
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
        meshRenderer.material = cubeMat;


        // Delete all uncombined children
        foreach(Transform quad in transform)
        {
            Destroy(quad.gameObject);
        }
    }
}
