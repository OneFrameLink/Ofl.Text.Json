using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;

namespace Ofl.Serialization.Json.Newtonsoft
{
    public class Iso8601DurationNonNullableTimeSpanJsonConverter : JsonConverter<TimeSpan>
    {
        #region Singleton

        public static readonly Iso8601DurationNonNullableTimeSpanJsonConverter Instance =
            new Iso8601DurationNonNullableTimeSpanJsonConverter();

        #endregion

        #region Constructor

        private Iso8601DurationNonNullableTimeSpanJsonConverter()
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

            // Convert.
            return XmlConvert.ToTimeSpan(spanString);
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
            writer.WriteStringValue(XmlConvert.ToString(value));
        }

        #endregion
    }
}