using System;

namespace Message.Encoder.Exceptions
{
    public class InvalidAmountOfHeadersSuppliedException : Exception
    {
        public InvalidAmountOfHeadersSuppliedException(string message) : base(message)
        {
        }
    }
}