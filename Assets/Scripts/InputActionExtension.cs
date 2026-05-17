using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public static class InputActionExtension
{
    static public IDisposable SubscribePerform(this InputAction action, Action<InputAction.CallbackContext> callback)
    {
        return new SubscribeInputActionPerform(action, callback);
    }
    static public IDisposable SubscribeStart(this InputAction action, Action<InputAction.CallbackContext> callback)
    {
        return new SubscribeInputActionStart(action, callback);
    }
    static public IDisposable SubscribeCancel(this InputAction action, Action<InputAction.CallbackContext> callback)
    {
        return new SubscribeInputActionCancel(action, callback);
    }
}

public class SubscribeInputActionPerform : IDisposable
{
    readonly InputAction action;
    readonly Action<InputAction.CallbackContext> callback;

    public SubscribeInputActionPerform(InputAction action, Action<InputAction.CallbackContext> callback)
    {
        action.performed += callback;
        this.action = action;
        this.callback = callback;
    }

    public void Dispose()
    {
        action.performed -= callback;
    }
}

public class SubscribeInputActionStart : IDisposable
{
    readonly InputAction action;
    readonly Action<InputAction.CallbackContext> callback;

    public SubscribeInputActionStart(InputAction action, Action<InputAction.CallbackContext> callback)
    {
        action.started += callback;
        this.action = action;
        this.callback = callback;
    }

    public void Dispose()
    {
        action.started -= callback;
    }
}

public class SubscribeInputActionCancel : IDisposable
{
    readonly InputAction action;
    readonly Action<InputAction.CallbackContext> callback;

    public SubscribeInputActionCancel(InputAction action, Action<InputAction.CallbackContext> callback)
    {
        action.canceled += callback;
        this.action = action;
        this.callback = callback;
    }

    public void Dispose()
    {
        action.canceled -= callback;
    }
}
