using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string actionMapName = "Gameplay";
    [SerializeField] private string thrustActionName = "Thrust";
    [SerializeField] private string rotateLeftActionName = "RotateLeft";
    [SerializeField] private string rotateRightActionName = "RotateRight";

    public float ThrustInput { get; private set; }
    public float RotateInput { get; private set; }

    private InputAction thrustAction;
    private InputAction rotateLeftAction;
    private InputAction rotateRightAction;

    private bool touchThrustPressed;
    private bool touchRotateLeftPressed;
    private bool touchRotateRightPressed;
    private bool gameplayStarted;

    private void OnEnable()
    {
        BindActions();
        EnableActions();
    }

    private void OnDisable()
    {
        DisableActions();
    }

    private void Update()
    {
        float actionThrust = ReadActionAsFloat(thrustAction);
        float actionLeft = Mathf.Abs(ReadActionAsFloat(rotateLeftAction));
        float actionRight = Mathf.Abs(ReadActionAsFloat(rotateRightAction));

        if (Keyboard.current != null)
        {
            actionThrust = Mathf.Max(actionThrust, Keyboard.current.wKey.isPressed || Keyboard.current.spaceKey.isPressed ? 1f : 0f);
            actionLeft = Mathf.Max(actionLeft, Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? 1f : 0f);
            actionRight = Mathf.Max(actionRight, Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1f : 0f);
        }

        if (Gamepad.current != null)
        {
            actionThrust = Mathf.Max(actionThrust, Gamepad.current.buttonSouth.isPressed ? 1f : 0f);
            actionLeft = Mathf.Max(actionLeft, Gamepad.current.leftStick.x.ReadValue() < -0.2f ? 1f : 0f);
            actionRight = Mathf.Max(actionRight, Gamepad.current.leftStick.x.ReadValue() > 0.2f ? 1f : 0f);
        }

        bool hasGameplayInput = actionThrust > 0.01f || actionLeft > 0.01f || actionRight > 0.01f || touchThrustPressed || touchRotateLeftPressed || touchRotateRightPressed;
        if (!gameplayStarted && hasGameplayInput)
        {
            gameplayStarted = true;
            Lander.Instance?.BeginGameplay();
        }

        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        float touchThrust = touchThrustPressed ? 1f : 0f;
        float touchLeft = touchRotateLeftPressed ? 1f : 0f;
        float touchRight = touchRotateRightPressed ? 1f : 0f;

        ThrustInput = Mathf.Clamp01(Mathf.Max(actionThrust, touchThrust));
        float left = Mathf.Clamp01(Mathf.Max(actionLeft, touchLeft));
        float right = Mathf.Clamp01(Mathf.Max(actionRight, touchRight));
        RotateInput = Mathf.Clamp(right - left, -1f, 1f);
    }

    public void SetTouchThrust(bool isPressed)
    {
        touchThrustPressed = isPressed;
    }

    public void SetTouchRotateLeft(bool isPressed)
    {
        touchRotateLeftPressed = isPressed;
    }

    public void SetTouchRotateRight(bool isPressed)
    {
        touchRotateRightPressed = isPressed;
    }

    public void OnTouchThrustDown(BaseEventData _)
    {
        SetTouchThrust(true);
    }

    public void OnTouchThrustUp(BaseEventData _)
    {
        SetTouchThrust(false);
    }

    public void OnTouchRotateLeftDown(BaseEventData _)
    {
        SetTouchRotateLeft(true);
    }

    public void OnTouchRotateLeftUp(BaseEventData _)
    {
        SetTouchRotateLeft(false);
    }

    public void OnTouchRotateRightDown(BaseEventData _)
    {
        SetTouchRotateRight(true);
    }

    public void OnTouchRotateRightUp(BaseEventData _)
    {
        SetTouchRotateRight(false);
    }

    private void BindActions()
    {
        if (inputActions == null)
        {
            Debug.LogError("PlayerInputHandler is missing InputActionAsset reference.");
            return;
        }

        InputActionMap actionMap = inputActions.FindActionMap(actionMapName, false);
        if (actionMap == null)
        {
            actionMap = inputActions.actionMaps.Count > 0 ? inputActions.actionMaps[0] : null;
            if (actionMap == null)
            {
                Debug.LogError($"Action map '{actionMapName}' was not found in InputActionAsset.");
                return;
            }
        }

        thrustAction = actionMap.FindAction(thrustActionName, false);
        rotateLeftAction = actionMap.FindAction(rotateLeftActionName, false);
        rotateRightAction = actionMap.FindAction(rotateRightActionName, false);

        if (thrustAction == null || rotateLeftAction == null || rotateRightAction == null)
        {
           
        }
    }

    private void EnableActions()
    {
        inputActions?.Enable();
        thrustAction?.Enable();
        rotateLeftAction?.Enable();
        rotateRightAction?.Enable();
    }

    private void DisableActions()
    {
        thrustAction?.Disable();
        rotateLeftAction?.Disable();
        rotateRightAction?.Disable();
    }

    private float ReadActionAsFloat(InputAction action)
    {
        if (action == null)
        {
            return 0f;
        }

        if (action.activeControl != null && action.activeControl.valueType == typeof(Vector2))
        {
            Vector2 value = action.ReadValue<Vector2>();
            return value.x;
        }

        return action.ReadValue<float>();
    }
}
