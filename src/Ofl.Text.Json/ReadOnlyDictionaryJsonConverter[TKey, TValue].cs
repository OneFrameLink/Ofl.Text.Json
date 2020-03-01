using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ofl.Text.Json
{
    public class ReadOnlyDictionaryJsonConverter<TKey, TValue> : JsonConverter<IReadOnlyDictionary<TKey, TValue>>
        where TKey : notnull
    {
        #region Constructor

        public ReadOnlyDictionaryJsonConverter()
        {
            // Get the helpers.
            (_keyDeserializer, _valueDeserializer) = DictionaryJsonConverterExtensions
                .GetImmutableDictionaryHelpers<TKey, TValue>();
        }


        #endregion

        #region Instance, read-only state

        private readonly Func<string, JsonSerializerOptions, TKey> _keyDeserializer;

        private readonly DictionaryJsonConverterExtensions.ValueDeserializer<TValue> _valueDeserializer;

        #endregion

        #region Overrides

        public override IReadOnlyDictionary<TKey, TValue> Read(
            ref Utf8JsonReader reader, 
            Type typeToConvert, 
            JsonSerializerOptions options
        )
        {
            // Validate parameters.
            if (options == null) throw new ArgumentNullException(nameof(options));

            // Is this null?  If so, return null.
            if (reader.TokenType == JsonTokenType.Null) return null!;

            // If it is not an object, throw.
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException($"Expected a token type of {nameof(JsonTokenType.StartObject)}, encountered {reader.TokenType}.");

            // The return value.
            var values = new Dictionary<TKey, TValue>();

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
                values.Add(key, value);
            }

            // If the token is not an end object, throw.
            if (reader.TokenType != JsonTokenType.EndObject)
                throw new JsonException($"Expected a token of {JsonTokenType.EndObject}, actual {reader.TokenType}.");

            // Return the values.
            return new ReadOnlyDictionary<TKey, TValue>(values);
        }

        public override void Write(
            Utf8JsonWriter writer,
            IReadOnlyDictionary<TKey, TValue> value,
            JsonSerializerOptions options
        ) => writer.Write(value, options);

        #endregion
    }
}