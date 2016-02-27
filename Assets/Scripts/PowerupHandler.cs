using UnityEngine;
using System.Collections;

public class PowerupHandler : MonoBehaviour
{
    private bool consumed;
    private GameManager gameManager;
    private Animator animator;

    private int direction;
    private float rotator;

    void Start ()
    {
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();
        StartCoroutine(Statics.FadeIn(this.GetComponent<Renderer>()));

        rotator = Random.Range(0.5f, 2.5f);
        int x = Random.Range(1, 11);
        direction = (x > 5) ? -1 : 1;

        StartCoroutine(DelayedAnimate());
    }

    IEnumerator DelayedAnimate ()
    {
        yield return new WaitForSeconds(Random.Range(0, 0.5f));
        animator.SetTrigger("animate");
    }

    void Update ()
    {
        transform.Rotate(Vector3.forward * direction, rotator);
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (!consumed)
        {
            if (other.tag == "Player")
            {
                animator.SetTrigger("consumed");
                gameManager.AddEnergy(2);
                Destroy(this.gameObject, 0.5f);
                consumed = true;
            }
        }
    }
}
