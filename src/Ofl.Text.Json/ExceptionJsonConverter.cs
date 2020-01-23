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


        #endregion
    }
}
