using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ofl.Serialization.Json.Newtonsoft
{
    public class Iso8601DurationTimeSpanJsonConverter : JsonConverterFactory
    {
        #region Overrides

        public override bool CanConvert(Type typeToConvert)
            => typeToConvert == typeof(TimeSpan) || typeToConvert == typeof(TimeSpan?);

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            // Validate parameters.
            if (typeToConvert == null) throw new ArgumentNullException(nameof(typeToConvert));
            if (options == null) throw new ArgumentNullException(nameof(options));

            // If this is a non null type return.
            if (typeToConvert == typeof(TimeSpan))
                return Iso8601DurationNonNullableTimeSpanJsonConverter.Instance;

            // Is this nullable?
            if (typeToConvert == typeof(TimeSpan?))
                return Iso8601DurationNullableTimeSpanJsonConverter.Instance;

            // Throw.
            throw new InvalidOperationException(
                $"The type {typeToConvert.FullName} is not supported by {nameof(Iso8601DurationTimeSpanJsonConverter)}.");
        }

        #endregion
    }
}