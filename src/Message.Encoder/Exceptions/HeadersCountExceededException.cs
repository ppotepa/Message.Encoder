using System;

namespace Message.Encoder.Exceptions
{
    public class HeadersCountExceededException : Exception
    {
        public HeadersCountExceededException(string message) : base(message)
        {
        }
    }
}