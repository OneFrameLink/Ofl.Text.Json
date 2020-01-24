using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Ofl.Text.Json.Tests
{
    public class UnixTimestampDateTimeOffsetJsonConverterTests
    {
        private class TestClassNonNull
        {
            [JsonConverter(typeof(UnixTimestampDateTimeOffsetJsonConverter))]
            public DateTimeOffset Value { get; set; }
        }

        [Theory]
        [InlineData("{\"Value\":1493391600 }", "2017-04-28T15:00:00.0000000+00:00")]
        public void Test_ReadJson_NonNullable(string json, string dateTimeOffsetString)
        {
            // Validate parameters.
            if (json == null) throw new ArgumentNullException(nameof(json));

            // Get the date time offset.
            DateTimeOffset expected = DateTimeOffset.Parse(dateTimeOffsetString);

            // Deserialize.
            TestClassNonNull testClass = JsonSerializer.Deserialize<TestClassNonNull>(json);

            // Equal.
            Assert.Equal(expected, testClass.Value);
        }

        private class TestClassNullable
        {
            [JsonConverter(typeof(UnixTimestampDateTimeOffsetJsonConverter))]
            public DateTimeOffset? Value { get; set; }
        }

        [Theory]
        [InlineData("{\"Value\": null }", null)]
        [InlineData("{\"Value\":1493391600 }", "2017-04-28T15:00:00.0000000+00:00")]
        public void Test_ReadJson_Nullable(string json, string dateTimeOffsetString)
        {
            // Validate parameters.
            if (json == null) throw new ArgumentNullException(nameof(json));

            // Get the date time offset.
            DateTimeOffset? expected = string.IsNullOrWhiteSpace(dateTimeOffsetString)
                ? null
                : (DateTimeOffset?)DateTimeOffset.Parse(dateTimeOffsetString);

            // Deserialize.
            TestClassNullable testClass = JsonSerializer.Deserialize<TestClassNullable>(json);

            // Equal.
            Assert.Equal(expected, testClass.Value);
        }
    }
}
