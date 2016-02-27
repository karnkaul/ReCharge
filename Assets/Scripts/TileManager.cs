using UnityEngine;
using System.Collections;

public class TileManager : MonoBehaviour
{
    public enum SpawnMode { Delayed, Instant };
    public SpawnMode spawnMode;

    private bool exitReached;

    void Start ()
    {
        switch (spawnMode)
        {
            case SpawnMode.Delayed:
                StartCoroutine(Statics.FadeIn(this.GetComponent<Renderer>()));
                break;
            default:
                break;
        }
        exitReached = false;
    }

    void OnTriggerStay2D (Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (this.tag == "Exit" && !exitReached)
            {
                Debug.Log("Exit reached.");
                FindObjectOfType<GameManager>().Invoke("LoadNext", 0.25f);
                exitReached = true;
            }
            BoardManager.playerTile = this.gameObject;
        }
    }
}
