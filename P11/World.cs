using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public Material textureAtlas;
    public static int worldHeight = 16;
    public static int chunkSize = 16;
    public static Dictionary<string, Chunk> world_Chunks;

    public static string BuildChunkName(Vector3 pos)
    {
        return $"{pos.x} _ {pos.y} _ {pos.z}";
    }

    IEnumerator BuildWorldHeight()
    {
        for(int i = 0; i < worldHeight; i++)
        {
            Vector3 chunkPosition = new Vector3(this.transform.position.x,
                                                i * chunkSize,
                                                this.transform.position.z);

            Chunk c = new Chunk(chunkPosition, textureAtlas);
            c.chunk.transform.parent = this.transform;
            world_Chunks.Add(c.chunk.name, c);
        }

        foreach(KeyValuePair<string,Chunk> c in world_Chunks)
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
