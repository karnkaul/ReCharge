using UnityEngine;
using System.Collections;

public class EventHandler : MonoBehaviour
{
    public enum InputType { Buttons, Axes };
    public InputType inputType;

    public delegate void HandleInput(Vector2 direction);
    public static HandleInput handleInput;
    //public static event Void AttemptMove;

    private delegate void _HandleInput();
    private _HandleInput _handleInput;

    private bool W;
    private bool A;
    private bool S;
    private bool D;
    private bool Up;
    private bool Down;
    private bool Left;
    private bool Right;

    void Start()
    {
        switch (inputType)
        {
            case InputType.Axes:
                _handleInput = AxisInput;
                break;
            default:
                _handleInput = ButtonInput;
                break;
        }

    }

    void SetButtons ()
    {
        W = Input.GetKeyDown(KeyCode.W);
        A = Input.GetKeyDown(KeyCode.A);
        S = Input.GetKeyDown(KeyCode.S);
        D = Input.GetKeyDown(KeyCode.D);
        Up = Input.GetKeyDown(KeyCode.UpArrow);
        Down = Input.GetKeyDown(KeyCode.DownArrow);
        Left = Input.GetKeyDown(KeyCode.LeftArrow);
        Right = Input.GetKeyDown(KeyCode.RightArrow);
    }

    void Update()
    {
        SetButtons();
        _handleInput();
    }

    void AxisInput()
    {
        int horizontal = (int)Input.GetAxisRaw("Horizontal");
        int vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
        {
            horizontal = Mathf.Clamp(horizontal, -1, 1);
            vertical = Mathf.Clamp(vertical, -1, 1);
            Vector3 direction = new Vector3(horizontal, vertical);
            if (handleInput != null)
                handleInput(direction);
        }
    }

    void ButtonInput()
    {
        short x = 0, y = 0;

        if (Up || W)
            y = 1;
        else if (Down || S)
            y = -1;
        if (Left || A)
            x = -1;
        else if (Right || D)
            x = 1;

        Vector2 direction = new Vector2(x, y);
        if (direction.magnitude > 1.0f)
            direction = Vector2.zero;

        if (direction != Vector2.zero)
        {
            if (handleInput != null)
                handleInput(direction);
        }
    }
}
