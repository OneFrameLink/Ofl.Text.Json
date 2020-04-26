using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Ofl.Text.Json
{
    internal static class DictionaryJsonConverterExtensions
    {
        public static (
            Func<string, JsonSerializerOptions, TKey> KeyDeserializer,
            ValueDeserializer<TValue> ValueDeserializer
        ) GetImmutableDictionaryHelpers<TKey, TValue>() => (
            GetKeyDeserializer<TKey>(),
            GetValueDeserializer<TValue>()
        );

        public static void Write<TKey, TValue>(
            this Utf8JsonWriter writer,
            IReadOnlyDictionary<TKey, TValue> value,
            JsonSerializerOptions options
        )
            where TKey : notnull
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
                // Get the key property.  If the key is of type string, then
                // just use that.
                string key = pair.Key is string s
                    ? s
                    : JsonSerializer.Serialize(pair.Key, pair.Key.GetType(), options);

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

        #region Helpers

        public delegate T ValueDeserializer<T>(ref Utf8JsonReader reader, JsonSerializerOptions options);

        private static ValueDeserializer<T> GetValueDeserializer<T>()
        {
            // If this is an object, special case.
            if (typeof(T) == typeof(object))
                // Render as a JsonElement.
                return (ref Utf8JsonReader r, JsonSerializerOptions o) => 
                    (T) (object) JsonSerializer.Deserialize<JsonElement>(ref r, o);

            // Deserialize normally.
            return JsonSerializer.Deserialize<T>;
        }

        private static Func<string, JsonSerializerOptions, T> GetKeyDeserializer<T>()
        {
            // If this is an object, special case.
            // Also if the key is of type string then just return the string.
            if (typeof(T) == typeof(object) || typeof(T) == typeof(string))
                // Force to T (which is object)
                return (v, o) => (T) (object) v;

            // Can deserialize normally.
            return JsonSerializer.Deserialize<T>;
        }

        #endregion
    }
}
