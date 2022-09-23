using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateQuads : MonoBehaviour
{
    public Material cubeMat;

    enum CubeSide { BOTTOM, TOP, RIGHT, LEFT, BACK, FRONT };

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
        Vector2 uv00 = new Vector2(0f, 0f);
        Vector2 uv10 = new Vector2(1f, 0f);
        Vector2 uv01 = new Vector2(0f, 1f);
        Vector2 uv11 = new Vector2(1f, 1f);


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
}
