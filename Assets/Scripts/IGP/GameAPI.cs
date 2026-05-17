using System.Collections;
using System.Collections.Generic;

namespace IGP
{
    public interface IInputProvider
    {
        bool GetKey(string key);
    }

    public interface IOutputProvider
    {
        void SetEntry(string key, bool value);
    }

    public class GameAPI
    {
        public readonly IInputProvider InputProvider;
        public readonly IOutputProvider OutputProvider;
        public readonly Logger Logger;

        public GameAPI(IInputProvider inputProvider, IOutputProvider outputProvider, Logger logger)
        {
            InputProvider = inputProvider;
            OutputProvider = outputProvider;
            Logger = logger;
        }
    }
}
