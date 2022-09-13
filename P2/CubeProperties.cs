using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CubeProperties : MonoBehaviour
{
    MeshFilter meshfiler;

    private void Start()
    {
        meshfiler = GetComponent<MeshFilter>();

        //----------Vertices----------
        foreach(Vector3 v in meshfiler.mesh.vertices)
        {
            Debug.Log("V : " + v);
        }

        //----------Normals----------
        foreach (Vector3 n in meshfiler.mesh.normals)
        {
            Debug.Log("N : " + n);
        }

        //----------UV----------
        foreach (Vector2 u in meshfiler.mesh.uv)
        {
            Debug.Log("U : " + u);
        }

        //----------Triangles----------
        foreach (int t in meshfiler.mesh.triangles)
        {
            Debug.Log("T : " + t);
        }


    }

}
