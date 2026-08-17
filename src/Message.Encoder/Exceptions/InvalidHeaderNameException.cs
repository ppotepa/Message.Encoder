using System;

namespace Message.Encoder.Exceptions
{
    public class InvalidHeaderNameException : Exception
    {
        public InvalidHeaderNameException(string message) : base(message)
        {
        }
    }
}