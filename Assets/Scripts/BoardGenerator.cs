using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardGenerator : MonoBehaviour, IBoardGenerator
{
    public GameObject[] tiles;
    public GameObject exitTile;
    public int numTiles;
    public float tileSize, noiseSampleSize = 0.3f;
    public bool debugInitBoard = false;
    public GameObject powerupPrefab, superPowerupPrefab;
    [Range(1.0f, 5.0f)]
    public float powerupChance, superChance;

    public GameObject[,] tileMap;
    [HideInInspector]
    public List<GameObject> powerups, supers;

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

    void Start ()
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

    public void InitBoard()
    {
        float levelPowerupChance = powerupChance - (float)(GameManager.Level) / 2;
        float levelSuperChance = superChance - ((float)(GameManager.Level) / 2) * (superChance / powerupChance);

        levelPowerupChance = Mathf.Clamp(levelPowerupChance, 1f, powerupChance);
        levelSuperChance = Mathf.Clamp(levelSuperChance, 0.1f, superChance);

        gameManager.UpdateUI();
        Debug.Log("Level " + GameManager.Level);
        Debug.Log("powerup chance:" + levelPowerupChance + " super chance:" + levelSuperChance);

        // Freeze player for animation duartion (1s)
        gameManager.DisablePlayerController();
        Invoke("EnablePC", Statics.fadeDuration);

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

                bool noPowerup = false;
                // Using guaranteed walkable circumference for now
                if (i == 0 || j == 0 || i == numTiles - 1 || j == numTiles - 1)
                {
                    noisyIndex = Random.Range(0, 2);
                    noPowerup = true;
                }

                if (noisyIndex >= 2)
                    noPowerup = true;

                // Instantiate floor tiles
                GameObject tile;
                string name;
               
                tile = Instantiate(tiles[noisyIndex], position, Quaternion.identity) as GameObject;
                bool tileOccupied = false;
                name = "Tile " + i.ToString() + "_" + j.ToString();

                tile.transform.parent = transform;
                tile.name = name;
                tileMap[j, i] = tile;

                if (i == numTiles - 1 && j == numTiles - 1)
                {
                    tile.GetComponent<Collider2D>().enabled = false;
                    GameObject exit = Instantiate(exitTile, position, Quaternion.identity) as GameObject;
                    exit.name = "Exit" ;
                    exit.transform.parent = this.transform;
                }

                // Dice roll instantiations
                // Powerup
                float roll = Random.Range(0.0f, 10.0f);
                GameObject powerup, super;

                if (!noPowerup && levelSuperChance > roll && !tileOccupied)
                {
                    super = Instantiate(superPowerupPrefab, position, Quaternion.identity) as GameObject;
                    super.transform.parent = tile.transform;
                    super.name = "Super Powerup " + supers.Count;
                    supers.Add(super);
                    tileOccupied = true;
                }

                if (!noPowerup && levelPowerupChance > roll && !tileOccupied)
                {
                    powerup = Instantiate(powerupPrefab, position, Quaternion.identity) as GameObject;
                    powerup.transform.parent = tile.transform;
                    powerup.name = "Powerup " + powerups.Count;
                    powerups.Add(powerup);
                    tileOccupied = true;
                }

                // Enemy
                roll = Random.Range(0.0f, 10.0f);
                if (!tileOccupied && roll > 12)
                {
                    tileOccupied = true;
                }

                // Flush
                tile = powerup = null;
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
