using UnityEngine;
using System.Collections;

public interface IBoardGenerator
{
    void InitBoard(bool restart = false);
    void StopEnemies();
}
