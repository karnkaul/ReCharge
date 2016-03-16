using UnityEngine;
using System.Collections;

public class TileManager : MonoBehaviour
{
    public enum SpawnMode { Delayed, Instant };
    public SpawnMode spawnMode;
    public AudioClip exitSFX;
    public Vector2 index;

    private bool exitReached;
    private AudioSource audioSource;

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
        audioSource = FindObjectOfType<BoardManager>().GetComponent<AudioSource>();
    }

    void OnTriggerStay2D (Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (this.tag == "Exit" && !exitReached)
            {
                Debug.Log("Exit reached.");
                if (exitSFX)
                    audioSource.PlayOneShot(exitSFX);
                FindObjectOfType<GameManager>().LoadNext();
                //other.gameObject.SetActive(false);
                exitReached = true;
            }
            BoardManager.playerTile = this.gameObject;
        }
    }
}
