using System;

namespace Message.Encoder.Exceptions;

public class InvalidAmountOfPayloadPropertiesSuppliedException : Exception
{
    public InvalidAmountOfPayloadPropertiesSuppliedException(string message) : base(message)
    {
    }

}