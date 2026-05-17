using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum InputActionMapType
{
    None = 0,
    Basic = 1 << 0,
    Run = 1 << 1,
    Code = 1 << 2,
}

public class GameInput : IDisposable
{
    public InputActions.BasicActions Basic => inputActions.Basic;
    public InputActions.RunActions Run => inputActions.Run;
    public InputActions.CodeActions Code => inputActions.Code;

    readonly InputActions inputActions;

    public GameInput()
    {
        inputActions = new();
        inputActions.Enable();
    }

    public InputActionMapType GetInputActionMaps()
    {
        InputActionMapType inputActionMapType = InputActionMapType.None;
        if (Basic.enabled) inputActionMapType |= InputActionMapType.Basic;
        if (Run.enabled) inputActionMapType |= InputActionMapType.Run;
        if (Code.enabled) inputActionMapType |= InputActionMapType.Code;
        return inputActionMapType;
    }

    public void SetInputActionMaps(InputActionMapType inputActionMapType)
    {
        if ((inputActionMapType & InputActionMapType.Basic) == InputActionMapType.Basic) Basic.Enable();
        else Basic.Disable();
        if ((inputActionMapType & InputActionMapType.Run) == InputActionMapType.Run) Run.Enable();
        else Run.Disable();
        if ((inputActionMapType & InputActionMapType.Code) == InputActionMapType.Code) Code.Enable();
        else Code.Disable();
    }

    void IDisposable.Dispose()
    {
        inputActions.Disable();
        inputActions.Dispose();
    }
}
