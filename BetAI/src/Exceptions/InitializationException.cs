using System;

namespace BetAI.Exceptions
{
    [Serializable]
    public class InitializationException: Exception
    {
        public string Error { get; private set; }
        public InitializationException()
        {

        }

        public InitializationException(string message)
        {
            Error = message;
        }
    }
}
