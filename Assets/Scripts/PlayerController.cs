using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{ 
    public GameObject board;

    [Range(20, 50)] [Header("Input")]
    public int moveSpeed = 35;
    public float _inputDelay = 0.1f;

    [Header("Audio")]
    public AudioClip[] moveSFX;
    public AudioClip[] blockedSFX;

    [Header("Debug")]
    public bool printDebugToLog = false;

    public static Statics.VoidV3 SmoothMove;

    public static bool disabled = false;

    private bool moving = false;
    private float elapsed;
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
        UnsubscribeDelegates();
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
        SmoothMove += __SmoothMove;
        EnableMovement();
        
    }

    public void UnsubscribeDelegates ()
    {
        SmoothMove -= __SmoothMove;
        DisableMovement();
    }

    void Start ()
    {
        if (board)
            boardManager = (IBoardManager)board.GetComponent<IBoardManager>();
        else
            boardManager = (IBoardManager)GameObject.Find("Board").GetComponent<IBoardManager>();
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
                // Single input: queue up (moves.Length-1) overflows.
                if (EventHandler.inputType == Statics.InputType.Buttons)
                {
                    
                    if (moves[moves.Length - 1] == Vector2.zero)
                    {
                        int index;
                        for (index = 0; index < moves.Length; ++index)
                            if (moves[index] == Vector2.zero)
                                break;

                        moves[index] = direction;

                        if (printDebugToLog)
                            for (index = 0; index < moves.Length; ++index)
                            if (moves[index] != Vector2.zero)
                                Debug.Log(index + ":" + moves[index]);
                    }
                    
                }

                // Turbo input: simply execute.
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
                GameManager.AddEnergy(-1);
            }
            moves[0] = Vector2.zero;
        }
        
        // Pseudo-dequeue
        if (moves[1] != Vector2.zero)
        for (int i = 1; i < moves.Length; ++i)
        {
            moves[i-1] = moves[i];
            if (i == moves.Length - 1)
                moves[i] = Vector2.zero;
        }
    }

    // Delegate helper
    void __SmoothMove(Vector3 position)
    {
        StartCoroutine(_SmoothMove(position));
    }

    IEnumerator _SmoothMove (Vector3 position)
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

        if (!disabled)
        {
            if (moves[0] != Vector2.zero && !moving)
                ExecuteMove();
        }
    }
}

