using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace IGP
{
    public class InputProvider : IInputProvider
    {
        GameInput gameInput;

        public InputProvider(GameInput gameInput)
        {
            this.gameInput = gameInput;
        }

        public bool GetKey(string key)
        {
            return key switch
            {
                "Up" => gameInput.Run.Up.IsPressed(),
                "Down" => gameInput.Run.Down.IsPressed(),
                "Right" => gameInput.Run.Right.IsPressed(),
                "Left" => gameInput.Run.Left.IsPressed(),
                "Fire" => gameInput.Run.Fire.IsPressed(),
                "Aim" => gameInput.Run.Aim.IsPressed(),
                "Jump" => gameInput.Run.Jump.IsPressed(),
                "Dash" => gameInput.Run.Dash.IsPressed(),
                _ => false,
            };
        }
    }
}
