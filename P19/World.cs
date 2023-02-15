using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour
{
    public GameObject Player;
    public Material textureAtlas;
    public static int worldHeight = 16;
    public static int chunkSize = 16;
    public static int worldSize = 2;
    public static int raduis = 1;
    public static Dictionary<string, Chunk> world_Chunks;
    public Slider loadingAmount;
    public Camera cam;
    public GameObject PlayButton;
    public bool firstBuild = true;
    public bool building = false;

    public static string BuildChunkName(Vector3 pos)
    {
        return $"{pos.x} _ {pos.y} _ {pos.z}";
    }

    IEnumerator BuildWorldHeight()
    {
        building = true;
        int posx = (int)Mathf.Floor(Player.transform.position.x / chunkSize);
        int posz = (int)Mathf.Floor(Player.transform.position.z / chunkSize);

        float totalChunks = (Mathf.Pow(raduis * 2 + 1, 2) * worldHeight) * 2;
        int processCount = 0;

        for (int z = -raduis; z <= raduis; z++)
            for (int x = -raduis; x <= raduis; x++)
                for (int y = 0; y < worldHeight; y++)
                {
                    Vector3 chunkPosition = new Vector3((x + posx) * chunkSize,
                                                        y * chunkSize,
                                                        (z + posz) * chunkSize);

                    Chunk c;
                    string n = BuildChunkName(chunkPosition);
                    if (world_Chunks.TryGetValue(n, out c))
                    {
                        c.status = Chunk.ChunkStatus.KEEP;
                        break;
                    }
                    else
                    {
                        c = new Chunk(chunkPosition, textureAtlas);
                        c.chunk.transform.parent = this.transform;
                        world_Chunks.Add(c.chunk.name, c);
                    }

                    if (firstBuild)
                    {
                        processCount++;
                        loadingAmount.value = processCount / totalChunks * 100;
                    }
                    yield return null;
                }

        foreach (KeyValuePair<string, Chunk> c in world_Chunks)
        {
            if (c.Value.status == Chunk.ChunkStatus.DRAW)
            {
                c.Value.DrawChunk();
                c.Value.status = Chunk.ChunkStatus.KEEP;
            }

            // delete the old chunks

            c.Value.status = Chunk.ChunkStatus.DONE;
            if (firstBuild)
            {
                processCount++;
                loadingAmount.value = processCount / totalChunks * 100;
            }
            yield return null;
        }

        if (firstBuild)
        {
            Player.SetActive(true);
            loadingAmount.gameObject.SetActive(false);
            cam.gameObject.SetActive(false);
            PlayButton.SetActive(false);
            firstBuild = false;
        }
        building = false;
    }


    public void StartBuild()
    {
        StartCoroutine(BuildWorldHeight());
    }


    private void Start()
    {
        Player.SetActive(false);
        world_Chunks = new Dictionary<string, Chunk>();
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
    }

    private void Update()
    {
        if (!building && !firstBuild)
            StartCoroutine(BuildWorldHeight());
    }
}
