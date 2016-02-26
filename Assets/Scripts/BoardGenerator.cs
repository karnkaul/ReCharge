using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardGenerator : MonoBehaviour
{
    public GameObject[] tiles;
    public int numTiles;
    public float tileSize, noiseSampleSize = 0.3f;
    public bool debugInitBoard = false;
    public GameObject[,] tileMap;


    private bool tileSpawnComplete = false;
    public bool TileSpawnComplete
    {
        get
        {
            return TileSpawnComplete;
        }
    }

    private float offsetX, offsetY;
    private IBoardManager boardManager;
    private GameManager gameManager;

    void Awake ()
    {
        boardManager = GetComponent(typeof(IBoardManager)) as IBoardManager;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        tileMap = new GameObject[numTiles, numTiles];
        InitBoard();
    }
	
	void Update ()
    {
	    if (debugInitBoard)
        {
            InitBoard();
            debugInitBoard = false;
        }
    }

    void InitBoard()
    {
        // Freeze player for animation duartion (1s)
        gameManager.DisablePlayerController();
        Invoke("EnablePC", 1);

        // Flush all children
        foreach (Transform obj in transform)
            Destroy(obj.gameObject);

        offsetX = Random.Range(0.0f, 10.0f - (2 * noiseSampleSize));
        offsetY = Random.Range(0.0f, 10.0f - (2 * noiseSampleSize));
        for (int i = 0; i < numTiles; ++i)
        {
            for (int j = 0; j < numTiles; ++j)
            {
                Renderer rend = tiles[0].GetComponent<Renderer>();
                Debug.DrawLine(rend.bounds.max, rend.bounds.min);
                Vector3 position = new Vector3(-2 * rend.bounds.min.x * j, -2 * rend.bounds.min.y * i, 0);
                int noisyIndex = getNoisyTileNumber(i, j, noiseSampleSize);

                // BUGGY: player can get surrounded by Obstacles
                /*
                // Exchange first tile if it is an Obstacle
                if (i == 0 && j == 0)
                //if (j == boardManager.GetPlayerTileIndices().x && i == boardManager.GetPlayerTileIndices().y) 
                    if (tiles[noisyIndex].tag == boardManager.GetObstacleTag())
                        noisyIndex -= 1;
                */

                // Using guaranteed walkable circumference for now
                if (i == 0 || j == 0 || i == numTiles - 1 || j == numTiles - 1)
                    noisyIndex = Random.Range(0, 2);

                GameObject tile = Instantiate(tiles[noisyIndex], position, Quaternion.identity) as GameObject;
                string name = "Tile " + i.ToString() + "_" + j.ToString();
                tile.transform.parent = transform;
                tile.name = name;
                tileMap[j, i] = tile;
                tile = null;
            }
        }
        boardManager.SetTileMap(tileMap);
    }

    void EnablePC ()
    {
        gameManager.EnablePlayerController();
    }

    int getNoisyTileNumber(int x, int y, float scale)
    {
        float noiseValue = Mathf.PerlinNoise(x * scale + offsetX, y * scale + offsetY);
        int tileSetSize = tiles.Length;
        float bucketSize = 1.0f / (float)tileSetSize;

        float currentBucket = 0;
        int ret = 0;
        
        do
        {
            currentBucket += bucketSize;
            
            if (noiseValue <= currentBucket)
            {
                //Debug.Log("nv:" + noiseValue + " cb:" + (currentBucket - bucketSize) + "-" + currentBucket + " ret:" + ret);
                return ret;
            }
            ret++;
        } while (ret < tileSetSize);
        //Debug.Log(ret - 1);
        return ret - 1;
    }
}
