using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Ofl.Text.Json.Tests
{
    public class JsonConstructorJsonConverterTests
    {
        #region Helpers

        [JsonConverter(typeof(JsonConstructorJsonConverter))]
        public class Test
        {
            #region Constructor

            [JsonConstructor]
            internal Test(string value)
            {
                // Assign parameters.
                // Normally we'd check against null here, but we want
                // to be able to test passing a default when the
                // JSON is not set.
                Value = value;
            }

            #endregion

            #region Instance, read-only state.

            public string Value { get; }

            #endregion
        }

        #endregion

        #region Tests

        [Fact]
        public void Test_JsonConstructor_Deserialize()
        {
            // The value.
            string expected = Guid.NewGuid().ToString("N");

            // The json.
            string json = @"{ ""value"": """ + expected + @""" }";

            // Create the options.
            var options = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Deserialize.
            Test actual = JsonSerializer.Deserialize<Test>(json, options);

            // Compare the value.
            Assert.Equal(expected, actual.Value);
        }

        [Fact]
        public void Test_JsonConstructor_Deserialize_With_Missing_Parameters()
        {
            // Create the options.
            var options = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Deserialize.
            Test actual = JsonSerializer.Deserialize<Test>("{}", options);

            // Compare the value.
            Assert.Null(actual.Value);
        }

        [Theory]
        [InlineData(@"{ ""value"": ""EXPECTED"", ""extra01"": 1 }")]
        [InlineData(@"{ ""value"": ""EXPECTED"", ""extra01"": 1, ""extra02"": 2 }")]
        public void Test_JsonConstructor_Deserialize_With_Extra_Json(string json)
        {
            // Validate parameters.
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentNullException(nameof(json));

            // Create the options.
            var options = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Deserialize.
            Test actual = JsonSerializer.Deserialize<Test>(json, options);

            // Compare the value.
            Assert.Equal("EXPECTED", actual.Value);
        }

        [Fact]
        public void Test_JsonConstructor_Serialize()
        {
            // Create the instance.
            var expected = new Test(Guid.NewGuid().ToString("N"));

            // Create the options.
            var options = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Serialize.
            string json = JsonSerializer.Serialize(expected, options);

            // Parse into a document.
            using var doc = JsonDocument.Parse(json);

            // Get the value from the document.
            string actual = doc
                .RootElement
                .GetProperty("value")
                .GetString();

            // Are they equal.
            Assert.Equal(expected.Value, actual);
        }

        #endregion
    }
}
