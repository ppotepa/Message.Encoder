using System;

namespace Message.Encoder.Exceptions
{
    public class InvalidPayloadPropertyNameException : Exception
    {
        public InvalidPayloadPropertyNameException(string message) : base(message)
        {
        }
    }
}
