using System;

namespace Database
{
    public class NotEnoughDataException : Exception
    {
        public string ErrorMessage {get;}
        public NotEnoughDataException(string message)
        {
            ErrorMessage = message;
        }
    }
}
