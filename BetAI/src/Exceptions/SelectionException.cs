using System;

namespace BetAI.Exceptions
{
    [Serializable]
    class SelectionException: Exception
    {
        public string Error { get; }

        public SelectionException(string message)
        {
            Error = message;
        }
    }
}
