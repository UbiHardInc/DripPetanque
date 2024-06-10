using UnityEngine.InputSystem;

public static class UiSFXUtils
{
    public static void PressCancel(InputAction.CallbackContext obj)
    {
        SoundManager.Instance.PlayUISFX("cancel");
    }

    public static void PressSubmit(InputAction.CallbackContext obj)
    {
        SoundManager.Instance.PlayUISFX("submit");
    }

    public static void MoveInUI(InputAction.CallbackContext obj)
    {
        SoundManager.Instance.PlayUISFX("move");
    }
}
