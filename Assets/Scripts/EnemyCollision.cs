using UnityEngine;
using System.Collections;

public class EnemyCollision : MonoBehaviour
{
    public Animator animator;
    public AudioClip killSound;

    private AudioSource audioSource;
    private bool collided = false;

    void Start ()
    {
        audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!collided)
        {
            if (other.tag == "Player")
            {
                audioSource.PlayOneShot(killSound);
                StartCoroutine(ExplodeAndGameOver());
                other.gameObject.SetActive(false);
                //BoardGenerator.DeleteEnemy(this.gameObject);
                //StartCoroutine(Explode());
                //GameManager.AddEnergy(-10);
                collided = true;
            }
        }
        else
            Debug.Log("Game is over.");
    }

    IEnumerator Explode()
    {
        if (animator)
            animator.SetTrigger("explode");

        yield return new WaitForSeconds(1);

        if (this) 
            Destroy(this.transform.parent.gameObject);
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
