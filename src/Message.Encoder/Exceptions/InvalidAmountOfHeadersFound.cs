using System;

namespace Message.Encoder.Exceptions
{
    public class InvalidAmountOfHeadersFound : Exception
    {
        public InvalidAmountOfHeadersFound(string message) : base(message)
        {
        }
    }
}