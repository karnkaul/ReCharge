using UnityEngine;
using System.Collections;

public interface IBoardManager
{
    void SetTileMap(GameObject[,] tiles);
    bool AttemptMove(Vector3 direction);
    Vector2 GetPlayerTileIndices();
    string GetObstacleTag();
}
