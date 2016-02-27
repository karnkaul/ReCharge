using UnityEngine;
using System.Collections;

public interface IBoardManager
{
    void SetTileMap(GameObject[,] tiles);
    void ResetPlayer();
    bool AttemptMove(Vector3 direction);
    Vector2 GetPlayerTileIndices();
    string GetObstacleTag();
}
