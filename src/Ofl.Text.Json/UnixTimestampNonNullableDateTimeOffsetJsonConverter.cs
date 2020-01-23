using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ofl.Serialization.Json.Newtonsoft
{
    public class UnixTimestampNonNullableDateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
    {
        #region Read-only state

        public static readonly UnixTimestampNonNullableDateTimeOffsetJsonConverter Instance =
            new UnixTimestampNonNullableDateTimeOffsetJsonConverter();

        #endregion

        #region Constructor

        public UnixTimestampNonNullableDateTimeOffsetJsonConverter()
        { }

        #endregion

        #region Overrides of JsonConverter

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            // Validate parameters.
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (options == null) throw new ArgumentNullException(nameof(options));

            // Write the value.
            writer.WriteNumberValue(value.ToUnixTimeSeconds());
        }

        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Validate parameters.
            if (typeToConvert == null) throw new ArgumentNullException(nameof(typeToConvert));
            if (options == null) throw new ArgumentNullException(nameof(options));

            // Get the int64, this is the seconds.
            long seconds = reader.GetInt64();

            // Return the date time offset.
            return DateTimeOffset.FromUnixTimeSeconds(seconds);
        }

        #endregion
    }
}
