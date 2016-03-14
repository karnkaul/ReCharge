using UnityEngine;
using System.Collections;

public class BoardManager : MonoBehaviour, IBoardManager
{
    public static GameObject playerTile;
    public GameObject player;
    public string obstacleTag = "Obstacle";

    private GameObject[,] tiles;
    private Vector3[,] tileLocations;
    private PlayerController playerController;

    void Start ()
    {
        tiles = GetComponent<BoardGenerator>().tileMap;
        if (!player)
            player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
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

        //for (int i = 0; i < tiles.GetLength(0); ++i)
        //    for (int j = 0; j < tiles.GetLength(1); ++j)
        //        if (playerTile == tiles[i, j])
        //            return new Vector2(i, j);
        try
        {
            return playerTile.GetComponent<TileManager>().index;
        }
        catch
        {
            return Vector2.zero;
        }
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
        Vector2 playerTileIndices = GetPlayerTileIndices();

        GameObject attemptedTile = GetNextTile((int)playerTileIndices.x, (int)playerTileIndices.y, direction);
        if (attemptedTile)
        {
            if (attemptedTile.tag == obstacleTag)
            {
                Debug.Log("Obstacle tag detected.");
                return false;
            }
            else
                StartCoroutine(playerController.SmoothMove(attemptedTile.transform.position));
        }
        else
            return false;

        return true;
    }

    public void ResetPlayer ()
    {
        player.transform.position = tiles[0, 0].transform.position;
        player.GetComponent<PlayerController>().ResetMoves();
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

}
