using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ofl.Text.Json
{
    public class UnixTimestampNullableDateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset?>
    {
        #region Read-only state

        public static readonly UnixTimestampNullableDateTimeOffsetJsonConverter Instance =
            new UnixTimestampNullableDateTimeOffsetJsonConverter();

        #endregion

        #region Constructor

        private UnixTimestampNullableDateTimeOffsetJsonConverter()
        { }

        #endregion

        #region Overrides of JsonConverter

        public override void Write(Utf8JsonWriter writer, DateTimeOffset? value, JsonSerializerOptions options)
        {
            // Validate parameters.
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (options == null) throw new ArgumentNullException(nameof(options));

            // If the value is null, then write null.
            if (value == null)
                // Write null.
                writer.WriteNullValue();
            else
                // Write the value.
                writer.WriteNumberValue(value.Value.ToUnixTimeSeconds());
        }

        public override DateTimeOffset? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Validate parameters.
            if (typeToConvert == null) throw new ArgumentNullException(nameof(typeToConvert));
            if (options == null) throw new ArgumentNullException(nameof(options));

            // If the token is null, return null.
            if (reader.TokenType == JsonTokenType.Null) return null;

            // Get the int64, this is the seconds.
            long seconds = reader.GetInt64();

            // Return the date time offset.
            return DateTimeOffset.FromUnixTimeSeconds(seconds);
        }

        #endregion
    }
}
