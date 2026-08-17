using System;
using System.Runtime.Serialization;

namespace Message.Encoder.Extensions
{
    public class InvalidHeadersLengthException : Exception
    {
        public InvalidHeadersLengthException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}