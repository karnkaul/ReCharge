using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{ 
    public GameObject board;

    [Range(20, 100)]
    public int moveSpeed = 50;
    public AudioClip[] moveSFX, blockedSFX;

    private bool moving = false, disabled = false;
    private GameManager gameManager;
    private IBoardManager boardManager;
    private Animator animator;
    private AudioSource audioSource;

    void OnEnable ()
    {
        SubscribeDelegates();
    }

    void OnDisable ()
    {
        UnsubscirbeDelegates();
    }

    void EnableMovement ()
    {
        EventHandler.handleInput += AttemptMove;
        disabled = false;
    }

    void DisableMovement ()
    {
        EventHandler.handleInput -= AttemptMove;
        disabled = true;        
    }

    public void SubscribeDelegates ()
    {
        EnableMovement();
    }

    public void UnsubscirbeDelegates ()
    {
        DisableMovement();
    }

    void Start ()
    {
        if (board)
            boardManager = (IBoardManager)board.GetComponent<IBoardManager>();
        else
            boardManager = (IBoardManager)GameObject.Find("Board").GetComponent<IBoardManager>();
        if (!gameManager)
            gameManager = FindObjectOfType<GameManager>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void AttemptMove (Vector2 direction)
    {
        if (!disabled)
        {
            if (!moving)
            {
                if (!boardManager.AttemptMove(direction))
                {
                    if (blockedSFX.Length > 0)
                        audioSource.PlayOneShot(blockedSFX[Random.Range(0, blockedSFX.Length)]);
                    animator.SetTrigger("Blocked");
                }
                else
                {
                    if (moveSFX.Length > 0)
                        audioSource.PlayOneShot(moveSFX[Random.Range(0, moveSFX.Length)]);
                    gameManager.AddEnergy(-1);
                }
            }
        }
    }

    public IEnumerator SmoothMove (Vector3 position)
    {
        moving = true;
        while (transform.position != position)
        { 
            transform.position = Vector3.Lerp(transform.position, position, 0.5f * Time.deltaTime * moveSpeed);
            if (Mathf.Abs(transform.position.sqrMagnitude - position.sqrMagnitude) < 0.5f)
                transform.position = position;
            yield return null;
        }
        moving = false;
    }

    void Update ()
    {
    }
}

