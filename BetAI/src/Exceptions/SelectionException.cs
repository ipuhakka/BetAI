using System;

namespace BetAI.Exceptions
{
    class SelectionException: Exception
    {
        public string error { get; }

        public SelectionException(string message)
        {
            error = message;
        }
    }
}
