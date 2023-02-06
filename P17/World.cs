using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public GameObject Player;
    public Material textureAtlas;
    public static int worldHeight = 16;
    public static int chunkSize = 16;
    public static int worldSize = 2;
    public static int raduis = 1;
    public static Dictionary<string, Chunk> world_Chunks;

    public static string BuildChunkName(Vector3 pos)
    {
        return $"{pos.x} _ {pos.y} _ {pos.z}";
    }

    IEnumerator BuildWorldHeight()
    {
        int posx = (int)Mathf.Floor(Player.transform.position.x / chunkSize);
        int posz = (int)Mathf.Floor(Player.transform.position.z / chunkSize);


        for (int z = -raduis; z <= raduis; z++)
            for (int x = -raduis; x <= raduis; x++)
                for (int y = 0; y < worldHeight; y++)
                {
                    Vector3 chunkPosition = new Vector3((x + posx) * chunkSize,
                                                        y * chunkSize,
                                                        (z + posz) * chunkSize);

                    Chunk c = new Chunk(chunkPosition, textureAtlas);
                    c.chunk.transform.parent = this.transform;
                    world_Chunks.Add(c.chunk.name, c);
                    yield return null;
                }

        foreach (KeyValuePair<string, Chunk> c in world_Chunks)
        {
            c.Value.DrawChunk();
            yield return null;
        }

        Player.SetActive(true);
    }


    private void Start()
    {
        Player.SetActive(false);
        world_Chunks = new Dictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        StartCoroutine(BuildWorldHeight());
    }
}
