using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ofl.Text.Json
{
    public class ImmutableDictionaryJsonConverter<TKey, TValue> : JsonConverter<IImmutableDictionary<TKey, TValue>>
        where TKey : notnull
    {
        #region Constructor

        public ImmutableDictionaryJsonConverter()
        {
            // Get the helpers.
            (_keyDeserializer, _valueDeserializer) = ImmutableDictionaryJsonConverterExtensions
                .GetImmutableDictionaryHelpers<TKey, TValue>();
        }


        #endregion

        #region Instance, read-only state

        private readonly Func<string, JsonSerializerOptions, TKey> _keyDeserializer;

        private readonly ImmutableDictionaryJsonConverterExtensions.ValueDeserializer<TValue> _valueDeserializer;

        #endregion

        #region Overrides

        public override IImmutableDictionary<TKey, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Validate parameters.
            if (options == null) throw new ArgumentNullException(nameof(options));

            // Is this null?  If so, return null.
            if (reader.TokenType == JsonTokenType.Null) return null!;

            // If it is not an object, throw.
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException($"Expected a token type of {nameof(JsonTokenType.StartObject)}, encountered {reader.TokenType}.");

            // The return value.
            IImmutableDictionary<TKey, TValue> values = ImmutableDictionary<TKey, TValue>.Empty;

            // Cycle.
            while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
            {
                // We should be on the name now.  Get the string.
                string stringKey = reader.GetString();

                // Convert the key.
                TKey key = _keyDeserializer(stringKey, options);

                // Move the reader now.
                if (!reader.Read())
                    throw new JsonException($"Expected call to {nameof(Utf8JsonReader.Read)} to return true, returned false.");

                // Deserialize the value.
                TValue value = _valueDeserializer(ref reader, options);

                // Add.
                values = values.Add(key, value);
            }

            // If the token is not an end object, throw.
            if (reader.TokenType != JsonTokenType.EndObject)
                throw new JsonException($"Expected a token of {JsonTokenType.EndObject}, actual {reader.TokenType}.");

            // Return the values.
            return values;
        }

        public override void Write(Utf8JsonWriter writer, IImmutableDictionary<TKey, TValue> value, JsonSerializerOptions options)
        {
            // Validate parameters.
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            if (options == null) throw new ArgumentNullException(nameof(options));
            
            // If the value is null, write null and get out.
            if (value == null)
            {
                // Write null and get out.
                writer.WriteNullValue();
                return;
            }

            // Write the start object.
            writer.WriteStartObject();

            // Cycle through the pairs.
            foreach (KeyValuePair<TKey, TValue> pair in value)
            {
                // Get the key property.
                string key = JsonSerializer.Serialize(pair.Key, pair.Key.GetType(), options);

                // Run through the dictionary json policy, if there is one.
                key = options.DictionaryKeyPolicy?.ConvertName(key) ?? key;

                // Write the property and the value.
                writer.WritePropertyName(key);

                // Write the value.
                JsonSerializer.Serialize(writer, pair.Value, options);
            }

            // Close the object off.
            writer.WriteEndObject();
        }

        #endregion
    }
}