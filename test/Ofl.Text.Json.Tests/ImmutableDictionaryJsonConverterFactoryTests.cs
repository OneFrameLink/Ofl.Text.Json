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

        #endregion

        #region Tests

        [Fact]
        public void Test_ImmutableDictionary_With_Non_String_Key_Serialization()
        {
            // Create the options.
            JsonSerializerOptions options = CreateJsonSerializerOptions();

            // Serialize a test with numbers.
            const string json = "{ \"1\": \"hello\", \"2\": \"there\" }";

            // Serialize.
            ImmutableDictionary<int, string> deserialized =
                JsonSerializer.Deserialize<ImmutableDictionary<int, string>>(json, options);

            // Validate the keys.
            Assert.Equal(2, deserialized.Count);
            Assert.Equal("hello", deserialized[1]);
            Assert.Equal("there", deserialized[2]);
        }

        #endregion
    }
}
