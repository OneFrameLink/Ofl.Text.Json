using System;
using System.Buffers;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace Ofl.Text.Json
{
    public class JsonConstructorJsonConverter<T> : JsonConverter<T>
    {
        #region Constructor

        public JsonConstructorJsonConverter(ConstructorInfo constructorInfo)
        {
            // Validate parameters.
            _constructorInfo = constructorInfo
                ?? throw new ArgumentNullException(nameof(constructorInfo));

            // Assign values.
            _parameterInfos = constructorInfo.GetParameters();
        }

        #endregion

        #region Instance, read-only state

        private readonly ConstructorInfo _constructorInfo;

        private readonly IReadOnlyList<ParameterInfo> _parameterInfos;

        #endregion

        #region Overrides

        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Validate parameters.
            if (typeToConvert == null) throw new ArgumentNullException(nameof(typeToConvert));
            if (options == null) throw new ArgumentNullException(nameof(options));

            // If the reader is not on a start object, throw.
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException($"Expected {nameof(JsonTokenType.StartObject)} when reading JSON tokens, found {reader.TokenType}.");

            // Converts the name.
            string ConvertParameterName(ParameterInfo parameterInfo) {
                // We will map the first character to upper case, as that is the expectation (pascal-cased)
                // and then run it through the naming policy.
                string name = string.Create(parameterInfo.Name.Length, 0, (s, _) =>{
                    // Copy to the span.
                    parameterInfo.Name.AsSpan().CopyTo(s);

                    // Convert the first character to upper case.
                    if (!char.IsUpper(s[0])) s[0] = char.ToUpper(s[0], CultureInfo.InvariantCulture);
                });

                // Run through the policy if there is one.
                return options.PropertyNamingPolicy?.ConvertName(name) ?? name;
            }

            // Store the parameters, map by converted name.
            IReadOnlyDictionary<string, (int ordinal, ParameterInfo parameterInfo)> parametersByName = _parameterInfos
                .Select((p, i) => (i, p))
                .ToDictionary(p => ConvertParameterName(p.p));

            // The parameter values.
            var parameterValues = new Dictionary<string, object>();

            // Read to the next token.
            reader.GuaranteedRead();

            // Cycle while there is a property.
            while (reader.TokenType == JsonTokenType.PropertyName)
            {
                // Get the property name.
                string propertyName = reader.GetString();

                // Go to the next token.
                reader.GuaranteedRead();

                // Get the parameter, if it doesn't exist, we can skip this.
                if (!parametersByName.TryGetValue(propertyName, out (int ordinal, ParameterInfo parameter) pair))
                {
                    // Skip.  If the token type is start object or array, skip that.
                    if (reader.TokenType == JsonTokenType.StartArray || reader.TokenType == JsonTokenType.StartObject)
                        // Skip.
                        reader.SkipObjectOrArray();
                    else
                    {
                        // Read while the next token is not a comment.
                        // TODO: Handle comments.
                        // Skip token for now.
                        reader.GuaranteedRead();
                    }

                    // Go to the next parameter.
                    continue;
                }

                // Deserialize.
                object parameterValue = JsonSerializer.Deserialize(ref reader, pair.parameter.ParameterType, options);

                // Add to the values.
                parameterValues.Add(propertyName, parameterValue);

                // Read to the next token.
                reader.GuaranteedRead();
            }

            // If the reader is not on a start object, throw.
            if (reader.TokenType != JsonTokenType.EndObject)
                throw new JsonException($"Expected a JSON token type of {nameof(JsonTokenType.EndObject)} when reading JSON tokens, encountered {reader.TokenType}.");

            // Get the array of parameters.
            object[] parameters = parametersByName
                .OrderBy(p => p.Value.ordinal)
                .Select(p => {
                    // Try and get the value, null is ok, it will cause
                    // a default to be passed to the constructor.
                    parameterValues.TryGetValue(p.Key, out object value);

                    // Return the value.
                    return value;
                })
                .ToArray();

            // Call the constructor, cast it back,
            return (T) _constructorInfo.Invoke(parameters);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            // Validate parameters.
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (options == null) throw new ArgumentNullException(nameof(options));

            // Write the start object.
            writer.WriteStartObject();

            // As per:
            // https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to#serialization-behavior
            // All public properties are serialized, so get those first.
            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties();

            // Cycle through and write.
            foreach (PropertyInfo property in properties)
            {
                // Write the name.
                writer.WritePropertyName(property, options);

                // Get the value.
                object propertyValue = property.GetValue(value);

                // Write the value.
                JsonSerializer.Serialize(writer, propertyValue, property.PropertyType, options);                
            }

            // Write the end object.
            writer.WriteEndObject();
        }

        #endregion
    }
}
