using System;

namespace Message.Encoder.Exceptions
{
    internal class InvalidSerializationOrderException : Exception
    {
        public InvalidSerializationOrderException(string message) : base(message)
        {
        }
    }
}
