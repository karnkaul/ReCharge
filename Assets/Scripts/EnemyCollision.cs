using UnityEngine;
using System.Collections;

public class EnemyCollision : MonoBehaviour
{
    public Animator animator;
    public AudioClip killSound;

    private AudioSource audioSource;

    void Start ()
    {
        audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!GameManager.gameOver)
        {
            if (other.tag == "Player")
            {
                audioSource.PlayOneShot(killSound);
                StartCoroutine(ExplodeAndGameOver());
                other.GetComponent<PlayerController>().StopAllCoroutines();
                other.gameObject.SetActive(false);
            }
        }
        else
            Debug.Log("Game is over.");
    }

    IEnumerator ExplodeAndGameOver()
    {
        if (animator)
            animator.SetTrigger("explode");
        
        yield return new WaitForSeconds(1);
        GameManager.SetGameOver();
        if (this)
            Destroy(this.transform.parent.gameObject);
    }
}
