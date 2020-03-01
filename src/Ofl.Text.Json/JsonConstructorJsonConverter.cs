using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ofl.Text.Json
{
    public class JsonConstructorJsonConverter : JsonConverterFactory
    {
        #region Overrides

        public override bool CanConvert(Type typeToConvert)
        {
            // Validate parameters.
            if (typeToConvert == null)
                throw new ArgumentNullException(nameof(typeToConvert));

            // Is there a constructor this can handle?  If so, return true
            // otherwise, return false.
            return typeToConvert.GetJsonConstructor() != null;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            // Validate parameters.
            if (typeToConvert == null) throw new ArgumentNullException(nameof(typeToConvert));

            // Make the converter.
            Type converterType = typeof(JsonConstructorJsonConverter<>).MakeGenericType(typeToConvert);

            // Get the constructor info.
            ConstructorInfo? constructorInfo = typeToConvert.GetJsonConstructor();

            // If null, throw.
            if (constructorInfo == null)
                throw new InvalidOperationException(
                    $"Call to {nameof(CreateConverter)} passed a type of {typeToConvert.FullName} which returned " +
                    $"true when passed to {nameof(CanConvert)} but did not return a {nameof(ConstructorInfo)} when " +
                    $"looking for JSON constructors.");

            // Create an instance.
            return (JsonConverter) Activator.CreateInstance(converterType, constructorInfo);
        }

        #endregion
    }
}
