using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Realtime.Messaging.Internal;

public class World : MonoBehaviour
{
    public GameObject player;
    public Material textureAtlas;
    public static int worldHeight = 16;
    public static int chunkSize = 16;
    public static int radius = 4;
    public static ConcurrentDictionary<string, Chunk> world_Chunks;
    public bool firstBuild = true;
    public List<string> toRemove = new List<string>();

    CoroutineQueue queue;
    public static uint maxCoroutine = 1000;

    public Vector3 lastBuildPos = Vector3.zero;

    public static string BuildChunkName(Vector3 pos)
    {
        return $"{pos.x} _ {pos.y} _ {pos.z}";
    }

    void BuildChunkAt(int x, int y, int z)
    {
        Vector3 chunkPosition = new Vector3(x * chunkSize,
                                            y * chunkSize,
                                            z * chunkSize);

        string n = BuildChunkName(chunkPosition);
        Chunk c;

        if (!world_Chunks.TryGetValue(n, out c))
        {
            c = new Chunk(chunkPosition, textureAtlas);
            c.chunk.transform.parent = this.transform;
            world_Chunks.TryAdd(c.chunk.name, c);
        }
    }

    IEnumerator BuildRecursiveWorld(int x, int y, int z, int rad)
    {
        rad--;
        if (rad <= 0) yield break;

        // build chunk front
        BuildChunkAt(x, y, z + 1);
        queue.Run(BuildRecursiveWorld(x, y, z + 1, rad));
        yield return null;


        // build chunk back
        BuildChunkAt(x, y, z - 1);
        queue.Run(BuildRecursiveWorld(x, y, z - 1, rad));
        yield return null;

        // build chunk right
        BuildChunkAt(x + 1, y, z);
        queue.Run(BuildRecursiveWorld(x + 1, y, z, rad));
        yield return null;

        // build chunk left
        BuildChunkAt(x - 1, y, z);
        queue.Run(BuildRecursiveWorld(x - 1, y, z, rad));
        yield return null;

        // build chunk up
        BuildChunkAt(x, y + 1, z);
        queue.Run(BuildRecursiveWorld(x, y + 1, z, rad));
        yield return null;

        // build chunk Down
        BuildChunkAt(x, y - 1, z);
        queue.Run(BuildRecursiveWorld(x, y - 1, z, rad));
        yield return null;
    }

    IEnumerator DrawChunks()
    {
        foreach (KeyValuePair<string, Chunk> c in world_Chunks)
        {
            if (c.Value.status == Chunk.ChunkStatus.DRAW)
            {
                c.Value.DrawChunk();
            }

            if (c.Value.chunk && Vector3.Distance(player.transform.position, c.Value.chunk.transform.position) > radius * chunkSize)
            {
                toRemove.Add(c.Key);
            }

            yield return null;
        }
    }

    IEnumerator RemoveOldChunks()
    {
        for (int i = 0; i < toRemove.Count; i++)
        {
            string n = toRemove[i];
            Chunk c;
            if (world_Chunks.TryGetValue(n, out c))
            {
                Destroy(c.chunk);
                c.Save();
                world_Chunks.Remove(n, out c);
                yield return null;
            }
        }
    }


    public void BuildNearPlayer()
    {
        StopCoroutine("BuildRecursiveWorld");
        queue.Run(BuildRecursiveWorld((int)player.transform.position.x / chunkSize,
                                           (int)player.transform.position.y / chunkSize,
                                           (int)player.transform.position.z / chunkSize, radius));
    }


    private void Start()
    {
        Vector3 pos = player.transform.position;
        player.transform.position = new Vector3(pos.x, NoiseUtils.GenerateHeight(pos.x, pos.z) + 1, pos.z);

        player.SetActive(false);

        lastBuildPos = player.transform.position;
        firstBuild = true;
        world_Chunks = new ConcurrentDictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        queue = new CoroutineQueue(maxCoroutine, StartCoroutine);


        //build Starting chunk
        BuildChunkAt((int)player.transform.position.x / chunkSize,
                     (int)player.transform.position.y / chunkSize,
                     (int)player.transform.position.z / chunkSize);

        // draw it
        queue.Run(DrawChunks());

        // create a bigger world
        queue.Run(BuildRecursiveWorld((int)player.transform.position.x / chunkSize,
                                           (int)player.transform.position.y / chunkSize,
                                           (int)player.transform.position.z / chunkSize, radius));
    }

    private void Update()
    {
        Vector3 movement = lastBuildPos - player.transform.position;

        if (movement.magnitude > chunkSize)
        {
            lastBuildPos = player.transform.position;
            BuildNearPlayer();
        }


        if (!player.activeSelf)
        {
            player.SetActive(true);
            firstBuild = false;
        }

        queue.Run(DrawChunks());
        queue.Run(RemoveOldChunks());
    }
}
