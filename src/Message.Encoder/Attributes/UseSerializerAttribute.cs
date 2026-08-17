using System;

namespace Message.Encoder.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class UseSerializerAttribute : Attribute
    {
        public UseSerializerAttribute(Type serializer)
        {
            Serializer = serializer;
        }

        public Type Serializer { get; set; }
    }
}