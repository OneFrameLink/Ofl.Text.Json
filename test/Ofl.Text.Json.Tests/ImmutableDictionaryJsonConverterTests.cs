using Ofl.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using Xunit;

namespace Ofl.Text.Json.Tests
{
    public class ImmutableDictionaryJsonConverterTests
    {
        #region Helpers

        public static JsonSerializerOptions CreateJsonSerializerOptions() =>
            new JsonSerializerOptions {
                Converters = { new ImmutableDictionaryJsonConverter() }
            };

        private static void AssertSerializedJson<TKey, TValue>(
            IImmutableDictionary<TKey, TValue> expected
        )
            where TKey : notnull
        {
            // Create the options.
            JsonSerializerOptions options = CreateJsonSerializerOptions();

            // Serialize.
            string json = JsonSerializer.Serialize(expected, options);

            // Deserialize into another immutable dictionary.
            // We are essentially round tripping.
            IImmutableDictionary<TKey, TValue> actual = JsonSerializer
                .Deserialize<IImmutableDictionary<TKey, TValue>>(json, options);

            // Counts differ?  Bail.
            Assert.Equal(expected.Count, actual.Count);

            // Make sure everything is equal.
            foreach (KeyValuePair<TKey, TValue> pair in expected)
            {
                // Assert everything is equal.
                Assert.True(actual.TryGetValue(pair.Key, out TValue value));
                Assert.Equal(pair.Value, value);
            }
        }

        private static void AssertSerializedJson(
            IImmutableDictionary<object, object> expected
        )
        {
            // Create the options.
            JsonSerializerOptions options = CreateJsonSerializerOptions();

            // Serialize.
            string json = JsonSerializer.Serialize(expected, options);

            // Deserialize into another immutable dictionary.
            // We are essentially round tripping.
            IImmutableDictionary<object, object> actual = JsonSerializer
                .Deserialize<IImmutableDictionary<object, object>>(json, options);

            // Counts differ?  Bail.
            Assert.Equal(expected.Count, actual.Count);

            // Make sure everything is equal.
            foreach (KeyValuePair<object, object> pair in expected)
            {
                // Serialize the key.
                string key = JsonSerializer.Serialize(pair.Key, options);

                // Create a JsonElement from the value in the expected.
                json = JsonSerializer.Serialize(pair.Value, options);

                // Deserialize.
                JsonElement expectedElement = JsonSerializer.Deserialize<JsonElement>(json, options);

                // Assert everything is equal.
                Assert.True(actual.TryGetValue(key, out object? value));

                // Get the actual element.
                var actualElement = (JsonElement) value!;

                // Assert.
                Assert.Equal(expectedElement.ValueKind, actualElement.ValueKind);
                Assert.Equal(expectedElement.GetRawText(), actualElement.GetRawText());
            }
        }

        #endregion

        #region Tests

        [Fact]
        public void Test_ImmutableDictionary_Serialization_Int32_Key_Round_Trip()
        {
            // Create the test.
            var test = ImmutableDictionary.CreateRange(EnumerableExtensions.From(
                new KeyValuePair<int, string>(1, "hello"),
                new KeyValuePair<int, string>(2, "there")
            ));

            // Assert 
            AssertSerializedJson(test);
        }

        [Fact]
        public void Test_ImmutableDictionary_Serialization_Object_Key_Round_Trip()
        {
            // Create the test.
            var test = ImmutableDictionary.CreateRange(EnumerableExtensions.From(
                new KeyValuePair<object, object>(1, "hello"),
                new KeyValuePair<object, object>(2, "there")
            ));

            // Assert 
            AssertSerializedJson(test);
        }

        #endregion
    }
}
