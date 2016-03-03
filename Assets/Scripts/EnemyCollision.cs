using UnityEngine;
using System.Collections;

public class EnemyCollision : MonoBehaviour
{
    public Animator animator;
    public AudioClip killSound;

    private GameManager gameManager;

    void Start ()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!GameManager.gameOver)
        {
            if (other.tag == "Player")
            {
                gameManager.GetComponent<AudioSource>().PlayOneShot(killSound);
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
        gameManager.GameOver();
        if (this)
            Destroy(this.transform.parent.gameObject);
    }
}
