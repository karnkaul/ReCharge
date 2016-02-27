using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{ 
    public GameObject board;

    [Range(20, 100)]
    public int moveSpeed = 50;

    private GameManager gameManager;
    private IBoardManager boardManager;
    private Animator animator;

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
    }

    void DisableMovement ()
    {
        EventHandler.handleInput -= AttemptMove;
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
    }

    void AttemptMove (Vector2 direction)
    {
        if (!boardManager.AttemptMove(direction))
            animator.SetTrigger("Blocked");
        else gameManager.AddEnergy(-1);
    }

    public IEnumerator SmoothMove (Vector3 position)
    {
        DisableMovement();
        while (transform.position != position)
        { 
            transform.position = Vector3.Lerp(transform.position, position, 0.5f * Time.deltaTime * moveSpeed);
            if (Mathf.Abs(transform.position.sqrMagnitude - position.sqrMagnitude) < 0.5f)
                transform.position = position;
            yield return null;
        }
        EnableMovement();
    }

    void Update ()
    {
    }
}

