using UnityEngine;
using System.Collections;

public class PowerupHandler : MonoBehaviour
{
    public AudioClip[] powerup, super;

    private bool consumed;
    private Animator animator;
    private int direction;
    private float rotator;
    private AudioSource audioSource;

    void Start ()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(Statics.FadeIn(this.GetComponent<Renderer>()));

        rotator = Random.Range(0.5f, 2.5f);
        int x = Random.Range(1, 11);
        direction = (x > 5) ? -1 : 1;

        StartCoroutine(DelayedAnimate());

        audioSource = GetComponent<AudioSource>();
    }

    IEnumerator DelayedAnimate ()
    {
        yield return new WaitForSeconds(Random.Range(0, 0.5f));
        animator.SetTrigger("animate");
    }

    void Update ()
    {
        if (tag == "Powerup")
            transform.Rotate(Vector3.forward * direction, rotator);
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (!consumed)
        {
            if (other.tag == "Player")
            {
                animator.SetTrigger("consumed");
                int energy = 2;
                if (powerup.Length > 0)
                    audioSource.PlayOneShot(powerup[Random.Range(0, powerup.Length)]);
                if (tag == "Super")
                {
                    energy = Random.Range(4, 12);
                    if (super.Length > 0)
                        audioSource.PlayOneShot(super[Random.Range(0, super.Length)]);
                }
                GameManager.AddEnergy(energy);
                Destroy(this.gameObject, 0.5f);
                consumed = true;
            }
        }
    }
}
