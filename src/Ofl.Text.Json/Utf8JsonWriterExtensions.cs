using System;
using System.Reflection;
using System.Text.Json;

namespace Ofl.Text.Json
{
    public static class Utf8JsonWriterExtensions
    {
        public static void WritePropertyName(
            this Utf8JsonWriter writer,
            PropertyInfo property,
            JsonSerializerOptions options
        )
        {
            // Validate parameters.
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (options == null) throw new ArgumentNullException(nameof(options));

            // Get the property name.
            string name = property.GetPropertyName(options);

            // Write the property name.
            writer.WritePropertyName(name);
        }

        public static void WriteString(
            this Utf8JsonWriter writer,
            PropertyInfo property,
            string? value,
            JsonSerializerOptions options
        )
        {
            // Validate parameters.
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (property == null) throw new ArgumentNullException(nameof(property));

            // Write the value.
            writer.WriteString(property.GetPropertyName(options), value);
        }
        public static void WriteNumber(
            this Utf8JsonWriter writer,
            PropertyInfo property,
            int value,
            JsonSerializerOptions options
        )
        {
            // Validate parameters.
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (property == null) throw new ArgumentNullException(nameof(property));

            // Write the value.
            writer.WriteNumber(property.GetPropertyName(options), value);
        }
    }
}
