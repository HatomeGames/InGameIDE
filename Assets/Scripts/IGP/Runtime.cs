using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace IGP
{
    public class Runtime : MonoBehaviour
    {
        [SerializeField] Agent agent;

        GameAPI gameAPI;
        Executable executable;
        bool pausing = false;

        [Inject]
        public void Construct(GameInput gameInput, Logger logger)
        {
            var inputProvider = new InputProvider(gameInput);
            var outputProvider = new OutputProvider(agent);
            gameAPI = new(inputProvider, outputProvider, logger);
        }

        public void Execute(Executable executable)
        {
            agent.ResetPosture();

            executable.Inject(gameAPI);

            executable.Init();

            this.executable = executable;
        }

        void Update()
        {
            if (executable == null) return;
            if (pausing) return;

            executable.Tick(Time.deltaTime);
        }

        public void SetPause(bool pause)
        {
            pausing = pause;
        }

        public void Kill()
        {
            executable = null;
        }
    }
}
