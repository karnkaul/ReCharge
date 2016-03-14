using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardGenerator : MonoBehaviour, IBoardGenerator
{
    // Tiles
    [SerializeField] [Header("Tiles")]
    private GameObject[] tiles;
    [SerializeField]
    private GameObject exitTile;
    [SerializeField]
    private int numTiles = 10;
    [SerializeField]
    private float tileSize = 5, noiseSampleSize = 0.3f;

    // Powerups
    [Header("Powerups")]
    public GameObject powerupPrefab;
    public GameObject superPowerupPrefab, enemyPrefab, playerPrefab;
    [Range(1.0f, 5.0f)]
    public float powerupChance = 5, superChance = 1;

    // Publics
    public GameObject[,] tileMap;
    [HideInInspector]
    public List<GameObject> powerups, supers, enemies;
    public delegate void _DeleteEnemy(GameObject x);
    public static _DeleteEnemy DeleteEnemy;

    // Debug
    [Header("Debug")]
    public bool debugInitBoard = false;

    private float offsetX, offsetY;
    private IBoardManager boardManager;
    private GameObject player;

    void OnEnable ()
    {
        DeleteEnemy += __DeleteEnemy;
    }

    void OnDisable()
    {
        DeleteEnemy -= __DeleteEnemy;
    }

    void Start ()
    {
        boardManager = GetComponent(typeof(IBoardManager)) as IBoardManager;
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

    public void StopEnemies ()
    {
        foreach (GameObject enemy in enemies)
            if (enemy)
                enemy.GetComponent<EnemyController>().triggers.freeze = true;
    }

    void __DeleteEnemy (GameObject enemy)
    {
        foreach(GameObject e in enemies)
        {
            if (e == enemy)
                enemies.Remove(enemy);
        }
    }

    public void InitBoard(bool restart = false)
    {
        if (!player)
            player = GameObject.Find("Player");
        if (restart)
        {
            if (player)
                Destroy(player);
            player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
        }
        if (!player.activeSelf)
            player.SetActive(true);

        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.moveSpeed += (GameManager.Level) / 2;
        playerController.moveSpeed = (int)Mathf.Clamp(playerController.moveSpeed, 30.0f, 40.0f);

        enemies.Clear();

        float levelPowerupChance = powerupChance - (float)(GameManager.Level) / 2;
        float levelSuperChance = superChance - ((float)(GameManager.Level) / 2) * (superChance / powerupChance);
        float enemyChance = 2 * (((float)(GameManager.Level + 1)/2) - 1);
        enemyChance = Mathf.Clamp(enemyChance, 0.0f, 3.0f);

        levelPowerupChance = Mathf.Clamp(levelPowerupChance, 1f, powerupChance);
        levelSuperChance = Mathf.Clamp(levelSuperChance, 0.1f, superChance);

        Debug.Log("Level " + GameManager.Level);
        Debug.Log("powerup chance:" + levelPowerupChance + " super chance:" + levelSuperChance + " enemy chance:" + enemyChance);

        // Freeze player for animation duartion (1s)
        GameManager.DisablePC();
        Invoke("EnablePC", Statics.fadeDuration);

        // Flush all children
        foreach (Transform obj in transform)
            Destroy(obj.gameObject);

        GameManager.UpdateUI();

        offsetX = Random.Range(0.0f, 10.0f - (2 * noiseSampleSize));
        offsetY = Random.Range(0.0f, 10.0f - (2 * noiseSampleSize));
        for (int i = 0; i < numTiles; ++i)
        {
            int superroll = Random.Range(0, numTiles);
            for (int j = 0; j < numTiles; ++j)
            {
                Renderer rend = tiles[0].GetComponent<Renderer>();
                Debug.DrawLine(rend.bounds.max, rend.bounds.min);
                Vector3 position = new Vector3(-2 * rend.bounds.min.x * j, -2 * rend.bounds.min.y * i, 0);
                int noisyIndex = GetNoisyTileNumber(i, j, noiseSampleSize);

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
                tile.GetComponent<TileManager>().index = new Vector2(j, i);
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
                if (enemyChance > 0)
                {
                    if (j == superroll)
                    {
                        if (enemyChance >= 3 && i == numTiles / 3)
                            InstantiateEnemy(position, tile);
                        if (enemyChance >= 2 && i == numTiles / 2)
                            InstantiateEnemy(position, tile);
                        if (enemyChance >= 1 && i == (2 * (numTiles / 3)))
                            InstantiateEnemy(position, tile);
                    }
                }
                // Flush
                tile = powerup = super = null;
            }
        }
        boardManager.SetTileMap(tileMap);
        boardManager.ResetPlayer();
    }

    void InstantiateEnemy (Vector3 position, GameObject tile)
    {
        GameObject enemy = Instantiate(enemyPrefab, position, Quaternion.identity) as GameObject;
        enemy.transform.parent = tile.transform;
        enemy.name = "Enemy " + enemies.Count;
        enemies.Add(enemy);
    }

    void EnablePC ()
    {
        GameManager.EnablePC();
    }

    int GetNoisyTileNumber(int x, int y, float scale)
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
                return ret;
            ret++;
        } while (ret < tileSetSize);

        return ret - 1;
    }
}
