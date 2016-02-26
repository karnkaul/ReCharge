using UnityEngine;
using System.Collections;

public class BoardManager : MonoBehaviour, IBoardManager
{
    public static GameObject playerTile;
    public GameObject player;
    public string obstacleTag = "Obstacle";

    private GameObject[,] tiles;
    private Vector3[,] tileLocations;

    void Start ()
    {
        tiles = GetComponent<BoardGenerator>().tileMap;
        if (!player)
            player = GameObject.Find("Player");
    }

    // Interface method
    public string GetObstacleTag()
    {
        return obstacleTag;
    }

    // Interface method
    public Vector2 GetPlayerTileIndices ()
    {
        if (tileLocations == null)
            return Vector2.zero;
        for (int i = 0; i < tileLocations.GetLength(0); ++i)
            for (int j = 0; j < tileLocations.GetLength(1); ++j)
                if (player.transform.position == tileLocations[i, j])
                    return new Vector2(i, j);
        return Vector2.zero;
    }

    // Interface method
    public void SetTileMap (GameObject[,] tiles)
    {
        this.tiles = tiles;

        tileLocations = new Vector3[tiles.GetLength(0), tiles.GetLength(1)];
        for (int i = 0; i < tiles.GetLength(0); ++i)
            for (int j = 0; j < tiles.GetLength(1); ++j)
                tileLocations[i, j] = tiles[i, j].transform.position;
    }

    // Interface method
    public bool AttemptMove (Vector3 direction)
    {

        // Get indices of player location's tiles
        int i = 0, j = 0;
        bool done = false;
        for (i = 0; i < tiles.GetLength(0); ++i)
        {
            for (j = 0; j < tiles.GetLength(1); ++j)
            {
                if (playerTile == tiles[i, j])
                {
                    done = true;
                    break;
                }
            }
            if (done)
                break;
        }

        GameObject attemptedTile = GetNextTile(i, j, direction);
        if (attemptedTile)
        {
            if (attemptedTile.tag == obstacleTag)
            {
                // Animate
                Debug.Log("Obstacle tag detected.");
                return false;
            }
            player.transform.position = attemptedTile.transform.position;
        }

        return true;
    }

    GameObject GetNextTile (int i, int j, Vector2 direction)
    {
        // Check attempted tile
        int newI = i + (int)direction.x;
        int newJ = j + (int)direction.y;
        if (newI < 0 || newI >= tiles.GetLength(0) || newJ < 0 || newJ >= tiles.GetLength(1))
        {
            Debug.Log("Boundary.");
            return null;
        }
        return tiles[newI, newJ];
    }

    void Update ()
    {
    }

}
