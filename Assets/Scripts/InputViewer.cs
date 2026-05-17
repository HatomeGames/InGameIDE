using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class InputViewer : MonoBehaviour
{
    [SerializeField] TMP_Text inputViewText;

    List<string> pressedButtons = new();

    void Update()
    {
        pressedButtons.Clear();

        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            foreach (KeyControl key in keyboard.allKeys)
            {
                if (key.isPressed) pressedButtons.Add(key.displayName);
            }
        }

        var mouse = Mouse.current;
        if (mouse != null)
        {
            if (mouse.leftButton.isPressed) pressedButtons.Add("Mouse Left");
            if (mouse.rightButton.isPressed) pressedButtons.Add("Mouse Right");
            if (mouse.middleButton.isPressed) pressedButtons.Add("Mouse Middle");
            if (mouse.forwardButton.isPressed) pressedButtons.Add("Mouse Forward");
            if (mouse.backButton.isPressed) pressedButtons.Add("Mouse Back");
        }

        inputViewText.SetText(string.Join("\n", pressedButtons));
    }
}
