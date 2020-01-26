using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ofl.Text.Json
{
    public class ExceptionJsonConverter : JsonConverter<Exception?>
    {
        #region Overrides

        public override void Write(Utf8JsonWriter writer, Exception? value, JsonSerializerOptions options)
        {
            // Validate parameters.
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (options == null) throw new ArgumentNullException(nameof(options));

            // Write.
            writer.WriteException(value, options);
        }

        public override Exception Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => throw new JsonException($"Deserializing exceptions with the {nameof(ExceptionJsonConverter)} is not supported at this time.");

        public override bool CanConvert(Type typeToConvert)
        {
            // Validate parameters.
            if (typeToConvert == null) throw new ArgumentNullException(nameof(typeToConvert));

            // Is this assignable to exception?  If so, then true, all
            // exceptions count.
            if (typeof(Exception).IsAssignableFrom(typeToConvert))
                return true;

            // Call the base.
            return base.CanConvert(typeToConvert);
        }

        #endregion
    }
}
