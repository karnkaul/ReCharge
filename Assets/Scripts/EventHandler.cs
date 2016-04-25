using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EventHandler : MonoBehaviour
{
    public static Statics.InputType inputType;
    public Dropdown inputTypeDropdown;

    // For PlayerController etc to latch on to.
    public delegate void HandleInput(Vector2 direction);
    public static HandleInput handleInput;

    // For self evaluation
    private delegate void _HandleInput();
    private _HandleInput _handleInput;

    // Singleton
    private static EventHandler instance;
    public static EventHandler Instance
    {
        get { return instance; }
    }

    private enum AndroidControl { Taps, Gyro };
    private AndroidControl androidControl;

    private bool W;
    private bool A;
    private bool S;
    private bool D;
    private bool Up;
    private bool Down;
    private bool Left;
    private bool Right;

    private int x, y;

    void Awake ()
    {
        // Singleton
        if (!instance)
            instance = this;
        if (instance != this)
            Destroy(gameObject);

        switch (inputType)
        {
            case Statics.InputType.Axes:
                _handleInput = AxisInput;
                break;
            default:
                _handleInput = ButtonInput;
                break;
        }

    }

    public void SetControllerType()
    {
        if (inputTypeDropdown)
        {
            switch (inputTypeDropdown.value)
            {
                case 1:
                    _handleInput = AxisInput;
                    inputType = Statics.InputType.Axes;
                    break;
                default:
                    _handleInput = ButtonInput;
                    inputType = Statics.InputType.Buttons;
                    break;
            }
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
        x = y = 0;

        if (Up || W)
            y = 1;
        else if (Down || S)
            y = -1;
        if (Left || A)
            x = -1;
        else if (Right || D)
            x = 1;


#if UNITY_ANDROID
        if (androidControl == AndroidControl.Gyro)
            HandleGyro();
#endif

        Vector2 direction = new Vector2(x, y);
        if (direction.magnitude > 1.0f)
            direction = Vector2.zero;

        if (direction != Vector2.zero)
        {
            if (handleInput != null)
                handleInput(direction);
        }
    }

    void HandleGyro()
    {
        float deadzone = 0.3f;
        float _x = Input.acceleration.x;
        float _y = Input.acceleration.y;

        // Positive
        if (_x > deadzone || _y > deadzone)
        {
            if (_x >= _y)
            {
                _y = 0;
                _x = 1;
            }
            else
            {
                _x = 0;
                _y = 1;
            }
        }
        else if (_x < -deadzone || _y < -deadzone)
        {
            if (_x <= _y)
            {
                _y = 0;
                _x = -1;
            }
            else
            {
                _x = 0;
                _y = -1;
            }
        }
        x = (short)_x;
        y = (short)_y;
    }

    public void HandleTaps(int x, int y)
    {
        if (!PlayerController.disabled)
        {
            this.x = x;
            this.y = y;
            if (handleInput != null)
                handleInput(new Vector2(x, y));
        }
    }
}
