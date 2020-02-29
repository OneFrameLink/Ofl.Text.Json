using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Ofl.Text.Json
{
    internal class ImmutableDictionaryJsonConverterExtensions
    {
        public static (
            Func<string, JsonSerializerOptions, TKey> KeyDeserializer,
            ValueDeserializer<TValue> ValueDeserializer
        ) GetImmutableDictionaryHelpers<TKey, TValue>() => (
            GetKeyDeserializer<TKey>(),
            GetValueDeserializer<TValue>()
        );


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
            if (typeof(T) == typeof(object))
                // Force to T (which is object)
                return (v, o) => (T) (object) v;

            // Can deserialize normally.
            return JsonSerializer.Deserialize<T>;
        }

        #endregion
    }
}
