using Ofl.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using Xunit;

namespace Ofl.Text.Json.Tests
{
    public class ImmutableDictionaryJsonConverterFactoryTests
    {
        #region Helpers

        public static JsonSerializerOptions CreateJsonSerializerOptions() =>
            new JsonSerializerOptions {
                Converters = { new ImmutableDictionaryJsonConverterFactory() }
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
        public void Test_ImmutableDictionary_Serialization_DateTimeOffset_Key_Round_Trip()
        {
            // Now.
            DateTimeOffset now = DateTimeOffset.Now;

            // Create the test.
            var test = ImmutableDictionary.CreateRange(EnumerableExtensions.From(
                new KeyValuePair<DateTimeOffset, string>(now, "hello"),
                new KeyValuePair<DateTimeOffset, string>(now.AddDays(1), "there")
            ));

            // Assert 
            AssertSerializedJson(test);
        }

        #endregion
    }
}
