using System;
using System.Runtime.Serialization;

namespace CompilingMethods.Classes.Exceptions
{
    public class UndeclaredVariableException : Exception
    {
        public UndeclaredVariableException()
        {
        }

        protected UndeclaredVariableException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public UndeclaredVariableException(string message) : base(message)
        {
        }
    }
}