using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace Ofl.Serialization.Json.Newtonsoft
{
    public class Iso8601DurationNullableTimeSpanJsonConverter : JsonConverter<TimeSpan?>
    {
        #region Singleton

        public static readonly Iso8601DurationNullableTimeSpanJsonConverter Instance =
            new Iso8601DurationNullableTimeSpanJsonConverter();

        #endregion

        #region Constructor

        private Iso8601DurationNullableTimeSpanJsonConverter()
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

            // Convert.
            return XmlConvert.ToTimeSpan(spanString);
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
                writer.WriteStringValue(XmlConvert.ToString(value.Value));
        }

        #endregion
    }
}