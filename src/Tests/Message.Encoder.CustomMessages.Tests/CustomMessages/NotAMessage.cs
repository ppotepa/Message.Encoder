using Message.Encoder.Attributes;
// ReSharper disable All

namespace Message.Encoder.CustomMessages.Tests.CustomMessages
{
    // classese were made for Test purpose only,
    // marked as partial so VS does not spit out a Warning
    [MessageType(MessageTypeCode = 200)]
    internal partial class NotAMessage
    {
    }

    internal partial class NotAHeader
    { 
    }

    internal partial class NotAPayload
    {
    }
}
