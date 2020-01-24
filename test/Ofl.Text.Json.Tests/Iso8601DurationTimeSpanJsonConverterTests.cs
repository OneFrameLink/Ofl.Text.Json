using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Ofl.Text.Json.Tests
{
    public class Iso8601DurationTimeSpanJsonConverterTests
    {
        private class NonNullableTest
        {
            [JsonConverter(typeof(Iso8601DurationTimeSpanJsonConverter))]
            public TimeSpan Value { get; set; }
        }

        [Theory]
        [InlineData("{\"Value\":\"PT1H\" }", "01:00:00")]
        [InlineData("{\"Value\":\"PT1M\" }", "00:01:00")]
        [InlineData("{\"Value\":\"PT1S\" }", "00:00:01")]
        [InlineData("{\"Value\":\"PT1H1M\" }", "01:01:00")]
        [InlineData("{\"Value\":\"PT1H1S\" }", "01:00:01")]
        [InlineData("{\"Value\":\"PT1M1S\" }", "00:01:01")]
        [InlineData("{\"Value\":\"PT1H1M1S\" }", "01:01:01")]
        public void Test_ReadNonNullableJson(string json, string durationString)
        {
            // Validate parameters.
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentNullException(nameof(json));
            if (string.IsNullOrWhiteSpace(durationString)) throw new ArgumentNullException(nameof(durationString));

            // Get the expected value.
            TimeSpan expected = TimeSpan.Parse(durationString);

            // Map to an object.
            NonNullableTest actual = JsonSerializer.Deserialize<NonNullableTest>(json);

            // Equal.
            Assert.Equal(expected, actual.Value);
        }

        private class NullableTest
        {
            [JsonConverter(typeof(Iso8601DurationTimeSpanJsonConverter))]
            public TimeSpan? Value { get; set; }
        }

        [Theory]
        [InlineData("{\"Value\": null }", null)]
        [InlineData("{\"Value\":\"PT1H\" }", "01:00:00")]
        [InlineData("{\"Value\":\"PT1M\" }", "00:01:00")]
        [InlineData("{\"Value\":\"PT1S\" }", "00:00:01")]
        [InlineData("{\"Value\":\"PT1H1M\" }", "01:01:00")]
        [InlineData("{\"Value\":\"PT1H1S\" }", "01:00:01")]
        [InlineData("{\"Value\":\"PT1M1S\" }", "00:01:01")]
        [InlineData("{\"Value\":\"PT1H1M1S\" }", "01:01:01")]
        public void Test_ReadNullableJson(string json, string durationString)
        {
            // Validate parameters.
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentNullException(nameof(json));

            // Get the expected value.
            TimeSpan? expected = string.IsNullOrWhiteSpace(durationString) ? 
                (TimeSpan?) null : TimeSpan.Parse(durationString);

            // Map to an object.
            NullableTest actual = JsonSerializer.Deserialize<NullableTest>(json);

            // Equal.
            Assert.Equal(expected, actual.Value);
        }
    }
}
