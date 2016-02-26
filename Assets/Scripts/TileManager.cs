using UnityEngine;
using System.Collections;

public class TileManager : MonoBehaviour
{
    public enum SpawnMode { Delayed, Instant };
    public SpawnMode spawnMode;

    void Start ()
    {
        switch (spawnMode)
        {
            case SpawnMode.Delayed:
                StartCoroutine(DelayedVisibility());
                break;
            default:
                break;
        }
    }

    IEnumerator DelayedVisibility ()
    {
        Renderer rend = GetComponent<Renderer>();
        rend.material.color = new Color(1, 1, 1, 0);

        yield return new WaitForSeconds(Random.Range(0.0f, 1.0f));
        rend.material.color = new Color(1, 1, 1, 1);
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.tag == "Player")
        {
            BoardManager.playerTile = this.gameObject;
        }
    }

}
