using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace IGP
{
    public class OutputProvider : IOutputProvider
    {
        Agent agent;

        public OutputProvider(Agent agent)
        {
            this.agent = agent;
        }

        public void SetEntry(string key, bool value)
        {
            if (!value) return;
            switch (key)
            {
                case "MoveForward":
                    agent.AddForce(Vector3.forward);
                    break;
                case "MoveBack":
                    agent.AddForce(Vector3.back);
                    break;
                case "MoveRight":
                    agent.AddForce(Vector3.right);
                    break;
                case "MoveLeft":
                    agent.AddForce(Vector3.left);
                    break;
                case "RotateRight":
                    agent.AddTorque(Vector3.up);
                    break;
                case "RotateLeft":
                    agent.AddTorque(Vector3.down);
                    break;
            }
        }
    }
}
