using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ofl.Text.Json
{
    public class TimeSpanConstantFormatSpecifierNullableJsonConverter : JsonConverter<TimeSpan?>
    {
        #region Singleton

        public static readonly TimeSpanConstantFormatSpecifierNullableJsonConverter Instance =
            new TimeSpanConstantFormatSpecifierNullableJsonConverter();

        #endregion

        #region Constructor

        private TimeSpanConstantFormatSpecifierNullableJsonConverter()
        { }

        #endregion

        #region Overrides

        public override TimeSpan? Read(
            ref Utf8JsonReader reader, 
            Type typeToConvert, 
            JsonSerializerOptions options
        )
        {
            // Validate parameters.
            if (typeToConvert == null) throw new ArgumentNullException(nameof(typeToConvert));
            if (options == null) throw new ArgumentNullException(nameof(options));

            // If this is a null token, return null.
            if (reader.TokenType == JsonTokenType.Null) return null;

            // Get the string.
            string? spanString = reader.GetString();

            // If the string is null return null.
            if (spanString == null) return null;

            // Parse.
            return TimeSpan.ParseExact(spanString, "c", CultureInfo.InvariantCulture);
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan? value, JsonSerializerOptions options)
        {
            // Validate parameters.
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (options == null) throw new ArgumentNullException(nameof(options));

            // If null, write null.
            if (value == null)
                // Write null.
                writer.WriteNullValue();
            else
                // Write the value.
                writer.WriteStringValue(value.Value.ToString("c", CultureInfo.InvariantCulture));
        }

        #endregion
    }
}