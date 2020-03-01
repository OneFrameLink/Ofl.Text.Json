using System;
using System.Text;
using System.Text.Json;
using Xunit;

namespace Ofl.Text.Json.Tests
{
    public class Utf8JsonReaderExtensionsTests
    {
        #region Tests

        [Theory]
        [InlineData(@"{ ""ignore"": { }, ""here"": null }")]
        [InlineData(@"{ ""ignore"": { ""ignored"": true }, ""here"": null }")]
        [InlineData(@"{ ""ignore"": { ""ignored"": { ""ignored"": true } }, ""here"": null }")]
        [InlineData(@"{ ""ignore"": { ""ignored"": [] }, ""here"": null }")]
        [InlineData(@"{ ""ignore"": [ ], ""here"": null }")]
        [InlineData(@"{ ""ignore"": [ true ], ""here"": null }")]
        [InlineData(@"{ ""ignore"": [ { } ], ""here"": null }")]
        [InlineData(@"{ ""ignore"": [ { ""ignored"": true } ], ""here"": null }")]
        [InlineData(@"{ ""ignore"": [ { ""ignored"": [ { ""ignored"": true } ] } ], ""here"": null }")]
        public void Test_SkipObjectOrArray(string json)
        {
            // Validate parameters.
            if (string.IsNullOrWhiteSpace(json)) throw new ArgumentNullException(nameof(json));

            // Get the bytes in utf8.
            ReadOnlySpan<byte> bytes = Encoding.UTF8.GetBytes(json);

            // Get the reader.
            var reader = new Utf8JsonReader(bytes);

            // Read until it is a property.
            while (reader.Read() && reader.TokenType != JsonTokenType.PropertyName)
            { }

            // The reader should be at the ignore property.
            Assert.Equal("ignore", reader.GetString());

            // Read to the next token.
            Assert.True(reader.Read());

            // Make the call.
            reader.SkipObjectOrArray();

            // This should be a property name, and "here".
            Assert.Equal(JsonTokenType.PropertyName, reader.TokenType);
            Assert.Equal("here", reader.GetString());
        }

        #endregion
    }
}
