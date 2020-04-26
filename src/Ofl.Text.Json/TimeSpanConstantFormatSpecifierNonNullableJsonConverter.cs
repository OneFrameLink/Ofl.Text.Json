using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ofl.Text.Json
{
    public class TimeSpanConstantFormatSpecifierNonNullableJsonConverter : JsonConverter<TimeSpan>
    {
        #region Singleton

        public static readonly TimeSpanConstantFormatSpecifierNonNullableJsonConverter Instance =
            new TimeSpanConstantFormatSpecifierNonNullableJsonConverter();

        #endregion

        #region Constructor

        private TimeSpanConstantFormatSpecifierNonNullableJsonConverter()
        { }

        #endregion

        #region Overrides

        public override TimeSpan Read(
            ref Utf8JsonReader reader, 
            Type typeToConvert, 
            JsonSerializerOptions options
        )
        {
            // Validate parameters.
            if (typeToConvert == null) throw new ArgumentNullException(nameof(typeToConvert));
            if (options == null) throw new ArgumentNullException(nameof(options));

            // Get the string.
            string spanString = reader.GetString();

            // Parse.
            return TimeSpan.ParseExact(spanString, "c", CultureInfo.InvariantCulture);
        }

        public override void Write(
            Utf8JsonWriter writer, 
            TimeSpan value, 
            JsonSerializerOptions options
        )
        {
            // Validate parameters.
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (options == null) throw new ArgumentNullException(nameof(options));

            // Write the value.
            writer.WriteStringValue(value.ToString("c", CultureInfo.InvariantCulture));
        }

        #endregion
    }
}