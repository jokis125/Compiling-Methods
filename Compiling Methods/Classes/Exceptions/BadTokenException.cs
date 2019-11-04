using System;

namespace CompilingMethods.Classes.Exceptions
{
    public class BadTokenException : Exception
    {
        public BadTokenException()
        {
        }

        public BadTokenException(string message)
            : base(message)
        {
        }

        public BadTokenException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}