using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material textureAtlas;
    public static int worldHeight = 2;
    public static int chunkSize = 8;
    public static int worldSize = 2;
    public static Dictionary<string, Chunk> world_Chunks;

    public static string BuildChunkName(Vector3 pos)
    {
        return $"{pos.x} _ {pos.y} _ {pos.z}";
    }

    IEnumerator BuildWorldHeight()
    {

        for (int z = 0; z < worldSize; z++)
            for (int x = 0; x < worldSize; x++)
                for (int y = 0; y < worldHeight; y++)
                {
                    Vector3 chunkPosition = new Vector3(x * chunkSize,
                                                        y * chunkSize,
                                                        z * chunkSize);

                    Chunk c = new Chunk(chunkPosition, textureAtlas);
                    c.chunk.transform.parent = this.transform;
                    world_Chunks.Add(c.chunk.name, c);
                }

        foreach (KeyValuePair<string, Chunk> c in world_Chunks)
        {
            c.Value.DrawChunk();
            yield return null;
        }
    }

    private void Start()
    {
        world_Chunks = new Dictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        StartCoroutine(BuildWorldHeight());
    }
}
