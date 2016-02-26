using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{ 
    public GameObject board;

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

    public void SubscribeDelegates ()
    {
        EventHandler.handleInput += AttemptMove;
    }

    public void UnsubscirbeDelegates ()
    {
        EventHandler.handleInput -= AttemptMove;
    }

    void Start ()
    {
        if (board)
            boardManager = (IBoardManager)board.GetComponent<IBoardManager>();
        else
            boardManager = (IBoardManager)GameObject.Find("Board").GetComponent<IBoardManager>();
        animator = GetComponent<Animator>();
    }

    void AttemptMove (Vector2 direction)
    {
        if (!boardManager.AttemptMove(direction))
            animator.SetTrigger("Blocked");
    }

    void Update ()
    {
    }
}

