using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInputButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public enum TouchAction
    {
        Thrust,
        RotateLeft,
        RotateRight
    }

    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private TouchAction touchAction;

    public void OnPointerDown(PointerEventData eventData)
    {
        SetPressed(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        SetPressed(false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetPressed(false);
    }

    private void SetPressed(bool pressed)
    {
        if (playerInputHandler == null)
        {
            return;
        }

        switch (touchAction)
        {
            case TouchAction.Thrust:
                playerInputHandler.SetTouchThrust(pressed);
                break;
            case TouchAction.RotateLeft:
                playerInputHandler.SetTouchRotateLeft(pressed);
                break;
            case TouchAction.RotateRight:
                playerInputHandler.SetTouchRotateRight(pressed);
                break;
        }
    }
}
