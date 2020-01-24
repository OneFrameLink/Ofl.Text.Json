using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ofl.Text.Json
{
    public class UnixTimestampDateTimeOffsetJsonConverter : JsonConverterFactory
    {
        #region Overrides

        public override bool CanConvert(Type typeToConvert) =>
            typeToConvert == typeof(DateTimeOffset) || typeToConvert == typeof(DateTimeOffset?);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            // Validate parameters.
            if (typeToConvert == null) throw new ArgumentNullException(nameof(typeToConvert));
            if (options == null) throw new ArgumentNullException(nameof(typeToConvert));

            // If this is a non null type return.
            if (typeToConvert == typeof(DateTimeOffset))
                return UnixTimestampNonNullableDateTimeOffsetJsonConverter.Instance;

            // Is this nullable?
            if (typeToConvert == typeof(DateTimeOffset?))
                return UnixTimestampNullableDateTimeOffsetJsonConverter.Instance;

            // Throw.
            throw new InvalidOperationException(
                $"The type {typeToConvert.FullName} is not supported by {nameof(UnixTimestampDateTimeOffsetJsonConverter)}.");
        }

        #endregion
    }
}
