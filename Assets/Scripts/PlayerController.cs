using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{ 
    public GameObject board;

    [Range(20, 50)]
    public int moveSpeed = 35;
    public float _inputDelay = 0.1f;
    public AudioClip[] moveSFX, blockedSFX;
    public bool showDebug = false;

    private bool moving = false, disabled = false;
    private float elapsed;
    private GameManager gameManager;
    private IBoardManager boardManager;
    private Animator animator;
    private AudioSource audioSource;
    private EventHandler eventHandler;
    private Vector2[] moves = new Vector2[2];
    private float inputDelay;

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
        eventHandler = FindObjectOfType<EventHandler>();

        inputDelay = _inputDelay;
    }

    void AttemptMove(Vector2 direction)
    {
        
        if (!disabled)
        {
            
            if (elapsed >= inputDelay)
            {
                // Single input
                if (eventHandler.inputType == EventHandler.InputType.Buttons)
                {
                    // Cache one overflow
                    if (moves[moves.Length - 1] == Vector2.zero)
                    {
                        int index;
                        for (index = 0; index < moves.Length; ++index)
                            if (moves[index] == Vector2.zero)
                                break;

                        moves[index] = direction;

                        if (showDebug)
                            for (index = 0; index < moves.Length; ++index)
                            if (moves[index] != Vector2.zero)
                                Debug.Log(index + ":" + moves[index]);
                    }
                    
                }
                // Turbo input
                else
                    moves[0] = direction;

                elapsed = 0;
            }
          
        }
    }

    // Time sensitive
    void ExecuteMove ()
    {
        // Double safety
        if (moves[0] != Vector2.zero)
        {
            if (!boardManager.AttemptMove(moves[0]))
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
            moves[0] = Vector2.zero;
        }
        
        // Pseudo-queue Push back
        if (moves[1] != Vector2.zero)
        for (int i = 1; i < moves.Length; ++i)
        {
            moves[i-1] = moves[i];
            if (i == moves.Length - 1)
                moves[i] = Vector2.zero;
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
        elapsed += Time.deltaTime;

        if (moves[0] != Vector2.zero && !moving) 
            ExecuteMove();
    }
}

