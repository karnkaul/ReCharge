using UnityEngine;
using System.Collections;

public class PowerupHandler : MonoBehaviour
{
    private bool consumed;
    private GameManager gameManager;
    private Animator animator;

    void Awake ()
    {
        gameManager = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (!consumed)
        {
            if (other.tag == "Player")
            {
                animator.SetTrigger("consumed");
                Debug.Log("Powerup " + this.name + " picked up.");
                gameManager.AddEnergy(2);
                Destroy(this.gameObject, 0.5f);
                consumed = true;
            }
        }
    }
}
