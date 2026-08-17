using Message.Encoder.Attributes;
using Message.Encoder.Exceptions;
using System;
using System.Linq;
using System.Reflection;

namespace Message.Encoder.Metadata.Serialization
{
    internal class SerializationMetadata
    {
        public readonly bool IsNullable;

        public SerializationMetadata(SerializationOrderAttribute attribute, PropertyInfo propertyInfo, object preactivated, bool isNullable)
        {
            this.Attribute = attribute;
            this.PropertyInfo = propertyInfo;
            this.Preactivated = preactivated;
            this.IsNullable = isNullable;
        }

        public SerializationOrderAttribute Attribute { get; }
        public object Preactivated { get; }
        public PropertyInfo PropertyInfo { get; }

        internal static SerializationMetadata[] Create(Type type)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));

            PropertyInfo[] properties = type.GetProperties();

            var propertiesWithAttributes = properties
                .Select(prop => new
                {
                    Property = prop,
                    Attribute = prop.GetCustomAttribute<SerializationOrderAttribute>()
                })
                .Where(item => item.Attribute is not null)
                .Select(item => new SerializationMetadata
                (
                    attribute: item.Attribute!,
                    propertyInfo: item.Property,
                    preactivated: GetUninitializedObject(item.Property),
                    isNullable: Nullable.GetUnderlyingType(item.Property.PropertyType) != null
                ))
                .OrderBy(metadata => metadata.Attribute.Order)
                .ToArray();

            var grouped = propertiesWithAttributes.GroupBy(prop => prop.Attribute.Order);
            var moreThanOnce = grouped.Where(group => group.Count() > 1).ToArray();

            if (moreThanOnce.Any())
            {
                var message = string.Join(", ", moreThanOnce.Select(x => x.Key));

                throw new InvalidSerializationOrderException(
                   $"Unable to build metadata. Some property orders are duplicated: {message}."
                );
            }

            return propertiesWithAttributes;
        }

        private static dynamic GetUninitializedObject(PropertyInfo prop)
        {
            if (prop.PropertyType == typeof(string)) return "";

            if (prop.PropertyType == typeof(long)) return (long)0;
            if (prop.PropertyType == typeof(long?)) return new long?(0);

            if (prop.PropertyType == typeof(int)) return (int)0;
            if (prop.PropertyType == typeof(int?)) return new int?(0);

            if (prop.PropertyType == typeof(short)) return (short)0;
            if (prop.PropertyType == typeof(short?)) return new short?(0);

            if (prop.PropertyType == typeof(byte)) return (byte)0;
            if (prop.PropertyType == typeof(byte?)) return new byte?(0);

            if (prop.PropertyType == typeof(bool)) return false;
            if (prop.PropertyType == typeof(bool?)) return new bool?(false);

            if (prop.PropertyType == typeof(float)) return (float)1;
            if (prop.PropertyType == typeof(float?)) return new float?(1);

            if (prop.PropertyType == typeof(double)) return (double)1;
            if (prop.PropertyType == typeof(double?)) return new double?(1);

            throw new ArgumentException($"Invalid type supplied {prop.PropertyType.Name}.");
        }
    }
}
