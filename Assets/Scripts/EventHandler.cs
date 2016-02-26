using UnityEngine;
using System.Collections;

public class EventHandler : MonoBehaviour
{
    public enum InputType { Buttons, Axes };
    public InputType inputType;

    public delegate void HandleInput (Vector2 direction);
    public static HandleInput handleInput;
    //public static event Void AttemptMove;

    private delegate void _HandleInput();
    private _HandleInput _handleInput;

    void Start ()
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

    void Update ()
    {
        _handleInput();
    }

    void AxisInput ()
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

    void ButtonInput ()
    {
        float x = 0, y = 0;

        if (Input.GetKeyDown(KeyCode.W))
            y = 1;
        else if (Input.GetKeyDown(KeyCode.S))
            y = -1;
        if (Input.GetKeyDown(KeyCode.A))
            x = -1;
        else if (Input.GetKeyDown(KeyCode.D))
            x = 1;

        Vector2 direction = new Vector2(x, y);

        if (direction != Vector2.zero)
        {
            if (handleInput != null)
                handleInput(direction);
        }
    }
}
